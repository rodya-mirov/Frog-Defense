﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Frog_Defense.Traps;
using Frog_Defense.Enemies;

namespace Frog_Defense
{
    enum DetailViewType { EnemyExisting, EnemyPreview, TrapExisting, TrapPreview, None };


    /// <summary>
    /// The role of this class is a little nebulous.  Fundamentally, it's a
    /// resource tracker.  But it's also a HUD!  There was just no reason
    /// to create a subobject just to track the player's money (there
    /// was just nothing else going on!)
    /// </summary>
    class PlayerHUD
    {
        private Enemy selectedEnemy;
        private Trap selectedTrap;

        private Enemy DetailEnemy
        {
            get { return selectedEnemy; }
            set
            {
                if (selectedEnemy != null)
                    selectedEnemy.Highlighted = false;

                if (value != null)
                    value.Highlighted = true;

                selectedEnemy = value;
            }
        }
        private Trap DetailTrap
        {
            get { return selectedTrap; }
            set
            {
                if (selectedTrap != null)
                    selectedTrap.Highlighted = false;

                if (value != null)
                    value.Highlighted = true;

                selectedTrap = value;
            }
        }

        private DetailViewType detailViewType;
        public DetailViewType DetailViewType
        {
            get { return detailViewType; }
            set
            {
                if (detailViewType == value)
                    return;

                detailViewType = value;

                switch (value)
                {
                    case DetailViewType.EnemyExisting:
                        DetailEnemy = env.ArenaManager.DetailEnemy;
                        DetailTrap = null;
                        setSelectedTrap(-1);
                        break;

                    case DetailViewType.EnemyPreview:
                        DetailEnemy = env.ArenaManager.WaveTracker.DetailEnemy;
                        DetailTrap = null;
                        setSelectedTrap(-1);
                        break;

                    case DetailViewType.TrapExisting:
                        DetailEnemy = null;
                        DetailTrap = env.ArenaManager.DetailTrap;
                        setSelectedTrap(-1);
                        break;

                    case DetailViewType.TrapPreview:
                        DetailEnemy = null;
                        DetailTrap = null;
                        break;

                    case DetailViewType.None:
                        DetailEnemy = null;
                        DetailTrap = null;
                        setSelectedTrap(-1);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

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

        private List<Trap> fixedTraps;

        //standard image stuff
        private const String highlightBorderPath = "Images/TrapPreviews/HighlightedBorder";
        private static Texture2D highlightBorderTexture;

        public PlayerHUD(GameUpdater env, int startingMoney = 1000)
        {
            this.money = startingMoney;
            this.env = env;

            this.health = STARTING_HEALTH;

            DetailViewType = DetailViewType.TrapPreview;

            previewX = 0;
            previewY = TDGame.MediumFont.MeasureString("A\nA\nA").Y + mainYBuffer; //the height of a 3-line string and a buffer

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

            fixedTraps.Add(new MineWall(env.ArenaManager, -1, -1, -1, -1));
            fixedTraps.Add(new DigPit(env.ArenaManager, -1, -1, -1, -1));
            fixedTraps.Add(new Build(env.ArenaManager, -1, -1, -1, -1));

            //basically, make a newline in the previews
            while (fixedTraps.Count % previewsPerRow != 0)
                fixedTraps.Add(null);

            fixedTraps.Add(new SpikeTrap(env.ArenaManager, -1, -1, -1, -1));

            fixedTraps.Add(new GunTrap(env.ArenaManager, -1, -1, -1, -1, Direction.UP));
            fixedTraps.Add(new CannonTrap(env.ArenaManager, -1, -1, -1, -1, Direction.UP));
            fixedTraps.Add(new DartTrap(env.ArenaManager, -1, -1, -1, -1, Direction.UP));

            setSelectedTrap(-1);
        }

        /// <summary>
        /// Spends some money (can go negative, check first)
        /// </summary>
        /// <param name="cash"></param>
        /// <returns></returns>
        public void Spend(int cash)
        {
            money -= cash;

            env.AchievementTracker.ReportPlayerMoney(money);
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

            env.AchievementTracker.ReportPlayerMoney(money);
        }

        /// <summary>
        /// What happens when an enemy finds a house!  Health reduced by one.
        /// </summary>
        public void TakePlayerHit()
        {
            health -= 1;

            if (health <= 0)
                TDGame.MainGame.Die();
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
            
            //check if the index is valid and not pointing to an empty "spacer" trap
            if (0 <= index && index < fixedTraps.Count && fixedTraps[index] != null)
            {
                setSelectedTrap(index);
                DetailViewType = DetailViewType.TrapPreview;
            }
        }

        private void setSelectedTrap(int index)
        {
            selectedPreviewIndex = index;

            if (0 <= index && index < fixedTraps.Count)
                env.ArenaManager.SelectedTrapType = fixedTraps[index].trapType;
            else
                env.ArenaManager.SelectedTrapType = TrapType.NoType;

            fixPreviewString();
        }

        private String previewString = "";

        private void fixPreviewString()
        {
            if (0 <= selectedPreviewIndex && selectedPreviewIndex < fixedTraps.Count)
            {
                previewString = fixedTraps[selectedPreviewIndex].BuyString();
            }
            else
            {
                previewString = "";
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset)
        {
            //Construct the text to draw ...
            String toDraw = "Cash: $" + Money + "\nHealth: " + Health + "\nRemaining: " + env.ArenaManager.WaveTracker.WavesRemaining;

            //Draw that text
            batch.DrawString(
                TDGame.SmallFont,
                toDraw,
                new Vector2(0 + xOffset, 0 + yOffset),
                Color.White
                );

            drawPreviews(gameTime, batch, xOffset, yOffset);
        }

        private const int previewsPerRow = 7;

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
                if (fixedTraps[index] != null)
                {
                    batch.Draw(fixedTraps[index].PreviewTexture, new Vector2(xPos, yPos), Color.White);
                }

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

            drawTrapInformation(batch, xPos, yPos);
        }

        private void drawTrapInformation(SpriteBatch batch, float xPos, float yPos)
        {
            Enemy e;
            Trap t;

            switch(DetailViewType)
            {
                case DetailViewType.TrapPreview:
                    batch.DrawString(TDGame.SmallFont,
                        previewString,
                        new Vector2(xPos, yPos),
                        Color.White);
                    break;

                case DetailViewType.EnemyPreview:
                    e = env.ArenaManager.WaveTracker.DetailEnemy;
                    if (e != null && e.IsAlive)
                    {
                        batch.DrawString(TDGame.SmallFont,
                            e.ToString(),
                            new Vector2(xPos, yPos),
                            Color.White
                            );
                    }
                    else
                    {
                        DetailViewType = DetailViewType.None;
                    }
                    break;

                case DetailViewType.TrapExisting:
                    t = env.ArenaManager.DetailTrap;

                    if (t != null)
                    {
                        batch.DrawString(TDGame.SmallFont,
                            t.SelfString(),
                            new Vector2(xPos, yPos),
                            Color.White
                            );
                    }
                    else
                    {
                        DetailViewType = DetailViewType.None;
                    }
                    break;

                case DetailViewType.EnemyExisting:
                    e = env.ArenaManager.DetailEnemy;
                    if (e != null && e.IsAlive)
                    {
                        batch.DrawString(TDGame.SmallFont,
                            e.ToString(),
                            new Vector2(xPos, yPos),
                            Color.White
                            );
                    }
                    else
                    {
                        DetailViewType = DetailViewType.None;
                    }
                    break;

                case DetailViewType.None:
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
