using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frog_Defense.Traps;
using Microsoft.Xna.Framework.Input;
using Frog_Defense.Enemies;
using Frog_Defense.Waves;
using Frog_Defense.Buttons;

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
        private bool paused;
        public bool Paused
        {
            get { return paused; }
            set { paused = value; }
        }

        private PlayerHUD player;
        public PlayerHUD Player
        {
            get { return player; }
        }
        private ButtonPanel buttonPanel;

        private ArenaManager arenaManager;
        public ArenaManager ArenaManager { get { return arenaManager; } }

        private bool shouldResumeGame;
        public bool HasSuspendedGame
        {
            get { return shouldResumeGame; }
        }

        private SpriteBatch batch;

        //All these offsets determine the location of all the panels
        //The plan is: WaveTracker is a horizontal bar across the top
        //Arena, PlayerHUD, and ButtonPanel are horizontally aligned underneath it
        //ButtonPanel hugs the right, Arena hugs the left, and PlayerHUD hugs the arena on its left
        private int ArenaOffsetX
        {
            get
            {
                return 0;
            }
        }
        private int ArenaOffsetY
        {
            get
            {
                return WaveTracker.PixelHeight + WaveTrackerOffsetY;
            }
        }

        private int ArenaPixelWidth
        {
            get { return 450; }
        }

        private int ArenaPixelHeight
        {
            get { return GraphicsDevice.Viewport.Height - ArenaOffsetY; }
        }

        private Rectangle ArenaScissorRectangle
        {
            get
            {
                return new Rectangle(
                    ArenaOffsetX,
                    ArenaOffsetY,
                    ArenaPixelWidth,
                    ArenaPixelHeight
                    );
            }
        }

        private int PlayerOffsetX
        {
            get { return ArenaOffsetX + ArenaPixelWidth + 10; }
        }
        private int PlayerOffsetY
        {
            get { return ArenaOffsetY; }
        }

        private int WaveTrackerOffsetX
        {
            get { return 0; }
        }
        private int WaveTrackerOffsetY
        {
            get { return 0; }
        }

        private int ButtonPanelOffsetX
        {
            get { return Game.GraphicsDevice.Viewport.Width - buttonPanel.PixelWidth - ArenaOffsetX; }
        }
        private int ButtonPanelOffsetY
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

            this.shouldResumeGame = false;
            this.paused = false;

            arenaManager = new ArenaManager(this, ArenaPixelWidth, ArenaScissorRectangle.Height);
            buttonPanel = ButtonPanel.MakeDefaultPanel(this, TDGame.SmallFont);
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            base.OnEnabledChanged(sender, args);

            //if we just got re-enabled
            if (this.Enabled)
            {
                Console.WriteLine("It's GO TIME");

                if (!shouldResumeGame)
                    ResetGame();

                shouldResumeGame = true;
            }
        }

        /// <summary>
        /// Completely starts over the game
        /// </summary>
        public void ResetGame()
        {
            arenaManager.ResetGame();

            player = new PlayerHUD(this);

            shouldResumeGame = false;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            TDGame.loadFonts();
            ArenaManager.LoadContent();
            PlayerHUD.LoadContent();

            rasterizerStateScissor = new RasterizerState() { ScissorTestEnable = true };
            rasterizerStateNoScissor = new RasterizerState() { ScissorTestEnable = false };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            updateMouse();

            if (!TDGame.MainGame.IsActive)
            {
                paused = true;
            }

            arenaManager.Update(gameTime, paused);
        }

        private int mouseX, mouseY;
        private bool mouseIsDown = false;
        private bool mouseWasDown = false;

        /// <summary>
        /// Finds the mouse position (in pixels) and tells the arena about it.
        /// </summary>
        private void updateMouse()
        {
            if (!TDGame.MainGame.IsActive)
                return;

            MouseState ms = Mouse.GetState();

            mouseX = ms.X;
            mouseY = ms.Y;

            //first, tell all the drawable components where the mouse is (in relative coordinates)
            arenaManager.updateMousePosition(mouseX - ArenaOffsetX, mouseY - ArenaOffsetY);
            player.MouseOver(mouseX - PlayerOffsetX, mouseY - PlayerOffsetY);

            //second, deal with all the clicking!
            mouseWasDown = mouseIsDown;
            mouseIsDown = (ms.LeftButton == ButtonState.Pressed);

            if (mouseWasDown && !mouseIsDown)
            {
                arenaManager.GetClicked();
                player.GetClicked();

                int xOffset = ButtonPanelOffsetX;
                int yOffset = ButtonPanelOffsetY;

                buttonPanel.ProcessClick(mouseX - xOffset, mouseY - yOffset);
            }
        }

        private RasterizerState rasterizerStateScissor, rasterizerStateNoScissor;

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);

            //the Arena needs to be drawn separately, because it uses a clipping region
            batch.GraphicsDevice.ScissorRectangle = ArenaScissorRectangle;

            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, rasterizerStateScissor);

            arenaManager.DrawArena(gameTime, batch, ArenaOffsetX, ArenaOffsetY, paused);

            batch.End();

            //everything else is normal
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, rasterizerStateNoScissor);

            player.Draw(gameTime, batch, PlayerOffsetX, PlayerOffsetY);
            arenaManager.WaveTracker.Draw(gameTime, batch, WaveTrackerOffsetX, WaveTrackerOffsetY);
            buttonPanel.Draw(gameTime, batch, ButtonPanelOffsetX, ButtonPanelOffsetY, paused);

            batch.End();
        }

        /// <summary>
        /// Does a little cleanup, but mostly just invalidates the gamestate for resumption.
        /// </summary>
        public void Die()
        {
            shouldResumeGame = false;
        }

        public void Win()
        {
            shouldResumeGame = false;
        }
    }
}
