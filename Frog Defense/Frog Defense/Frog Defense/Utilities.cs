using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frog_Defense
{
    struct Point3
    {
        public int x;
        public int y;
        public int z;

        public Point3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    enum Direction
    {
        UP, RIGHT, LEFT, DOWN
    };
}
