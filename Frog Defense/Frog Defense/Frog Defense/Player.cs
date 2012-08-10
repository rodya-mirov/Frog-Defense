using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frog_Defense
{
    /// <summary>
    /// The role of this class is a little nebulous.  Fundamentally, it's a
    /// resource tracker.  But that's just a couple of variables, and they
    /// could really be stored anywhere.  They just needed a home?
    /// </summary>
    class Player
    {
        private String name;
        public String Name
        {
            get { return name; }
        }

        private int money;
        public int Money
        {
            get { return money; }
        }

        private EnvironmentUpdater env;

        public Player(EnvironmentUpdater env, int startingMoney = 0, String name = "Commander Badass")
        {
            this.name = name;
            this.money = startingMoney;
            this.env = env;
        }

        public bool attemptSpend(int cash)
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

        public void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset)
        {
            //Construct the text to draw ...
            String toDraw = Name + "\n     Remaining Cash: $" + Money;

            //Draw that text
            batch.DrawString(
                env.Font,
                toDraw,
                new Vector2(0 + xOffset, 0 + yOffset),
                Color.White
                );
        }
    }
}
