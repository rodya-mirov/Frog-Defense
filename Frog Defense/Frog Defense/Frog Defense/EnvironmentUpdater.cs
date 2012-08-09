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
        private Queue<Enemy> enemies;

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

            enemies = new Queue<Enemy>();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Arena.LoadContent();
            Enemy.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            int numEnemies = enemies.Count;

            for (int i = 0; i < numEnemies; i++)
            {
                Enemy enemy = enemies.Dequeue();

                enemy.Update();

                if (enemy.IsAlive())
                    enemies.Enqueue(enemy);
            }

            arena.Update();

            if (enemies.Count < 1)
            {
                enemies.Enqueue(arena.makeEnemy());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            int xOffset = (TDGame.MainGame.GraphicsDevice.Viewport.Width - arena.PixelWidth) / 2;
            int yOffset = (TDGame.MainGame.GraphicsDevice.Viewport.Height - arena.PixelHeight) / 2;

            batch.Begin();

            arena.Draw(gameTime, batch, xOffset, yOffset);

            int numEnemies = enemies.Count;
            for (int i = 0; i < numEnemies; i++)
            {
                Enemy enemy = enemies.Dequeue();
                enemy.Draw(gameTime, batch, xOffset, yOffset);
                enemies.Enqueue(enemy);
            }

            batch.End();
        }
    }
}
