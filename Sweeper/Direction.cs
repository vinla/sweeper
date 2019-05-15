using Microsoft.Xna.Framework;

namespace Sweeper
{
	public static class Direction
    {
        public static Point Up => new Point(0, -1);

        public static Point Down => new Point(0, 1);

        public static Point Left => new Point(-1, 0);

        public static Point Right => new Point(1, 0);

        public static Point[] CompassPoints => new[] { Up, Right, Down, Left };
    }    
}
