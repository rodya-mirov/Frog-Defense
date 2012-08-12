using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Frog_Defense.Traps;

namespace Frog_Defense
{
    /// <summary>
    /// The role of this class is a little nebulous.  Fundamentally, it's a
    /// resource tracker.  But that's just a couple of variables, and they
    /// could really be stored anywhere.  They just needed a home?
    /// 
    /// But this class also displays the HUD, so maybe the name is a little
    /// wrong.
    /// </summary>
    class PlayerHUD
    {
        private int selectedPreviewIndex;

        private int money;
        public int Money
        {
            get { return money; }
        }

        private const int STARTING_HEALTH = 3;

        private int health;
        public int Health
        {
            get { return health; }
        }

        private GameUpdater env;
        private Arena arena;

        private List<Trap> fixedTraps;

        //standard image stuff
        private const String highlightBorderPath = "Images/TrapPreviews/HighlightedBorder";
        private static Texture2D highlightBorderTexture;

        public PlayerHUD(GameUpdater env, Arena arena, int startingMoney = 0)
        {
            this.money = startingMoney;
            this.env = env;
            this.arena = arena;

            this.health = STARTING_HEALTH;

            previewX = 0;
            previewY = env.MediumFont.MeasureString("A\nA").Y + mainYBuffer; //the height of a 2-line string and a buffer

            setupFixedTraps();
        }

        public static void LoadContent()
        {
            if (highlightBorderTexture == null)
                highlightBorderTexture = TDGame.MainGame.Content.Load<Texture2D>(highlightBorderPath);
        }

        private void setupFixedTraps()
        {
            Random ran = new Random();

            fixedTraps = new List<Trap>();

            fixedTraps.Add(new SpikeTrap(arena, env, -1, -1));
            fixedTraps.Add(new GunTrap(arena, env, -1, -1, Direction.UP));
            fixedTraps.Add(new DartTrap(arena, env, -1, -1, Direction.UP));

            setSelectedTrap(-1);
        }

        /// <summary>
        /// If there is enough money to pay the amount, reduces money by the specified
        /// amount.  Otherwise, NO MONEY IS SPENT (it does not just "spend all it has")
        /// </summary>
        /// <param name="cash"></param>
        /// <returns></returns>
        public bool AttemptSpend(int cash)
        {
            if (cash <= money)
            {
                money -= cash;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Generates income for the player.  No checks are done here, so if there
        /// was somehow "a hit to the wallet" you could put in a negative value that
        /// the player couldn't afford.
        /// </summary>
        /// <param name="cash"></param>
        public void AddMoney(int cash)
        {
            money += cash;
        }

        /// <summary>
        /// What happens when an enemy finds a house!  Health reduced by one.
        /// </summary>
        public void TakeHit()
        {
            health -= 1;
        }

        private int mouseX, mouseY;

        /// <summary>
        /// Update the stored mouse position, given in RELATIVE COORDINATES.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MouseOver(int x, int y)
        {
            mouseX = x;
            mouseY = y;
        }

        public void GetClicked()
        {
            fixHighlight();
        }

        private void fixHighlight()
        {
            //relativize the mouse coordinates a little more
            int relevantX = mouseX - (int)previewX;
            int relevantY = mouseY - (int)previewY;

            //reject obviously wrong values
            if (relevantX < 0 || relevantY < 0)
                return;

            //transform into matrix coordinates
            relevantX /= (int)(Trap.PreviewWidth + xBuffer);
            relevantY /= (int)(Trap.PreviewHeight + yBuffer);

            //reject "too far" clicks
            if (relevantX >= previewsPerRow)
                return;

            //now convert into index form
            int index = relevantX + relevantY * previewsPerRow;

            if (0 <= index && index < fixedTraps.Count)
            {
                setSelectedTrap(index);
            }
        }

        private void setSelectedTrap(int index)
        {
            selectedPreviewIndex = index;

            if (0 <= index && index < fixedTraps.Count)
                arena.SelectedTrapType = fixedTraps[index].trapType;
            else
                arena.SelectedTrapType = TrapType.NoType;

            fixPreviewString();
        }

        private String previewString = "";

        private void fixPreviewString()
        {
            if (0 <= selectedPreviewIndex && selectedPreviewIndex < fixedTraps.Count)
            {
                previewString = fixedTraps[selectedPreviewIndex].ToString();
            }
            else
            {
                previewString = "";
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset)
        {
            //Construct the text to draw ...
            String toDraw = "Cash: $" + Money + "\nHealth: " + Health;

            //Draw that text
            batch.DrawString(
                env.MediumFont,
                toDraw,
                new Vector2(0 + xOffset, 0 + yOffset),
                Color.White
                );

            drawPreviews(gameTime, batch, xOffset, yOffset);
        }

        private int previewsPerRow = 10;

        private float mainYBuffer = 20;

        private float xBuffer = 2;
        private float yBuffer = 2;

        private float previewX;
        private float previewY;

        /// <summary>
        /// Helper method for Draw; it draws the previews and the specific information from the selected trap!
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="batch"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        private void drawPreviews(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset)
        {
            float xPos = previewX + xOffset;
            float yPos = previewY + yOffset;

            int index = 0;
            int xIndex = 0;

            while (index < fixedTraps.Count)
            {
                batch.Draw(fixedTraps[index].PreviewTexture, new Vector2(xPos, yPos), Color.White);

                if (index == selectedPreviewIndex)
                    batch.Draw(highlightBorderTexture, new Vector2(xPos, yPos), Color.White);

                xIndex += 1;

                if (xIndex >= previewsPerRow)
                {
                    xIndex = 0;
                    xPos = previewX + xOffset;
                    yPos += Trap.PreviewHeight + yBuffer;
                }
                else
                {
                    xPos += xBuffer + Trap.PreviewWidth;
                }

                index++;
            }

            xPos = previewX + xOffset;

            yPos += Trap.PreviewHeight + yBuffer + mainYBuffer;

            batch.DrawString(env.SmallFont, previewString, new Vector2(xPos, yPos), Color.White);
        }
    }
}
