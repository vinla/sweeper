using Microsoft.Xna.Framework;

namespace Sweeper
{
	public class Spirit
	{
		public Spirit(int x, int y)
		{
			Location = new Point(x, y);
		}

		public Point Location { get; set; }
	}	
}
