using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Frog_Defense
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TDGame : Game
    {
        //This is a singleton structure and I don't feel like passing
        //around a bazillion references if I don't have to.
        public static TDGame MainGame = null;

        //The main GameUpdater
        private GameUpdater env;

        //The main menu screens
        private MenuScreen startScreen;
        private MenuScreen deathScreen;
        private MenuScreen winScreen;

        //some pure colors so other things can draw basic shapes easily
        public static Texture2D PureRed
        {
            get { return pureRed; }
        }
        private static Texture2D pureRed;
        private const String pureRedPath = "Images/PureColors/Red";

        public static Texture2D PureBlue
        {
            get { return pureBlue; }
        }
        private static Texture2D pureBlue;
        private const String pureBluePath = "Images/PureColors/Blue";

        public static Texture2D PureGreen
        {
            get { return pureGreen; }
        }
        private static Texture2D pureGreen;
        private const String pureGreenPath = "Images/PureColors/Green";

        public static Texture2D PureWhite
        {
            get { return pureWhite; }
        }
        private static Texture2D pureWhite;
        private const String pureWhitePath = "Images/PureColors/White";

        public static Texture2D PureBlack
        {
            get { return pureBlack; }
        }
        private static Texture2D pureBlack;
        private const String pureBlackPath = "Images/PureColors/Black";

        GraphicsDeviceManager graphics;

        //also some fonts
        private static SpriteFont smallFont;
        public static SpriteFont SmallFont
        {
            get { return smallFont; }
        }

        private static SpriteFont mediumFont;
        public static SpriteFont MediumFont
        {
            get { return mediumFont; }
        }

        private static SpriteFont bigFont;
        public static SpriteFont BigFont
        {
            get { return bigFont; }
        }

        private static SpriteFont hugeFont;
        public static SpriteFont HugeFont
        {
            get { return hugeFont; }
        }

        public TDGame()
        {
            MainGame = this;
            this.IsMouseVisible = true;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            env = new GameUpdater();
            Components.Add(env);

            startScreen = MenuScreen.makeStartScreen();
            Components.Add(startScreen);

            deathScreen = MenuScreen.makeDeathScreen();
            Components.Add(deathScreen);

            winScreen = MenuScreen.makeWinScreen();
            Components.Add(winScreen);

            setActiveComponent(startScreen);
        }

        /// <summary>
        /// Goes back to the main menu
        /// </summary>
        public void BackToMainMenu()
        {
            setActiveComponent(startScreen);
        }

        /// <summary>
        /// Resets the current game, then starts it up
        /// </summary>
        public void NewGame()
        {
            env.ResetGame();
            setActiveComponent(env);
        }

        /// <summary>
        /// Start the game up into play mode.
        /// </summary>
        public void StartPlaying()
        {
            setActiveComponent(env);
        }

        /// <summary>
        /// This is what happens when you die!  Your health hits zero,
        /// I mean.  Disables the GameUpdater and goes on to the death screen.
        /// </summary>
        public void Die()
        {
            env.Die();
            setActiveComponent(deathScreen);
        }

        /// <summary>
        /// This is what happens when you win!  Disables the GameUpdater
        /// and goes on to the win screen.
        /// </summary>
        public void Win()
        {
            env.Win();
            setActiveComponent(winScreen);
        }

        /// <summary>
        /// Disables all GameComponents except the parameter, which is
        /// enabled and set to visible.
        /// </summary>
        /// <param name="comp"></param>
        private void setActiveComponent(DrawableGameComponent comp)
        {
            foreach (DrawableGameComponent gc in Components)
            {
                if (gc != comp)
                {
                    gc.Enabled = false;
                    gc.Visible = false;
                }
            }

            comp.Enabled = true;
            comp.Visible = true;
        }

        public bool HasSuspendedGame
        {
            get { return env.HasSuspendedGame; }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.  It also loads the content of all its components (apparently)
        /// automatically, so we don't need to do anything here except load the
        /// textures that everyone uses.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            loadColors();

            loadFonts();

            Console.WriteLine(GraphicsDevice.Viewport.Bounds.ToString());
        }

        private void loadColors()
        {
            if (pureBlack == null)
                pureBlack = Content.Load<Texture2D>(pureBlackPath);

            if (pureWhite == null)
                pureWhite = Content.Load<Texture2D>(pureWhitePath);

            if (pureRed == null)
                pureRed = Content.Load<Texture2D>(pureRedPath);

            if (pureBlue == null)
                pureBlue = Content.Load<Texture2D>(pureBluePath);

            if (pureGreen == null)
                pureGreen = Content.Load<Texture2D>(pureGreenPath);
        }

        public static void loadFonts()
        {
            if (smallFont == null)
                smallFont = TDGame.MainGame.Content.Load<SpriteFont>("Fonts/SmallFont");

            if (mediumFont == null)
                mediumFont = TDGame.MainGame.Content.Load<SpriteFont>("Fonts/MediumFont");

            if (bigFont == null)
                bigFont = TDGame.MainGame.Content.Load<SpriteFont>("Fonts/BigFont");

            if (hugeFont == null)
                hugeFont = TDGame.MainGame.Content.Load<SpriteFont>("Fonts/HugeFont");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
