using Cosmos.Core;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using EQUINOX.Audio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQUINOX.Display
{
    public class OS_Details
    {
        private Pen red;
        private Pen blue;
        private Pen common;
        private Pen white;
        private Pen black;
        private Pen light_blue;
        private Pen dark_green;
        private Pen yellow;
        private Int32 rows, cols;

        public Music sound;

        string[] lines = new string[]
        {
            "INDIE OPERATING SYSTEM",
            "Version 1.0.0 - Alpha Release",
            "Developed by the INDIE DEVS:",
            "- James Quincy Venedict T. Gonzales",
            "- Raymond Ashley B. Ricabar",
            "- Simon L. Sangalang",
            "- Clarence Dion M. Roque",
            "- Kurt Stephene M. Seles",
        };

        int startX = 40;       // starting X coordinate
        int startY = 300;      // starting Y coordinate
        int lineHeight = 20;   // vertical spacing between lines

        public OS_Details(Canvas canvas)
        {
            this.white = new Pen(Color.White);

            this.rows = canvas.Mode.Rows;
            this.cols = canvas.Mode.Columns;
            var font = PCScreenFont.Default;

            for (int i = 0; i < lines.Length; i++)
            {
                canvas.DrawString(lines[i], font, this.white, startX, startY + i * lineHeight);
            }
        }

        public int tryDetailsClick(Int32 mouseX, Int32 mouseY)
        {
            if (new Rectangle(mouseX, mouseY, 1, 1).IntersectsWith(new Rectangle(40, 400, 125, 50)))
            {
                sound = new Music();
                sound.DoReMi();
                return 1;
            }

            if (new Rectangle(mouseX, mouseY, 1, 1).IntersectsWith(new Rectangle(310, 400, 125, 50)))
            {
                sound = new Music();
                sound.DoReMi();
                return 2;
            }

            if (new Rectangle(mouseX, mouseY, 1, 1).IntersectsWith(new Rectangle(570, 400, 125, 50)))
            {
                sound = new Music();
                sound.DoReMi();
                return 3;
            }

            return 0;
        }
    }
}
