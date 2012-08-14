using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frog_Defense.Buttons
{
    class ClearSelectionButton : Button
    {
        public override string Text { get { return "Clear"; } }

        private int textOffsetX, textOffsetY;
        private GameUpdater env;

        public ClearSelectionButton(GameUpdater env, int width, int height, SpriteFont font)
            : base(width, height, font)
        {
            this.env = env;

            Vector2 measurements = font.MeasureString(Text);
            textOffsetX = (int)((width - measurements.X) / 2);
            textOffsetY = (int)((height - measurements.Y) / 2);
        }

        public override void GetClicked()
        {
            env.Player.PreviewType = SelectedPreviewType.None;
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset, bool paused)
        {
            base.Draw(gameTime, batch, xOffset, yOffset, paused);

            Vector2 drawPosition = new Vector2(xOffset + textOffsetX, yOffset + textOffsetY);

            batch.DrawString(font, Text, drawPosition, Color.White);
        }
    }
}
