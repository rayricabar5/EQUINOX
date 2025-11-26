using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using EQUINOX.Audio;
using EQUINOX.Features;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQUINOX.Display
{
    public class Pref
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
        private SavePref pref;

        public Pref(Canvas canvas)
        {
            this.red = new Pen(Color.Crimson);
            this.blue = new Pen(Color.DarkBlue);
            this.common = new Pen(Color.DarkGray);
            this.white = new Pen(Color.White);
            this.black = new Pen(Color.Black);
            this.light_blue = new Pen(Color.LightBlue);
            this.dark_green = new Pen(Color.DarkGreen);
            this.yellow = new Pen(Color.Yellow);

            this.rows = canvas.Mode.Rows;
            this.cols = canvas.Mode.Columns;
            var font = PCScreenFont.Default;

            canvas.DrawFilledRectangle(this.red, 40, 400, 75, 50);
            canvas.DrawFilledRectangle(this.white, 115, 400, 25, 50);
            canvas.DrawFilledRectangle(this.black, 140, 400, 25, 50);
            canvas.DrawString("Deck of Cards", font, this.white, 180, 415);

            canvas.DrawFilledRectangle(this.blue, 310, 400, 75, 50);
            canvas.DrawFilledRectangle(this.white, 385, 400, 25, 50);
            canvas.DrawFilledRectangle(this.light_blue, 410, 400, 25, 50);
            canvas.DrawString("Skyline", font, this.white, 460, 415);

            canvas.DrawFilledRectangle(this.dark_green, 570, 400, 75, 50);
            canvas.DrawFilledRectangle(this.yellow, 645, 400, 25, 50);
            canvas.DrawFilledRectangle(this.red, 670, 400, 25, 50);
            canvas.DrawString("Veggie Salad", font, this.white, 720, 415);
        }

        public bool tryPrefClick(Int32 mouseX, Int32 mouseY)
        {
            if (new Rectangle(mouseX, mouseY, 1, 1).IntersectsWith(new Rectangle(40, 400, 125, 50)))
            {
                pref.SavePreference("Deck of Cards");
                return true;
            }

            if (new Rectangle(mouseX, mouseY, 1, 1).IntersectsWith(new Rectangle(310, 400, 125, 50)))
            {
                pref.SavePreference("Skyline");
                return true;
            }

            if (new Rectangle(mouseX, mouseY, 1, 1).IntersectsWith(new Rectangle(570, 400, 125, 50)))
            {
                pref.SavePreference("Veggie Salad");
                return true;
            }

            return false;
        }

    }
}
