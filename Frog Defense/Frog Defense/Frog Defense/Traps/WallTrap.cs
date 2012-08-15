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

        protected int wallSquareX, wallSquareY;

        public override TrapLocationType LocationType { get { return TrapLocationType.Wall; } }

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
            : base(env, xSquare, ySquare)
        {
            this.facing = facing;

            this.wallSquareX = xSquare;
            this.wallSquareY = ySquare;

            switch (facing)
            {
                case Direction.UP:
                    this.wallSquareY++;
                    break;

                case Direction.DOWN:
                    this.wallSquareY--;
                    break;

                case Direction.RIGHT:
                    this.wallSquareX--;
                    break;

                case Direction.LEFT:
                    this.wallSquareX++;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public override void shift(int xChange, int yChange, int xSquaresChange, int ySquaresChange)
        {
            base.shift(xChange, yChange, xSquaresChange, ySquaresChange);

            this.wallSquareX += xSquaresChange;
            this.wallSquareY += ySquaresChange;
        }

        public override bool touchesWall(int x, int y)
        {
            return wallSquareX == x && wallSquareY == y;
        }
    }
}
