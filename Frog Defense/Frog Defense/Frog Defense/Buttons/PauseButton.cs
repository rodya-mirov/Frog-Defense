using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frog_Defense.Buttons
{
    class PauseButton : Button
    {
        protected const String unpausedText = "PAUSE";
        protected Vector2 unpausedSize;
        protected Vector2 unpausedOffset;

        protected const String pausedText = "UNPAUSE";
        protected Vector2 pausedSize;
        protected Vector2 pausedOffset;

        protected bool paused;
        protected GameUpdater env;

        protected SpriteFont font;

        public PauseButton(GameUpdater env, SpriteFont font, int width, int height)
            : base(width, height)
        {
            this.env = env;
            this.font = font;

            unpausedSize = font.MeasureString(unpausedText);
            pausedSize = font.MeasureString(pausedText);

            unpausedOffset = new Vector2((int)((width - unpausedSize.X) / 2), (int)((height - unpausedSize.Y) / 2));
            pausedOffset = new Vector2((int)((width - pausedSize.X) / 2), (int)((height - pausedSize.Y) / 2));
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset, bool paused)
        {
            base.Draw(gameTime, batch, xOffset, yOffset, paused);

            this.paused = paused;

            Vector2 drawPosition;
            String toDraw;

            if (paused)
            {
                toDraw = pausedText;
                drawPosition = new Vector2(xOffset + pausedOffset.X, yOffset + pausedOffset.Y);
            }
            else
            {
                toDraw = unpausedText;
                drawPosition = new Vector2(xOffset + unpausedOffset.X, yOffset + unpausedOffset.Y);
            }

            batch.DrawString(font, toDraw, drawPosition, Color.White);
        }

        public override void GetClicked()
        {
            base.GetClicked();

            env.Paused = !(env.Paused);
        }
    }
}
