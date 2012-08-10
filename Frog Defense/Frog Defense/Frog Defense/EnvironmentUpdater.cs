using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frog_Defense.Traps;
using Microsoft.Xna.Framework.Input;
using Frog_Defense.Enemies;

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
        private Player player;

        private Arena arena;
        private Queue<Enemy> enemies;
        private Queue<Trap> traps;

        private SpriteBatch batch;

        private SpriteFont font;
        public SpriteFont Font
        {
            get { return font; }
        }

        private int ArenaOffsetX
        {
            get
            {
                return ArenaOffsetY;
            }
        }
        private int ArenaOffsetY
        {
            get
            {
                return (TDGame.MainGame.GraphicsDevice.Viewport.Height - arena.PixelHeight) / 2;
            }
        }

        private int PlayerOffsetX
        {
            get { return ArenaOffsetX * 2 + arena.PixelWidth; }
        }
        private int PlayerOffsetY
        {
            get { return ArenaOffsetY; }
        }

        /// <summary>
        /// The only constructor for EnvironmentUpdater
        /// </summary>
        public EnvironmentUpdater()
            : base(TDGame.MainGame)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            batch = new SpriteBatch(TDGame.MainGame.GraphicsDevice);
            font = TDGame.MainGame.Content.Load<SpriteFont>("MainFont");

            player = new Player(this, 500);
            arena = new Arena(this);

            enemies = new Queue<Enemy>();
            traps = new Queue<Trap>();

            foreach (Trap trap in arena.makeTraps())
            {
                traps.Enqueue(trap);
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Arena.LoadContent();

            Enemy.LoadContent();
            BasicEnemy.LoadContent();

            SpikeTrap.LoadContent();
            GunTrap.LoadContent();
        }

        private int framesBetweenSpawns = 45;
        private int framesSinceSpawn = 0;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            updateMouse();

            updateEnemies();

            updateTraps();

            arena.Update();

            if (framesSinceSpawn < framesBetweenSpawns)
                framesSinceSpawn++;

            else if (enemies.Count < 20)
            {
                enemies.Enqueue(arena.makeEnemy());
                framesSinceSpawn = 0;
            }
        }

        private int mouseX, mouseY;
        private bool mouseClicked = false;
        private bool mouseWasClicked = false;

        /// <summary>
        /// Finds the mouse position (in pixels) and tells the arena about it.
        /// </summary>
        private void updateMouse()
        {
            MouseState ms = Mouse.GetState();

            mouseX = ms.X;
            mouseY = ms.Y;

            arena.updateMousePosition(mouseX - ArenaOffsetX, mouseY - ArenaOffsetY);

            mouseWasClicked = mouseClicked;
            mouseClicked = (ms.LeftButton == ButtonState.Pressed);

            if (mouseWasClicked && !mouseClicked)
            {
                Trap t = arena.addTrapAtMousePosition(player);
                if (t != null)
                    traps.Enqueue(t);
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
                    player.TakeHit();
                }
                else if (enemy.IsAlive)
                {
                    enemies.Enqueue(enemy);
                }
                else
                {
                    player.AddMoney(enemy.CashValue);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            batch.Begin();

            drawArenaAndRelated(gameTime);

            player.Draw(gameTime, batch, PlayerOffsetX, PlayerOffsetY);

            batch.End();
        }

        /// <summary>
        /// Helper method for Draw; this draws the arena and everything in it (enemies, traps, etc.)
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="arenaOffsetX"></param>
        /// <param name="arenaOffsetY"></param>
        private void drawArenaAndRelated(GameTime gameTime)
        {
            int arenaOffsetX = ArenaOffsetX;
            int arenaOffsetY = ArenaOffsetY;

            //Draw the arena and its walls...
            arena.Draw(gameTime, batch, arenaOffsetX, arenaOffsetY);

            //...draw the traps...
            foreach (Trap t in traps)
            {
                t.Draw(gameTime, batch, arenaOffsetX, arenaOffsetY);
            }

            //...draw the enemies...
            foreach (Enemy e in enemies)
            {
                e.Draw(gameTime, batch, arenaOffsetX, arenaOffsetY);
            }

            //...and their health bars
            foreach (Enemy e in enemies)
            {
                e.DrawHealthBar(gameTime, batch, arenaOffsetX, arenaOffsetY);
            }
        }
    }
}
