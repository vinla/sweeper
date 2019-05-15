using System;

namespace Sweeper
{
	[AttributeUsage(AttributeTargets.Method)]
	public class InputActionAttribute : Attribute
	{
		public InputActionAttribute(GameInput input)
		{
			Input = input;
		}

		public GameInput Input { get; }
	}
}
