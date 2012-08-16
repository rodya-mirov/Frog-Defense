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
        private enum MenuScreenType { Start, Death, Win, Achievements };

        private SpriteBatch batch;
        private Menu menu;
        private MenuScreenType type;
        private GameUpdater env; //not always used

        private MenuScreen(MenuScreenType type)
            : base(TDGame.MainGame)
        {
            this.type = type;
        }

        public static MenuScreen makeDeathScreen()
        {
            return new MenuScreen(MenuScreenType.Death);
        }

        public static MenuScreen makeStartScreen()
        {
            return new MenuScreen(MenuScreenType.Start);
        }

        public static MenuScreen makeWinScreen()
        {
            return new MenuScreen(MenuScreenType.Win);
        }

        public static MenuScreen makeAchievementsScreen(GameUpdater env)
        {
            MenuScreen output;
            
            output = new MenuScreen(MenuScreenType.Achievements);
            output.env = env;

            return output;
        }

        public override void Initialize()
        {
            base.Initialize();

            batch = new SpriteBatch(GraphicsDevice);

            switch (type)
            {
                case MenuScreenType.Death:
                    menu = Menu.MakeDeathMenu();
                    break;

                case MenuScreenType.Start:
                    menu = Menu.MakeStartMenu();
                    break;

                case MenuScreenType.Win:
                    menu = Menu.MakeWinMenu();
                    break;

                case MenuScreenType.Achievements:
                    menu = Menu.MakeAchievementMenu(env);
                    break;

                default:
                    throw new NotImplementedException();
            }
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
