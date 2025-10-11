
namespace ParticleBuddy
{
	/// <summary>
	/// This is a callback method for getting the rotation to shoot particles.
	/// used to break out dependencies
	/// </summary>
	/// <returns>a method to get an angle in radians.</returns>
	public delegate float RotationDelegate();
}
