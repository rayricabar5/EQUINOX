using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Console = System.Console;
using Sys = Cosmos.System;

namespace EQUINOX.Display
{
    public class Redraw
    {
        private Pen pen1;
        private Pen pen2;
        private Color bg;

        public Redraw(Color background, Color pen1c, Color pen2c)
        {
            bg = background;
            pen1 = new Pen(pen1c);
            pen2 = new Pen(pen2c);
        }

        public void Draw(Canvas canvas)
        {
            canvas.Clear(bg);

            // I
            canvas.DrawFilledRectangle(pen1, 20, 20, 110, 25);
            canvas.DrawFilledRectangle(pen1, 60, 45, 25, 80);
            canvas.DrawFilledRectangle(pen1, 20, 125, 110, 25);

            // N
            canvas.DrawFilledRectangle(pen1, 160, 20, 25, 130);
            canvas.DrawFilledRectangle(pen1, 245, 20, 25, 130);
            canvas.DrawFilledRectangle(pen1, 185, 46, 21, 26);
            canvas.DrawFilledRectangle(pen1, 206, 72, 21, 26);
            canvas.DrawFilledRectangle(pen1, 227, 98, 21, 26);
            canvas.DrawFilledRectangle(pen1, 248, 124, 22, 26);

            // D
            canvas.DrawFilledRectangle(pen1, 300, 20, 25, 130);
            canvas.DrawFilledRectangle(pen1, 385, 40, 25, 90);
            canvas.DrawFilledRectangle(pen1, 325, 20, 40, 13);
            canvas.DrawFilledRectangle(pen1, 355, 33, 40, 13);
            canvas.DrawFilledRectangle(pen1, 325, 137, 40, 13);
            canvas.DrawFilledRectangle(pen1, 355, 124, 40, 13);

            // I
            canvas.DrawFilledRectangle(pen1, 440, 20, 110, 25);
            canvas.DrawFilledRectangle(pen1, 480, 45, 25, 80);
            canvas.DrawFilledRectangle(pen1, 440, 125, 110, 25);

            // E
            canvas.DrawFilledRectangle(pen1, 580, 20, 25, 130);
            canvas.DrawFilledRectangle(pen1, 605, 20, 85, 26);
            canvas.DrawFilledRectangle(pen1, 605, 72, 85, 26);
            canvas.DrawFilledRectangle(pen1, 605, 124, 85, 26);

            // O
            canvas.DrawFilledRectangle(pen2, 505, 140, 25, 130);
            canvas.DrawFilledRectangle(pen2, 590, 140, 25, 130);
            canvas.DrawFilledRectangle(pen2, 530, 140, 60, 25);
            canvas.DrawFilledRectangle(pen2, 530, 245, 60, 25);

            // S
            canvas.DrawFilledRectangle(pen2, 670, 140, 85, 26);
            canvas.DrawFilledRectangle(pen2, 645, 140, 25, 78);
            canvas.DrawFilledRectangle(pen2, 670, 192, 60, 26);
            canvas.DrawFilledRectangle(pen2, 730, 192, 25, 78);
            canvas.DrawFilledRectangle(pen2, 645, 244, 85, 26);
        }
    }

}
