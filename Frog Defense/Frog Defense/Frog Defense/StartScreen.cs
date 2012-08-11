using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frog_Defense.Menus;
using Microsoft.Xna.Framework.Input;

namespace Frog_Defense
{
    class StartScreen : DrawableGameComponent
    {
        private SpriteBatch batch;
        private Menu menu;

        public StartScreen()
            : base(TDGame.MainGame)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            batch = new SpriteBatch(GraphicsDevice);

            menu = Menu.MakeStartMenu();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Menu.LoadContent();
        }

        private ButtonState wasPressed = ButtonState.Released;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MouseState ms = Mouse.GetState();

            menu.MouseOver(ms.X, ms.Y);

            if (wasPressed == ButtonState.Pressed && ms.LeftButton == ButtonState.Released)
            {
                menu.MouseClick();
            }

            wasPressed = ms.LeftButton;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.Black);

            batch.Begin();

            menu.Draw(gameTime, batch);

            batch.End();
        }
    }
}
