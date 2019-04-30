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
	}
}
