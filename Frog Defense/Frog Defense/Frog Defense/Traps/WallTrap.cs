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

        protected WallTrap(Arena arena, GameUpdater env, Direction facing)
            : base(arena, env)
        {
            this.facing = facing;
        }
    }
}
