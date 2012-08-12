using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frog_Defense.Waves;
using Frog_Defense.Enemies;
using Frog_Defense.Traps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frog_Defense
{
    class ArenaManager
    {
        private ArenaMap arenaMap;
        public ArenaMap Map { get { return arenaMap; } }

        private WaveTracker waveTracker;
        public WaveTracker WaveTracker { get { return waveTracker; } }

        private Queue<Enemy> enemies;
        private Queue<Enemy> enemiesToBeAdded;

        private Queue<Trap> traps;
        private Queue<Trap> trapsToBeAdded;

        private GameUpdater env;

        public int PixelWidthArena
        {
            get { return arenaMap.PixelWidth; }
        }
        public int PixelHeightArena
        {
            get { return arenaMap.PixelHeight; }
        }

        public PlayerHUD Player
        {
            get { return env.Player; }
        }

        private TrapType selectedTrapType;
        public TrapType SelectedTrapType
        {
            get { return selectedTrapType; }
            set { selectedTrapType = value; }
        }

        public ArenaManager(GameUpdater env)
        {
            this.env = env;
            this.selectedTrapType = TrapType.NoType;
        }

        public static void LoadContent()
        {
            ArenaMap.LoadContent();

            Enemy.LoadContent();

            Trap.LoadContent();
        }

        public void ResetGame()
        {
            arenaMap = new ArenaMap(this);
            waveTracker = new WaveTracker(arenaMap, this);

            enemies = new Queue<Enemy>();
            enemiesToBeAdded = new Queue<Enemy>();

            traps = new Queue<Trap>();
            trapsToBeAdded = new Queue<Trap>();

            framesOfWin = 0;
        }

        public void GetClicked()
        {
            arenaMap.GetClicked();
        }

        public void addTrap(Trap t)
        {
            trapsToBeAdded.Enqueue(t);
        }

        public void addEnemy(Enemy e)
        {
            enemiesToBeAdded.Enqueue(e);
        }

        public void Update(GameTime gameTime, bool paused)
        {
            //first, add in all the queued up new friends :)
            foreach (Trap t in trapsToBeAdded)
                traps.Enqueue(t);

            trapsToBeAdded.Clear();

            //most things won't happen during paused time
            if (!paused)
            {
                foreach (Enemy e in enemiesToBeAdded)
                    enemies.Enqueue(e);

                enemiesToBeAdded.Clear();

                //update things that do things
                updateEnemies();
                updateTraps();

                //prep for spawning
                waveTracker.Update();
            }

            detectWin();
        }

        private const int framesBeforeWin = 60;
        private int framesOfWin;

        private void detectWin()
        {
            if (enemies.Count == 0 && waveTracker.isClear())
            {
                framesOfWin++;
                if (framesOfWin >= framesBeforeWin)
                {
                    TDGame.MainGame.Win();
                }
            }
        }

        /// <summary>
        /// Helper method for Update.  This updates all the enemies through cycling,
        /// and simply forgets to add them back in if the enemy is dead.  It also
        /// credits the player with the kill.
        /// </summary>
        private void updateEnemies()
        {
            int numEnemies = enemies.Count;

            for (int i = 0; i < numEnemies; i++)
            {
                Enemy enemy = enemies.Dequeue();

                enemy.Update();

                if (enemy.HasReachedGoal)
                {
                    Player.TakeHit();
                }
                else if (enemy.IsAlive)
                {
                    enemies.Enqueue(enemy);
                }
                else
                {
                    Player.AddMoney(enemy.CashValue);
                }
            }
        }

        /// <summary>
        /// Just cycles through the traps and updates them
        /// </summary>
        private void updateTraps()
        {
            int numTraps = traps.Count;
            for (int i = 0; i < numTraps; i++)
            {
                Trap trap = traps.Dequeue();
                trap.Update(enemies);
                traps.Enqueue(trap);
            }
        }

        public void DrawArena(GameTime gameTime, SpriteBatch batch, int arenaOffsetX, int arenaOffsetY, bool paused)
        {
            arenaMap.Draw(gameTime, batch, arenaOffsetX, arenaOffsetY, paused);

            //...draw the traps...
            foreach (Trap t in traps)
            {
                t.Draw(gameTime, batch, arenaOffsetX, arenaOffsetY, paused);
            }

            //...draw the enemies...
            foreach (Enemy e in enemies)
            {
                e.Draw(gameTime, batch, arenaOffsetX, arenaOffsetY, paused);
            }

            //...and their health bars
            foreach (Enemy e in enemies)
            {
                e.DrawHealthBar(gameTime, batch, arenaOffsetX, arenaOffsetY, paused);
            }

            arenaMap.DrawPausedOverlay(gameTime, batch, arenaOffsetX, arenaOffsetY, paused);
        }

        public void spawnEnemy(Enemy e)
        {
            arenaMap.putInSpawn(e);
            enemiesToBeAdded.Enqueue(e);
        }
    }
}
