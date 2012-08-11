using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frog_Defense.Traps
{
    class PoisonID
    {
        private static int nextID;

        private int myID;
        public int ID
        {
            get { return myID; }
        }

        /// <summary>
        /// Enemies use the poisonID to determine unique sources of poison.
        /// Make sure to get one here.
        /// </summary>
        /// <returns></returns>
        public static PoisonID GetPoisonID()
        {
            return new PoisonID();
        }

        private PoisonID()
        {
            myID = nextID++;
        }
    }
}
