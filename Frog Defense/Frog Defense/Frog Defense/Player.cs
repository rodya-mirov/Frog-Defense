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

        private const int STARTING_HEALTH = 3;

        private int health;
        public int Health
        {
            get { return health; }
        }

        private EnvironmentUpdater env;

        public Player(EnvironmentUpdater env, int startingMoney = 0, String name = "Commander Badass")
        {
            this.name = name;
            this.money = startingMoney;
            this.env = env;

            this.health = STARTING_HEALTH;
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

        public void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset)
        {
            //Construct the text to draw ...
            String toDraw = Name + "\n     Remaining Cash: $" + Money + "\n     Remaining Health: " + Health;

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
