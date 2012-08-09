using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frog_Defense
{
    /// <summary>
    /// This is currently the only GameComponent, and overrides the typical content loading system and
    /// (more importantly) the main game loop.  Manages the contained Arena, its traps, and the enemies.
    /// 
    /// It also handles the drawing and contains the only SpriteBatch, which gets passed around.
    /// </summary>
    class EnvironmentUpdater : DrawableGameComponent
    {
        private Arena arena;

        private SpriteBatch batch;

        /// <summary>
        /// Only constructor for EnvironmentUpdater
        /// </summary>
        public EnvironmentUpdater()
            : base(TDGame.MainGame)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            arena = new Arena(this);
            batch = new SpriteBatch(TDGame.MainGame.GraphicsDevice);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Arena.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            batch.Begin();

            arena.Draw(gameTime, batch);

            batch.End();
        }
    }
}
