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
        private String text;
        public override string Text
        {
            get { return text; }
        }

        private int textOffsetX, textOffsetY;

        public MenuButton(String text, SpriteFont font, int width, int height)
            : base(width, height, font)
        {
            this.text = text;

            Vector2 measurements = font.MeasureString(text);
            textOffsetX = (int)((width - measurements.X) / 2);
            textOffsetY = (int)((height - measurements.Y) / 2);
        }

        public override void GetClicked()
        {
            base.GetClicked();

            TDGame.MainGame.BackToMainMenu();
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset, bool paused)
        {
            base.Draw(gameTime, batch, xOffset, yOffset, paused);

            Vector2 drawPosition = new Vector2(xOffset + textOffsetX, yOffset + textOffsetY);

            batch.DrawString(font, text, drawPosition, Color.White);
        }
    }
}
