﻿using System;
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

        public MenuButton(SpriteFont font, int width, int height)
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

        public override void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset, bool paused)
        {
            base.Draw(gameTime, batch, xOffset, yOffset, paused);

            Vector2 drawPosition = new Vector2(xOffset + textOffsetX, yOffset + textOffsetY);

            batch.DrawString(font, Text, drawPosition, Color.White);
        }
    }
}
