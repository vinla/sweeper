using Microsoft.Xna.Framework;

namespace Sweeper
{
    public static class IntegerExtensions
	{
		public static int Decrement(this int i, int min, int max)
		{
			int returnValue = i - 1;
			return returnValue < min ? max : returnValue;
		}

		public static int Increment(this int i, int min, int max)
		{
			int returnValue = i + 1;
			return returnValue > max ? min : returnValue;
		}

		public static Point Offset(this Point p, int x, int y)
		{
			return new Point(p.X + x, p.Y + y);
		}

		public static Point Offset(this Point p, Point offset)
		{
			return p.Offset(offset.X, offset.Y);
		}

		public static bool InBouds(this Point p, int minX, int minY, int maxX, int maxY)
		{
			return p.X >= minX && p.X < maxX && p.Y >= minY && p.Y < maxY;
		}

		public static Point Times(this Point p, int multiplier)
		{
			return new Point(p.X * multiplier, p.Y * multiplier);
		}

		public static int DistanceTo(this Point p, Point target)
		{
			var diffX = System.Math.Abs(p.X - target.X);
			var diffY = System.Math.Abs(p.Y - target.Y);
			return diffX + diffY;
		}
	}
}
