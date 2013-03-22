using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ParticleBuddy
{
	/// <summary>
	/// This is all the data needed to persist a particle emitter out to XML
	/// </summary>
	public class ParticleXML
	{
		public byte R = 0;
		public byte G = 0;
		public byte B = 0;
		public byte Alpha = 0;
		public float FadeSpeed = 0.0f;
		public Vector2 MaxVelocity = new Vector2(0.0f);
		public Vector2 MinVelocity = new Vector2(0.0f);
		public float ParticleSize;
		public Vector2 Scale = new Vector2(0.0f);
		public Vector2 StartRotation = new Vector2(0.0f);
		public Vector2 Spin = new Vector2(0.0f);
		public int NumStartParticles = 0;
		public float EmitterLife = 0.0f;
		public float ParticleLife = 0.0f;
		public float CreationPeriod = 0.0f;
		public float ParticleGrav = 0.0f;
		public string BmpFileName = "";
	}
}