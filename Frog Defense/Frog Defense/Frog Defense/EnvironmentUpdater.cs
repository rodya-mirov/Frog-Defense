using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frog_Defense.Traps;
using Microsoft.Xna.Framework.Input;

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
        private Queue<Trap> traps;

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

            int xOffset = (TDGame.MainGame.GraphicsDevice.Viewport.Width - arena.PixelWidth) / 2;
            int yOffset = (TDGame.MainGame.GraphicsDevice.Viewport.Height - arena.PixelHeight) / 2;

            arena.updateMousePosition(mouseX - xOffset, mouseY - yOffset);

            mouseWasClicked = mouseClicked;
            mouseClicked = (ms.LeftButton == ButtonState.Pressed);

            if (mouseWasClicked && !mouseClicked)
            {
                Trap t = arena.addTrapAtMousePosition();
                if (t != null)
                    traps.Enqueue(t);
            }
        }

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

        private void updateEnemies()
        {
            int numEnemies = enemies.Count;

            for (int i = 0; i < numEnemies; i++)
            {
                Enemy enemy = enemies.Dequeue();

                enemy.Update();

                if (enemy.IsAlive())
                    enemies.Enqueue(enemy);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            int xOffset = (TDGame.MainGame.GraphicsDevice.Viewport.Width - arena.PixelWidth) / 2;
            int yOffset = (TDGame.MainGame.GraphicsDevice.Viewport.Height - arena.PixelHeight) / 2;

            batch.Begin();

            arena.Draw(gameTime, batch, xOffset, yOffset);

            int numTraps = traps.Count;
            for (int i = 0; i < numTraps; i++)
            {
                Trap trap = traps.Dequeue();
                trap.Draw(gameTime, batch, xOffset, yOffset);
                traps.Enqueue(trap);
            }

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
