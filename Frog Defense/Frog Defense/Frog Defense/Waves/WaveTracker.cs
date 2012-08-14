using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frog_Defense.Enemies;

namespace Frog_Defense.Waves
{
    struct EnemyTracker
    {
        public Enemy enemy;
        public int ticksRemaining;

        public EnemyTracker(Enemy enemy, int ticksRemaining)
        {
            this.enemy = enemy;
            this.ticksRemaining = ticksRemaining;
        }
    }

    class WaveTracker
    {
        private Queue<EnemyTracker> enemies;
        private Queue<Enemy> toBeSpawned;

        private ArenaManager env;
        private ArenaMap arena;

        public static int PixelHeight
        {
            get { return 24; }
        }
        public static int PixelWidth
        {
            get { return TDGame.MainGame.GraphicsDevice.Viewport.Width; }
        }

        public WaveTracker(ArenaMap arena, ArenaManager env)
        {
            this.arena = arena;
            this.env = env;
            this.enemies = new Queue<EnemyTracker>();
            this.toBeSpawned = new Queue<Enemy>();

            loadDefaultEnemies();
        }

        private void loadDefaultEnemies()
        {
            Enemy e;

            int waveInterval = 600;

            int lag = waveInterval;

            for (float level = 1f; level < 7f; level += 1f)
            {

                for (int i = 0; i < 15; i++)
                {
                    e = new BasicEnemy(arena, env, -1, -1, level);
                    enemies.Enqueue(new EnemyTracker(e, lag));

                    lag += e.TicksAfterSpawn;
                }

                lag += waveInterval;

                for (int i = 0; i < 18; i++)
                {
                    e = new QuickEnemy(arena, env, -1, -1, level);
                    enemies.Enqueue(new EnemyTracker(e, lag));

                    lag += e.TicksAfterSpawn;
                }

                lag += waveInterval;

                for (int i = 0; i < 8; i++)
                {
                    e = new ToughEnemy(arena, env, -1, -1, level);
                    enemies.Enqueue(new EnemyTracker(e, lag));

                    lag += e.TicksAfterSpawn;
                }

                lag += waveInterval;

                for (int i = 0; i < 15; i++)
                {
                    e = new ImmuneEnemy(arena, env, -1, -1, level);
                    enemies.Enqueue(new EnemyTracker(e, lag));

                    lag += e.TicksAfterSpawn;
                }

                lag += waveInterval;
            }
        }

        public void Update()
        {
            int numEnemies = enemies.Count;
            for (int i = 0; i < numEnemies; i++)
            {
                EnemyTracker et = enemies.Dequeue();
                et.ticksRemaining--;

                if (et.ticksRemaining > 0)
                {
                    enemies.Enqueue(et);
                }
                else
                {
                    toBeSpawned.Enqueue(et.enemy);
                }
            }

            if (toBeSpawned.Count > 0)
            {
                Enemy e = toBeSpawned.Dequeue();
                env.spawnEnemy(e);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset)
        {
            //draw the backdrop ...
            Rectangle drawRect = new Rectangle(xOffset, yOffset, PixelWidth, PixelHeight);

            batch.Draw(TDGame.PureBlack, drawRect, Color.White);

            drawRect.X += 1;
            drawRect.Y += 1;
            drawRect.Width -= 2;
            drawRect.Height -= 2;

            batch.Draw(TDGame.PureWhite, drawRect, Color.White);

            //now draw each enemy's preview
            foreach (EnemyTracker e in enemies)
            {
                Vector2 drawPosition = new Vector2(xOffset + 1 + e.ticksRemaining/2 - Enemy.PreviewImageWidth, yOffset + 2);
                batch.Draw(e.enemy.PreviewTexture, drawPosition, Color.White);
            }
        }

        /// <summary>
        /// Determines whether the pipe is completely clear; that is,
        /// if there are no enemies left to be spawned, ever!
        /// </summary>
        /// <returns></returns>
        public bool isClear()
        {
            return enemies.Count == 0 && toBeSpawned.Count == 0;
        }
    }
}
