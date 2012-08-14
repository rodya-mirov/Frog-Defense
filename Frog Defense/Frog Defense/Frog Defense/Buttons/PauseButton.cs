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
        protected const String unpausedText = "Pause";
        protected Point unpausedOffset;

        protected const String pausedText = "Unpause";
        protected Point pausedOffset;
        protected GameUpdater env;

        public override string Text
        {
            get
            {
                if (env.Paused)
                    return pausedText;
                else
                    return unpausedText;
            }
        }

        public override int TextOffsetX
        {
            get
            {
                if (env.Paused)
                    return pausedOffset.X;
                else
                    return unpausedOffset.X;
            }
        }

        public override int TextOffsetY
        {
            get
            {
                if (env.Paused)
                    return pausedOffset.Y;
                else
                    return unpausedOffset.Y;
            }
        }

        public PauseButton(GameUpdater env, SpriteFont font, int width, int height)
            : base(width, height, font)
        {
            this.env = env;

            Vector2 unpausedSize = font.MeasureString(unpausedText);
            Vector2 pausedSize = font.MeasureString(pausedText);

            unpausedOffset = new Point((int)((width - unpausedSize.X) / 2), (int)((height - unpausedSize.Y) / 2));
            pausedOffset = new Point((int)((width - pausedSize.X) / 2), (int)((height - pausedSize.Y) / 2));
        }

        public override void GetClicked()
        {
            base.GetClicked();

            env.Paused = !(env.Paused);
        }
    }
}
