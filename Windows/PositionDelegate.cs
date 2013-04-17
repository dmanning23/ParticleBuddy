using Microsoft.Xna.Framework;

namespace ParticleBuddy
{
	/// <summary>
	/// This is a callback method for getting a position
	/// used to break out dependencies
	/// </summary>
	/// <returns>a method to get a position.</returns>
	public delegate Vector2 PositionDelegate();
}
