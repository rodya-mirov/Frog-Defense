using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frog_Defense.PlayerData;

namespace Frog_Defense.Menus
{
    class Menu
    {
        protected List<MenuItem> items;

        protected string mainText;
        protected virtual String MainText { get { return mainText; } }
        protected virtual Color TextColor { get { return Color.White; } }

        protected static SpriteFont smallFont;
        protected static SpriteFont mediumFont;
        protected static SpriteFont bigFont;

        protected virtual SpriteFont MainFont { get { return bigFont; } }

        private Vector2 drawPosition;
        private static Vector2 defaultDrawPosition = new Vector2(100, 100);

        //the number of pixels RIGHT to push menu items (past the title display)
        private const float xOffset = 50;

        //the number of pixels DOWN to put between menu items
        private const float yBuffer = 10;

        protected Menu(String text)
        {
            mainText = text;
            drawPosition = defaultDrawPosition;

            items = new List<MenuItem>();
        }

        public void MouseOver(int mouseX, int mouseY)
        {
            foreach (MenuItem item in items)
            {
                item.MousePosition(mouseX, mouseY);
            }
        }

        public void MouseClick()
        {
            foreach (MenuItem item in items)
            {
                if (item.Active && item.MousedOver)
                    item.GetClicked();
            }
        }

        public static void LoadContent()
        {
            if (smallFont == null)
                smallFont = TDGame.MainGame.Content.Load<SpriteFont>("Fonts/SmallFont");
            if (mediumFont == null)
                mediumFont = TDGame.MainGame.Content.Load<SpriteFont>("Fonts/MediumFont");
            if (bigFont == null)
                bigFont = TDGame.MainGame.Content.Load<SpriteFont>("Fonts/BigFont");
        }

        public static Menu MakeStartMenu()
        {
            Menu output = new Menu("FROG DEFENSE");

            output.addItem(new NewGameButton("New Game", mediumFont));
            output.addItem(new ResumeButton("Resume Game", mediumFont));
            output.addItem(new GoToAchievementsItem("View Achievements", mediumFont));
            output.addItem(new ExitButton("Exit Game", mediumFont));

            return output;
        }

        public static Menu MakeDeathMenu()
        {
            Menu output = new Menu("You have died. Try to avoid that.");

            output.addItem(new StartButton("Back to Main Menu", mediumFont));
            output.addItem(new ExitButton("Exit Game", mediumFont));

            return output;
        }

        public static Menu MakeWinMenu()
        {
            Menu output = new Menu("You win! You're the best winner ever.");

            output.addItem(new StartButton("Back to Main Menu", mediumFont));
            output.addItem(new ExitButton("Exit Game", mediumFont));

            return output;
        }

        public static Menu MakeAchievementMenu(GameUpdater env)
        {
            Menu output = new Menu("Achievements");

            output.drawPosition.X = 30;
            output.drawPosition.Y = 10;

            foreach (Achievements a in Enum.GetValues(typeof(Achievements)))
                output.addItem(new AchievementItem(a, env, smallFont));

            output.addItem(new StartButton("Back to Main Menu", mediumFont));

            return output;
        }

        private void addItem(MenuItem item)
        {
            //calculate its draw position
            float xPos = drawPosition.X + xOffset;
            float yPos;

            if (items.Count == 0)
            {
                yPos = drawPosition.Y + bigFont.MeasureString(mainText).Y + yBuffer;
            }
            else
            {
                yPos = items[items.Count - 1].Bottom + yBuffer;
            }

            item.setPosition(xPos, yPos);

            //add it to the list
            items.Add(item);
        }

        public void Draw(GameTime gameTime, SpriteBatch batch)
        {
            batch.DrawString(MainFont, MainText, drawPosition, TextColor);

            foreach (MenuItem item in items)
            {
                item.Draw(gameTime, batch);
            }
        }
    }
}
