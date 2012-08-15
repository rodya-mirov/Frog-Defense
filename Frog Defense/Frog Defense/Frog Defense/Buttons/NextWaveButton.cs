using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frog_Defense.Buttons
{
    class NextWaveButton : Button
    {
        public override string Text
        {
            get
            {
                return "Next Wave";
            }
        }

        private GameUpdater env;

        private int textOffsetX, textOffsetY;

        public override int TextOffsetX { get { return textOffsetX; } }
        public override int TextOffsetY { get { return textOffsetY; } }

        public NextWaveButton(GameUpdater env, int width, int height, SpriteFont font)
            : base(width, height, font)
        {
            this.env = env;

            Vector2 measurement = font.MeasureString(Text);

            textOffsetX = (int)((width - measurement.X) / 2f);
            textOffsetY = (int)((height - measurement.Y) / 2f);
        }

        /// <summary>
        /// Sells a tower, if there is one
        /// </summary>
        public override void GetClicked()
        {
            env.ArenaManager.WaveTracker.NextWave();
        }
    }
}
