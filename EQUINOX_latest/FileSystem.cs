using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cosmos.System.FileSystem.VFS;
using Sys = Cosmos.System;

namespace EQUINOX
{
    public abstract class FsNode
    {
        public string Name { get; set; }
        public DirectoryNode Parent { get; set; }
        public DateTime Created { get; } = DateTime.UtcNow;
        public DateTime Modified { get; set; } = DateTime.UtcNow;

        protected FsNode(string name, DirectoryNode parent)
        {
            Name = name;
            Parent = parent;
        }

        public string FullPath
        {
            get
            {
                var parts = new List<string>();
                FsNode cur = this;
                while (cur != null)
                {
                    parts.Add(cur.Name);
                    cur = cur.Parent;
                }
                parts.Reverse(); // root has empty name
                return "/" + string.Join("/", parts.Skip(1));
            }
        }
    }

    public class FileNode : FsNode
    {
        public string Content { get; set; } = string.Empty;
        public FileNode(string name, DirectoryNode parent) : base(name, parent) { }
    }

    public class DirectoryNode : FsNode
    {
        public Dictionary<string, FsNode> Children { get; } =
            new Dictionary<string, FsNode>(StringComparer.OrdinalIgnoreCase);
        public DirectoryNode(string name, DirectoryNode parent) : base(name, parent) { }
    }

    public class FileSystem
    {
        private readonly DirectoryNode _root;
        private DirectoryNode _cwd;

        public FileSystem()
        {
            _root = new DirectoryNode(string.Empty, null);
            _cwd = _root;
        }

        public string Pwd() => _cwd == _root ? "/" : _cwd.FullPath;

        public bool Cd(string path, out string message)
        {
            if (string.IsNullOrWhiteSpace(path) || path == "/")
            {
                _cwd = _root;
                message = Pwd();
                return true;
            }
            bool absolute = path.StartsWith("/");
            var parts = NormalizeParts(path);
            var cur = absolute ? _root : _cwd;
            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                if (!cur.Children.TryGetValue(part, out var node) || node is not DirectoryNode dir)
                {
                    message = $"Directory not found: {part}";
                    return false;
                }
                cur = dir;
            }
            _cwd = cur;
            message = Pwd();
            return true;
        }

        public bool Mkdir(string path, out string message)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                message = "Invalid path.";
                return false;
            }
            bool absolute = path.StartsWith("/");
            var parts = NormalizeParts(path);
            var cur = absolute ? _root : _cwd;
            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                if (!cur.Children.TryGetValue(part, out var node))
                {
                    var nd = new DirectoryNode(part, cur);
                    cur.Children[part] = nd;
                    cur = nd;
                }
                else
                {
                    if (node is DirectoryNode existingDir) cur = existingDir;
                    else { message = "A file with that name exists."; return false; }
                }
            }
            message = $"Directory: {cur.FullPath}";
            return true;
        }

        public bool Touch(string path, out string message)
        {
            var (parent, name) = ResolveParent(path);
            if (parent == null || string.IsNullOrEmpty(name))
            {
                message = "Invalid path.";
                return false;
            }
            if (parent.Children.TryGetValue(name, out var node))
            {
                if (node is FileNode f)
                {
                    f.Modified = DateTime.UtcNow;
                    message = $"Touched: {f.FullPath}";
                    return true;
                }
                message = "A directory with that name exists.";
                return false;
            }
            var file = new FileNode(name, parent);
            parent.Children[name] = file;
            message = $"Created file: {file.FullPath}";
            return true;
        }

        public bool WriteFile(string path, string content, bool append, out string message)
        {
            var (parent, name) = ResolveParent(path);
            if (parent == null || string.IsNullOrEmpty(name))
            {
                message = "Invalid path.";
                return false;
            }
            EnsureDirectoryExists(GetParentPath(path));
            if (!parent.Children.TryGetValue(name, out var node))
            {
                node = new FileNode(name, parent);
                parent.Children[name] = node;
            }
            if (node is FileNode file)
            {
                file.Content = append ? file.Content + content : content;
                file.Modified = DateTime.UtcNow;
                message = $"Wrote: {file.FullPath}";
                return true;
            }
            message = "Path is a directory.";
            return false;
        }

        public bool ReadFile(string path, out string content)
        {
            content = null;
            var (parent, name) = ResolveParent(path);
            if (parent == null || string.IsNullOrEmpty(name)) return false;
            if (!parent.Children.TryGetValue(name, out var node)) return false;
            if (node is FileNode file)
            {
                content = file.Content;
                return true;
            }
            return false;
        }

        public bool Delete(string path, out string message)
        {
            var (parent, name) = ResolveParent(path);
            if (parent == null || string.IsNullOrEmpty(name))
            {
                message = "Invalid path.";
                return false;
            }
            if (!parent.Children.TryGetValue(name, out var node))
            {
                message = "Not found.";
                return false;
            }
            if (node is DirectoryNode dir && dir.Children.Count > 0)
            {
                message = "Directory not empty.";
                return false;
            }
            parent.Children.Remove(name);
            message = $"Deleted: {path}";
            return true;
        }

        public bool Rename(string path, string newName, out string message)
        {
            var (parent, name) = ResolveParent(path);
            if (parent == null || string.IsNullOrEmpty(name))
            {
                message = "Invalid path.";
                return false;
            }
            if (!parent.Children.TryGetValue(name, out var node))
            {
                message = "Not found.";
                return false;
            }
            if (parent.Children.ContainsKey(newName))
            {
                message = "Target name exists.";
                return false;
            }
            parent.Children.Remove(name);
            node.Name = newName;
            parent.Children[newName] = node;
            message = $"Renamed to {newName}";
            return true;
        }

        public string[] Ls(string path = null)
        {
            DirectoryNode dir;
            if (string.IsNullOrEmpty(path))
            {
                dir = _cwd;
            }
            else
            {
                bool absolute = path.StartsWith("/");
                var parts = NormalizeParts(path);
                dir = absolute ? _root : _cwd;
                for (int i = 0; i < parts.Length; i++)
                {
                    var part = parts[i];
                    if (!dir.Children.TryGetValue(part, out var node) || node is not DirectoryNode nd)
                    {
                        return Array.Empty<string>();
                    }
                    dir = nd;
                }
            }

            var keys = new string[dir.Children.Count];
            dir.Children.Keys.CopyTo(keys, 0);

            var result = new List<string>(keys.Length);
            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                var node = dir.Children[key];
                var type = node is DirectoryNode ? "dir " : "file";

                var dt = node.Modified;
                string Two(int val) => val < 10 ? "0" + val : val.ToString();
                string timestamp = dt.Year + "-" + Two(dt.Month) + "-" + Two(dt.Day) + " " +
                                   Two(dt.Hour) + ":" + Two(dt.Minute) + ":" + Two(dt.Second);

                result.Add(type + "\t" + node.Name + "\t" + timestamp);
            }

            return result.ToArray();
        }

        public bool SaveToVfs(string vfsPath, out string message)
        {
            if (!IsVfsRegistered())
            {
                message = "Save failed: VFS not registered.";
                return false;
            }
            try
            {
                var lines = new List<string>();
                var allDirs = GetAllDirectories();
                for (int i = 0; i < allDirs.Count; i++)
                {
                    var d = allDirs[i];
                    if (d == _root) continue;
                    lines.Add($"DIR|{d.FullPath}");
                }

                var allFiles = GetAllFiles();
                for (int i = 0; i < allFiles.Count; i++)
                {
                    var f = allFiles[i];
                    var b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(f.Content ?? string.Empty));
                    lines.Add($"FILE|{f.FullPath}|{b64}|{f.Modified.Ticks}");
                }

                using (var fs = File.Open(vfsPath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    for (int i = 0; i < lines.Count; i++)
                        sw.WriteLine(lines[i]);
                    sw.Flush();
                    fs.Flush();
                }

                int dirCount = Math.Max(0, allDirs.Count - 1);
                int fileCount = allFiles.Count;
                message = $"Saved {dirCount} dirs, {fileCount} files to {vfsPath}";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Save failed: {ex.Message}";
                return false;
            }
        }

        public bool LoadFromVfs(string vfsPath, out string message)
        {
            if (!IsVfsRegistered())
            {
                message = "Load failed: VFS not registered.";
                return false;
            }
            try
            {
                if (!File.Exists(vfsPath))
                {
                    message = "Persistence file not found.";
                    return false;
                }
                var lines = File.ReadAllLines(vfsPath);
                _root.Children.Clear();
                _cwd = _root;
                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    line = line.Trim();
                    var parts = line.Split(new[] { '|' }, 4);
                    if (parts.Length == 0) continue;

                    var tag = parts[0].Trim();

                    if (tag == "DIR" && parts.Length >= 2)
                    {
                        var dirPath = parts[1].Trim();
                        if (string.IsNullOrEmpty(dirPath) || dirPath == "/") continue;
                        EnsureDirectoryExists(dirPath);
                    }
                    else if (tag == "FILE" && parts.Length >= 3)
                    {
                        var path = parts[1].Trim();
                        var b64 = parts[2].Trim();
                        string content;
                        try { content = Encoding.UTF8.GetString(Convert.FromBase64String(b64)); }
                        catch { content = string.Empty; }

                        EnsureDirectoryExists(GetParentPath(path));
                        var (parent, name) = ResolveParent(path);
                        if (parent != null && !string.IsNullOrEmpty(name))
                        {
                            var file = new FileNode(name, parent) { Content = content };
                            if (parts.Length == 4 && long.TryParse(parts[3].Trim(), out var ticks))
                                file.Modified = new DateTime(ticks, DateTimeKind.Utc);
                            parent.Children[name] = file;
                        }
                    }
                }
                message = $"Loaded from {vfsPath}";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Load failed: {ex.Message}";
                return false;
            }
        }

        public bool Copy(string srcPath, string destPath, out string message)
        {
            message = null;
            var (srcParent, srcName) = ResolveParent(srcPath);
            if (srcParent == null || string.IsNullOrEmpty(srcName))
            {
                message = "Invalid source path.";
                return false;
            }
            if (!srcParent.Children.TryGetValue(srcName, out var srcNode))
            {
                message = "Source not found.";
                return false;
            }

            bool destIsDirTarget = false;
            DirectoryNode destDirNode = null;

            if (!string.IsNullOrWhiteSpace(destPath))
            {
                var maybeDir = ResolveDirectory(destPath);
                if (maybeDir != null)
                {
                    destIsDirTarget = true;
                    destDirNode = maybeDir;
                }
            }

            if (destIsDirTarget)
            {
                if (destDirNode.Children.ContainsKey(srcNode.Name))
                {
                    message = "Destination already contains an item named " + srcNode.Name;
                    return false;
                }
                if (srcNode is FileNode sf)
                {
                    var nf = new FileNode(srcNode.Name, destDirNode)
                    {
                        Content = sf.Content,
                        Modified = DateTime.UtcNow
                    };
                    destDirNode.Children[nf.Name] = nf;
                    message = "Copied file to " + nf.FullPath;
                    return true;
                }
                if (srcNode is DirectoryNode sd)
                {
                    if (IsDescendant(sd, destDirNode))
                    {
                        message = "Cannot copy a directory into its descendant.";
                        return false;
                    }
                    var nd = new DirectoryNode(sd.Name, destDirNode);
                    destDirNode.Children[nd.Name] = nd;
                    CopyDirectoryRecursive(sd, nd);
                    message = "Copied directory to " + nd.FullPath;
                    return true;
                }
            }
            else
            {
                var (destParent, destName) = ResolveParent(destPath);
                if (destParent == null || string.IsNullOrEmpty(destName))
                {
                    message = "Invalid destination path.";
                    return false;
                }
                if (destParent.Children.ContainsKey(destName))
                {
                    message = "Destination exists.";
                    return false;
                }
                if (srcNode is FileNode sf)
                {
                    var nf = new FileNode(destName, destParent)
                    {
                        Content = sf.Content,
                        Modified = DateTime.UtcNow
                    };
                    destParent.Children[destName] = nf;
                    message = "Copied file to " + nf.FullPath;
                    return true;
                }
                if (srcNode is DirectoryNode sd)
                {
                    if (IsDescendant(sd, destParent))
                    {
                        message = "Cannot copy a directory into its descendant.";
                        return false;
                    }
                    var nd = new DirectoryNode(destName, destParent);
                    destParent.Children[destName] = nd;
                    CopyDirectoryRecursive(sd, nd);
                    message = "Copied directory to " + nd.FullPath;
                    return true;
                }
            }

            message = "Unsupported node type.";
            return false;
        }

        private void CopyDirectoryRecursive(DirectoryNode source, DirectoryNode target)
        {
            foreach (var kv in source.Children)
            {
                if (kv.Value is FileNode f)
                {
                    var nf = new FileNode(f.Name, target)
                    {
                        Content = f.Content,
                        Modified = DateTime.UtcNow
                    };
                    target.Children[nf.Name] = nf;
                }
                else if (kv.Value is DirectoryNode d)
                {
                    var nd = new DirectoryNode(d.Name, target);
                    target.Children[nd.Name] = nd;
                    CopyDirectoryRecursive(d, nd);
                }
            }
        }

        private bool IsDescendant(DirectoryNode candidateAncestor, DirectoryNode possibleDescendant)
        {
            var cur = possibleDescendant;
            while (cur != null)
            {
                if (cur == candidateAncestor) return true;
                cur = cur.Parent;
            }
            return false;
        }

        public string[] Find(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Array.Empty<string>();
            term = term.Trim();
            var results = new List<string>();
            var stack = new Stack<DirectoryNode>();
            stack.Push(_root);
            while (stack.Count > 0)
            {
                var dir = stack.Pop();
                foreach (var kv in dir.Children)
                {
                    if (kv.Value.Name.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                        results.Add(kv.Value is DirectoryNode ? kv.Value.FullPath + "/" : kv.Value.FullPath);
                    if (kv.Value is DirectoryNode dd)
                        stack.Push(dd);
                }
            }

            for (int i = 0; i < results.Count - 1; i++)
            {
                int min = i;
                for (int j = i + 1; j < results.Count; j++)
                {
                    if (string.Compare(results[j], results[min], StringComparison.OrdinalIgnoreCase) < 0)
                        min = j;
                }
                if (min != i)
                {
                    var tmp = results[i];
                    results[i] = results[min];
                    results[min] = tmp;
                }
            }

            return results.ToArray();
        }

        private List<DirectoryNode> GetAllDirectories()
        {
            var result = new List<DirectoryNode>();
            var stack = new Stack<DirectoryNode>();
            stack.Push(_root);
            while (stack.Count > 0)
            {
                var dir = stack.Pop();
                result.Add(dir);
                foreach (var kv in dir.Children)
                {
                    if (kv.Value is DirectoryNode childDir)
                        stack.Push(childDir);
                }
            }
            return result;
        }

        private List<FileNode> GetAllFiles()
        {
            var result = new List<FileNode>();
            var stack = new Stack<DirectoryNode>();
            stack.Push(_root);
            while (stack.Count > 0)
            {
                var dir = stack.Pop();
                foreach (var kv in dir.Children)
                {
                    if (kv.Value is FileNode f) result.Add(f);
                    else if (kv.Value is DirectoryNode d) stack.Push(d);
                }
            }
            return result;
        }

        private void EnsureDirectoryExists(string path)
        {
            if (string.IsNullOrEmpty(path) || path == "/") return;
            bool abs = path.StartsWith("/");
            var parts = NormalizeParts(path);
            var cur = abs ? _root : _cwd;
            for (int i = 0; i < parts.Length; i++)
            {
                var p = parts[i];
                if (!cur.Children.TryGetValue(p, out var node) || node is not DirectoryNode dir)
                {
                    var nd = new DirectoryNode(p, cur);
                    cur.Children[p] = nd;
                    cur = nd;
                }
                else cur = dir;
            }
        }

        private (DirectoryNode parent, string name) ResolveParent(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return (_cwd, string.Empty);
            bool abs = path.StartsWith("/");
            var parts = NormalizeParts(path);
            if (parts.Length == 0) return (_cwd, string.Empty);
            var name = parts[parts.Length - 1];
            var parentParts = new string[parts.Length - 1];
            for (int i = 0; i < parentParts.Length; i++) parentParts[i] = parts[i];
            var parent = abs ? _root : _cwd;
            for (int i = 0; i < parentParts.Length; i++)
            {
                var p = parentParts[i];
                if (!parent.Children.TryGetValue(p, out var node) || node is not DirectoryNode dir)
                    return (null, null);
                parent = dir;
            }
            return (parent, name);
        }

        private DirectoryNode ResolveDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
                return _cwd;
            bool absolute = path.StartsWith("/");
            var parts = NormalizeParts(path);
            var cur = absolute ? _root : _cwd;
            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                if (!cur.Children.TryGetValue(part, out var node) || node is not DirectoryNode nd)
                    return null;
                cur = nd;
            }
            return cur;
        }

        private string GetParentPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return "/";
            var parts = NormalizeParts(path);
            if (parts.Length <= 1) return "/";
            var sb = new StringBuilder();
            for (int i = 0; i < parts.Length - 1; i++)
            {
                sb.Append('/');
                sb.Append(parts[i]);
            }
            return sb.ToString();
        }

        private string[] NormalizeParts(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return Array.Empty<string>();
            var raw = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var stack = new List<string>();
            for (int i = 0; i < raw.Length; i++)
            {
                var part = raw[i];
                if (part == ".") continue;
                if (part == "..")
                {
                    if (stack.Count > 0) stack.RemoveAt(stack.Count - 1);
                    continue;
                }
                stack.Add(part);
            }
            return stack.ToArray();
        }

        private bool IsVfsRegistered()
        {
            try
            {
                VFSManager.ThrowIfNotRegistered();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}