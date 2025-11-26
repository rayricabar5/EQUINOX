using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using EQUINOX.Audio;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EQUINOX.Display
{
    public class TabBar
    {
        private Pen pen1;
        private Pen pen2;
        private Pen pen3;
        private Int32 rows, cols;
        public Music sound;
        private Canvas canvas;
        public Pref pref;
        
        public TabBar(Canvas canvas)
        {
            this.pen1 = new Pen(Color.DeepPink);
            this.pen2 = new Pen(Color.White);
            this.pen3 = new Pen(Color.LightBlue);
            this.rows = canvas.Mode.Rows;
            this.cols = canvas.Mode.Columns;
            var font = PCScreenFont.Default;

            canvas.DrawFilledRectangle(this.pen1, 40, 650, 75, 75);
            canvas.DrawString("User Account Preferences", font, this.pen2, 125, 685);

            canvas.DrawFilledRectangle(this.pen3, 540, 650, 75, 75);
            canvas.DrawString("System Details", font, this.pen2, 625, 685);
        }

        public int tryTabClick(Int32 mouseX, Int32 mouseY)
        {
            if (new Rectangle(mouseX, mouseY, 1, 1).IntersectsWith(new Rectangle(40, 650, 75, 75)))
            {
                return 1;
            }

            if (new Rectangle(mouseX, mouseY, 1, 1).IntersectsWith(new Rectangle(540, 650, 75, 75)))
            {
                return 2;
            }

            return 0;
        }

    }
}
