﻿using System;
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
    class GameUpdater : DrawableGameComponent
    {
        private PlayerHUD player;
        public PlayerHUD Player
        {
            get { return player; }
        }

        private Arena arena;
        private Queue<Enemy> enemies;
        private Queue<Enemy> enemiesToBeAdded;

        private Queue<Trap> traps;
        private Queue<Trap> trapsToBeAdded;

        private SpriteBatch batch;

        private SpriteFont smallFont;
        public SpriteFont SmallFont
        {
            get { return smallFont; }
        }

        private SpriteFont mediumFont;
        public SpriteFont MediumFont
        {
            get { return mediumFont; }
        }

        private SpriteFont bigFont;
        public SpriteFont BigFont
        {
            get { return bigFont; }
        }

        private int ArenaOffsetX
        {
            get
            {
                return 30;
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
        public GameUpdater()
            : base(TDGame.MainGame)
        {
        }



        public override void Initialize()
        {
            base.Initialize();

            batch = new SpriteBatch(TDGame.MainGame.GraphicsDevice);

            resetGame();
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            base.OnEnabledChanged(sender, args);

            //if we just got re-enabled
            if (this.Enabled)
            {
                Console.WriteLine("It's GO TIME");
                resetGame();
            }
        }

        private void resetGame()
        {
            arena = new Arena(this);
            player = new PlayerHUD(this, arena, 500);

            enemies = new Queue<Enemy>();
            enemiesToBeAdded = new Queue<Enemy>();

            traps = new Queue<Trap>();
            trapsToBeAdded = new Queue<Trap>();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            smallFont = TDGame.MainGame.Content.Load<SpriteFont>("SmallFont");
            mediumFont = TDGame.MainGame.Content.Load<SpriteFont>("MediumFont");
            bigFont = TDGame.MainGame.Content.Load<SpriteFont>("BigFont");

            Arena.LoadContent();
            PlayerHUD.LoadContent();

            Enemy.LoadContent();
            BasicEnemy.LoadContent();
            BigBasicEnemy.LoadContent();

            SpikeTrap.LoadContent();
            GunTrap.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            while (trapsToBeAdded.Count > 0)
            {
                traps.Enqueue(trapsToBeAdded.Dequeue());
            }

            while (enemiesToBeAdded.Count > 0)
            {
                enemies.Enqueue(enemiesToBeAdded.Dequeue());
            }

            updateMouse();

            updateEnemies();

            updateTraps();

            arena.Update();
        }

        private int mouseX, mouseY;
        private bool mouseIsDown = false;
        private bool mouseWasDown = false;

        /// <summary>
        /// Finds the mouse position (in pixels) and tells the arena about it.
        /// </summary>
        private void updateMouse()
        {
            MouseState ms = Mouse.GetState();

            mouseX = ms.X;
            mouseY = ms.Y;

            //first, tell all the components where the mouse is (in relative coordinates)
            arena.updateMousePosition(mouseX - ArenaOffsetX, mouseY - ArenaOffsetY);
            player.MouseOver(mouseX - PlayerOffsetX, mouseY - PlayerOffsetY);

            //second, deal with all the clicking!
            mouseWasDown = mouseIsDown;
            mouseIsDown = (ms.LeftButton == ButtonState.Pressed);

            if (mouseWasDown && !mouseIsDown)
            {
                arena.GetClicked();
                player.GetClicked();
            }
        }

        public void addTrap(Trap t)
        {
            traps.Enqueue(t);
        }

        public void addEnemy(Enemy e)
        {
            enemiesToBeAdded.Enqueue(e);
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

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