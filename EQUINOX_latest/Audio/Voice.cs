using Cosmos.HAL.Audio;
using Cosmos.HAL.BlockDevice.Registers;
using Cosmos.HAL.Drivers.PCI.Audio;
using Cosmos.System;
using Cosmos.System.Audio;
using Cosmos.System.Audio.IO;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using EQUINOX.Display;
using EQUINOX.Features;
using IL2CPU.API.Attribs;
using IL2CPU.API.Attribs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Console = System.Console;
using Sys = Cosmos.System;

namespace EQUINOX.Audio
{
    public class Voice
    {

        [ManifestResourceStream(ResourceName = "EQUINOX_latest.tagline.wav")] public static byte[] tagline;
        public void VoiceOver()
        {
            var mixer = new AudioMixer();
            //var audioStream = MemoryAudioStream.FromWave(tagline);
            var audioStream = new MemoryAudioStream(new SampleFormat(AudioBitDepth.Bits16, 2, true), 48000, tagline);
            var driver = AC97.Initialize(bufferSize: 4096);
            mixer.Streams.Add(audioStream);

            var audioManager = new AudioManager()
            {
                Stream = mixer,
                Output = driver
            };
            audioManager.Enable();
        }
    }
}
