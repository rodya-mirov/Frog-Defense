using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frog_Defense.Buttons
{
    class MenuButton : Button
    {
        public override string Text
        {
            get { return "Main Menu"; }
        }

        private int textOffsetX, textOffsetY;
        public override int TextOffsetX
        {
            get { return textOffsetX; }
        }
        public override int TextOffsetY
        {
            get { return textOffsetY; }
        }

        public MenuButton(int width, int height, SpriteFont font)
            : base(width, height, font)
        {
            Vector2 measurements = font.MeasureString(Text);
            textOffsetX = (int)((width - measurements.X) / 2);
            textOffsetY = (int)((height - measurements.Y) / 2);
        }

        public override void GetClicked()
        {
            base.GetClicked();

            TDGame.MainGame.BackToMainMenu();
        }
    }
}
