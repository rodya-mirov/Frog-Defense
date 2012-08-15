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
        /// <summary>
        /// This is the list of enemies we're keeping track of;
        /// if the enemy is null, that denotes the beginning of the next wave
        /// </summary>
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

        private int numWaves;

        private void loadDefaultEnemies()
        {
            Enemy e;

            numWaves = 0;

            int waveInterval = 600;

            int lag = waveInterval;

            float level = 1f;
            float increaseAmount = .35f;

            while (numWaves < 40)
            {
                switch (numWaves % 4)
                {
                    case 0:
                        for (int i = 0; i < 15; i++)
                        {
                            e = new BasicEnemy(arena, env, -1, -1, level);
                            enemies.Enqueue(new EnemyTracker(e, lag));

                            lag += e.TicksAfterSpawn;
                        }
                        break;

                    case 1:
                        for (int i = 0; i < 18; i++)
                        {
                            e = new QuickEnemy(arena, env, -1, -1, level);
                            enemies.Enqueue(new EnemyTracker(e, lag));

                            lag += e.TicksAfterSpawn;
                        }
                        break;

                    case 2:
                        for (int i = 0; i < 8; i++)
                        {
                            e = new ToughEnemy(arena, env, -1, -1, level);
                            enemies.Enqueue(new EnemyTracker(e, lag));

                            lag += e.TicksAfterSpawn;
                        }
                        break;

                    case 3:
                        for (int i = 0; i < 15; i++)
                        {
                            e = new ImmuneEnemy(arena, env, -1, -1, level);
                            enemies.Enqueue(new EnemyTracker(e, lag));

                            lag += e.TicksAfterSpawn;
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }

                numWaves++;
                enemies.Enqueue(new EnemyTracker(null, lag));

                //enemies scale linearly
                level += increaseAmount;

                increaseAmount += .01f;

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

                if (e == null)
                {
                    numWaves--;
                }
                else
                {
                    env.spawnEnemy(e);
                }
            }
        }

        private int mouseX, mouseY;
        private bool mouseOnScreen;
        private Enemy mousedOverEnemy, selectedEnemy;
        public Enemy DetailEnemy
        {
            get { return selectedEnemy; }
        }

        public void UpdateMousePosition(int mouseX, int mouseY)
        {
            this.mouseX = mouseX;
            this.mouseY = mouseY;

            if (mouseX >= 0 && mouseX < PixelWidth && mouseY >= 0 && mouseY < PixelHeight)
            {
                mouseOnScreen = true;
                updateMousedOverEnemy();
            }
            else
            {
                mouseOnScreen = false;
            }
        }

        private const int mouseoverTolerance = 8;
        private void updateMousedOverEnemy()
        {
            int lagEstimate = mouseX * 2;

            mousedOverEnemy = null;

            foreach (EnemyTracker et in enemies)
            {
                if (Math.Abs(et.ticksRemaining - lagEstimate) <= mouseoverTolerance)
                {
                    mousedOverEnemy = et.enemy;
                    break;
                }
                else if (et.ticksRemaining > lagEstimate)
                {
                    break;
                }
            }
        }

        public void GetClicked()
        {
            if (mouseOnScreen)
            {
                if (mousedOverEnemy != null)
                {
                    selectedEnemy = mousedOverEnemy;
                    env.Player.DetailViewType = DetailViewType.EnemyPreview;
                }
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
                if (e.enemy != null)
                {
                    Vector2 drawPosition = new Vector2(xOffset + 1 + e.ticksRemaining / 2 - Enemy.PreviewImageWidth / 2, yOffset + 2);
                    batch.Draw(e.enemy.PreviewTexture, drawPosition, Color.White);
                }
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

        public void NextWave()
        {
            int n = enemies.Count;

            if (n == 0)
                return;

            int desiredThreshold = 10;

            int reduction = enemies.Peek().ticksRemaining - desiredThreshold;

            for (int i = 0; i < n; i++)
            {
                EnemyTracker et = enemies.Dequeue();
                et.ticksRemaining -= reduction;
                enemies.Enqueue(et);
            }
        }

        public int WavesRemaining
        {
            get { return numWaves; }
        }
    }
}
