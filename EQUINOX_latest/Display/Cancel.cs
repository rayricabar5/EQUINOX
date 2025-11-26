using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System.Graphics;
using System.Drawing;
using Sys = Cosmos.System;
using Cosmos.System;
using System.Runtime.CompilerServices;
using Console = System.Console;
using Cosmos.System.Graphics.Fonts;

namespace EQUINOX.Display
{
    public class Cancel
    {
        private Pen pen1;
        private Pen pen2;
        private Int32 rows, cols;
        public Cancel(Canvas canvas) 
        {
            this.pen1 = new Pen(Color.White);
            this.pen2 = new Pen(Color.Crimson);
            this.rows = canvas.Mode.Rows;
            this.cols = canvas.Mode.Columns;
            var font = PCScreenFont.Default;

            canvas.DrawFilledRectangle(this.pen2, 925, 20, 75, 75);
            canvas.DrawString("X", font, this.pen1, 960, 50);
        }

        public bool tryCancelClick (Int32 mouseX, Int32 mouseY)
        {
            if (new Rectangle(mouseX, mouseY, 1, 1).IntersectsWith(new Rectangle(925, 20, 75, 75))){
                Console.Beep();
                return true;
            }

            return false;
        }
    }
}
