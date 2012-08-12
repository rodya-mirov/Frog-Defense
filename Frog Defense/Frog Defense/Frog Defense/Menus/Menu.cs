﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frog_Defense.Menus
{
    class Menu
    {
        private List<MenuItem> items;

        private String mainText;

        private static SpriteFont smallFont;
        private static SpriteFont mediumFont;
        private static SpriteFont bigFont;

        private Vector2 drawPosition = new Vector2(100, 100);

        //the number of pixels RIGHT to push menu items (past the title display)
        private const float xOffset = 50;

        //the number of pixels DOWN to put between menu items
        private const float yBuffer = 10;

        private Menu(String text)
        {
            mainText = text;
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
            output.addItem(new ExitButton("Exit Game", mediumFont));

            return output;
        }

        public static Menu MakeDeathMenu()
        {
            Menu output = new Menu("You have died.  Try to avoid that.");

            output.addItem(new StartButton("Back to Main Menu", mediumFont));
            output.addItem(new ExitButton("Exit Game", mediumFont));

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
            batch.DrawString(bigFont, mainText, drawPosition, Color.White);

            foreach (MenuItem item in items)
            {
                item.Draw(gameTime, batch);
            }
        }
    }
}
