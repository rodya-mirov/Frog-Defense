using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frog_Defense.Traps
{
    abstract class WallTrap : Trap
    {
        protected Direction facing;
        public Direction Facing
        {
            get { return facing; }
        }

        protected int xSquare, ySquare;

        /// <summary>
        /// Constructs a new WallTrap.  The square coordinates should be
        /// the square the WallTrap will actually occupy, not the wall it is
        /// attached to!
        /// </summary>
        /// <param name="env"></param>
        /// <param name="facing"></param>
        /// <param name="xSquare"></param>
        /// <param name="ySquare"></param>
        protected WallTrap(ArenaManager env, Direction facing, int xSquare, int ySquare)
            : base(env)
        {
            this.facing = facing;

            switch (facing)
            {
                case Direction.UP:
                    ySquare++;
                    break;

                case Direction.DOWN:
                    ySquare--;
                    break;

                case Direction.RIGHT:
                    xSquare--;
                    break;

                case Direction.LEFT:
                    xSquare++;
                    break;

                default:
                    throw new NotImplementedException();
            }

            this.xSquare = xSquare;
            this.ySquare = ySquare;
        }

        public override bool touchesWall(int x, int y)
        {
            return xSquare == x && ySquare == y;
        }
    }
}
