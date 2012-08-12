using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frog_Defense.Waves
{
    class WaveTracker
    {
        public int PixelHeight
        {
            get { return 20; }
        }

        public int PixelWidth
        {
            get { return TDGame.MainGame.GraphicsDevice.Viewport.Width; }
        }

        private GameUpdater env;

        public WaveTracker(GameUpdater env)
        {
            this.env = env;
        }

        public void Update()
        {
        }

        public void Draw(GameTime gameTime, SpriteBatch batch, float xOffset, float yOffset)
        {

        }
    }
}
