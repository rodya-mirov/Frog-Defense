using System;
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

        private const String fontPath = "MainFont";
        private static SpriteFont font;

        private Vector2 drawPosition = new Vector2(100, 100);

        //the number of pixels RIGHT to push menu items (past the title display)
        private const float xOffset = 50;

        //the number of pixels DOWN to put between menu items
        private const float yBuffer = 10;

        private Menu()
        {
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
                if (item.MousedOver)
                    item.GetClicked();
            }
        }

        public static void LoadContent()
        {
            if (font == null)
                font = TDGame.MainGame.Content.Load<SpriteFont>(fontPath);
        }

        public static Menu MakeStartMenu()
        {
            Menu output = new Menu();
            output.mainText = "FROG DEFENSE";

            output.items = new List<MenuItem>();

            output.addItem(new NewGameButton("New Game", font));
            output.addItem(new ExitButton("Exit Game", font));

            return output;
        }



        private void addItem(MenuItem item)
        {
            //calculate its draw position
            float xPos = drawPosition.X + xOffset;
            float yPos;

            if (items.Count == 0)
            {
                yPos = drawPosition.Y + font.MeasureString(mainText).Y + yBuffer;
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
            batch.DrawString(font, mainText, drawPosition, Color.White);

            foreach (MenuItem item in items)
            {
                item.Draw(gameTime, batch);
            }
        }
    }
}
