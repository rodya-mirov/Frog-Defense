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
    class MenuScreen : DrawableGameComponent
    {
        private SpriteBatch batch;
        private Menu menu;

        private MenuScreen(Menu menu)
            : base(TDGame.MainGame)
        {
            this.menu = menu;
        }

        public static MenuScreen makeDeathScreen()
        {
            return new MenuScreen(Menu.MakeDeathMenu());
        }

        public static MenuScreen makeStartScreen()
        {
            return new MenuScreen(Menu.MakeStartMenu());
        }

        public override void Initialize()
        {
            base.Initialize();

            batch = new SpriteBatch(GraphicsDevice);
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

            updateMouse();
        }

        private void updateMouse()
        {
            if (!TDGame.MainGame.IsActive)
                return;

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
