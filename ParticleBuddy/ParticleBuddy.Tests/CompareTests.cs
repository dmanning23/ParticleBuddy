using FilenameBuddy;
using MathNet.Numerics;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using Shouldly;
using System.Diagnostics;

namespace ParticleBuddy.Tests
{
	public class TestEmitterTemplate : EmitterTemplate
	{
		public TestEmitterTemplate()
		{
		}

		public TestEmitterTemplate(EmitterTemplate inst) : base(inst)
		{
		}

		public void Compare(TestEmitterTemplate inst)
		{
			Debug.Assert(FadeSpeed == inst.FadeSpeed);
			Debug.Assert(MaxParticleVelocity.X.AlmostEqual(inst.MaxParticleVelocity.X));
			Debug.Assert(MaxParticleVelocity.Y.AlmostEqual(inst.MaxParticleVelocity.Y));
			Debug.Assert(MinParticleVelocity.X.AlmostEqual(inst.MinParticleVelocity.X));
			Debug.Assert(MinParticleVelocity.Y.AlmostEqual(inst.MinParticleVelocity.Y));
			Debug.Assert(ParticleSize == inst.ParticleSize);
			Debug.Assert(NumStartParticles == inst.NumStartParticles);
			Debug.Assert(EmitterLife == inst.EmitterLife);
			Debug.Assert(Expires == inst.Expires);
			Debug.Assert(ParticleLife == inst.ParticleLife);
			Debug.Assert(CreationPeriod == inst.CreationPeriod);
			Debug.Assert(ParticleGravity == inst.ParticleGravity);
			Debug.Assert(ParticleColor == inst.ParticleColor);
			Debug.Assert(Spin.X.AlmostEqual(inst.Spin.X));
			Debug.Assert(Spin.Y.AlmostEqual(inst.Spin.Y));
			Debug.Assert(Scale.X.AlmostEqual(inst.Scale.X));
			Debug.Assert(Scale.Y.AlmostEqual(inst.Scale.Y));
			Debug.Assert(StartRotation.X.AlmostEqual(inst.StartRotation.X));
			Debug.Assert(StartRotation.Y.AlmostEqual(inst.StartRotation.Y));
		}
	}

	public class CompareTests
	{
		#region construction

		[TestCase(1f, 2f, 3, 4f, true, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, "catpants.jpg")]
		public void ConstructionTests(float fadeSpeed,
			float particleSize,
			int numStartParticles,
			float emitterLife,
			bool expires,
			float particleLife,
			float creationPeriod,
			float particleGravity,
			float minSpin,
			float maxSpin,
			float minScale,
			float maxScale,
			float minStartRotation,
			float maxStartRotation,
			string imageFile)
		{
			var first = new TestEmitterTemplate()
			{
				FadeSpeed = fadeSpeed,
				ParticleSize = particleSize,
				NumStartParticles = numStartParticles,
				EmitterLife = emitterLife,
				Expires = expires,
				ParticleLife = particleLife,
				CreationPeriod = creationPeriod,
				ParticleGravity = particleGravity,
				MinSpin = minSpin,
				MaxSpin = maxSpin,
				MinScale = minScale,
				MaxScale = maxScale,
				MinStartRotation = minStartRotation,
				MaxStartRotation = maxStartRotation,
				ImageFile = new Filename(imageFile)
			};

			first.FadeSpeed.ShouldBe(fadeSpeed);
			first.ParticleSize.ShouldBe(particleSize);
			first.NumStartParticles.ShouldBe(numStartParticles);
			first.EmitterLife.ShouldBe(emitterLife);
			first.Expires.ShouldBe(expires);
			first.ParticleLife.ShouldBe(particleLife);
			first.CreationPeriod.ShouldBe(creationPeriod);
			first.ParticleGravity.ShouldBe(particleGravity);
			first.MinSpin.ShouldBe(minSpin);
			first.MaxSpin.ShouldBe(maxSpin);
			first.MinScale.ShouldBe(minScale);
			first.MaxScale.ShouldBe(maxScale);
			first.MinStartRotation.ShouldBe(minStartRotation);
			first.MaxStartRotation.ShouldBe(maxStartRotation);
			first.ImageFile.File.ShouldBe(new Filename(imageFile).File);
		}

		[TestCase(1f, 2f, 3, 4f, true, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, "catpants.jpg")]
		public void CopyConstructionTests(float fadeSpeed,
			float particleSize,
			int numStartParticles,
			float emitterLife,
			bool expires,
			float particleLife,
			float creationPeriod,
			float particleGravity,
			float minSpin,
			float maxSpin,
			float minScale,
			float maxScale,
			float minStartRotation,
			float maxStartRotation,
			string imageFile)
		{
			var first = new TestEmitterTemplate()
			{
				FadeSpeed = fadeSpeed,
				ParticleSize = particleSize,
				NumStartParticles = numStartParticles,
				EmitterLife = emitterLife,
				Expires = expires,
				ParticleLife = particleLife,
				CreationPeriod = creationPeriod,
				ParticleGravity = particleGravity,
				MinSpin = minSpin,
				MaxSpin = maxSpin,
				MinScale = minScale,
				MaxScale = maxScale,
				MinStartRotation = minStartRotation,
				MaxStartRotation = maxStartRotation,
				ImageFile = new Filename(imageFile)
			};

			var second = new TestEmitterTemplate(first);

			second.FadeSpeed.ShouldBe(fadeSpeed);
			second.ParticleSize.ShouldBe(particleSize);
			second.NumStartParticles.ShouldBe(numStartParticles);
			second.EmitterLife.ShouldBe(emitterLife);
			second.Expires.ShouldBe(expires);
			second.ParticleLife.ShouldBe(particleLife);
			second.CreationPeriod.ShouldBe(creationPeriod);
			second.ParticleGravity.ShouldBe(particleGravity);
			second.MinSpin.ShouldBe(minSpin);
			second.MaxSpin.ShouldBe(maxSpin);
			second.MinScale.ShouldBe(minScale);
			second.MaxScale.ShouldBe(maxScale);
			second.MinStartRotation.ShouldBe(minStartRotation);
			second.MaxStartRotation.ShouldBe(maxStartRotation);
			second.ImageFile.File.ShouldBe(new Filename(imageFile).File);
		}

		#endregion //construction

		#region comparison

		[TestCase(1f, 2f, 3, 4f, true, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, "catpants.jpg")]
		public void ComparisonTests(float fadeSpeed,
			float particleSize,
			int numStartParticles,
			float emitterLife,
			bool expires,
			float particleLife,
			float creationPeriod,
			float particleGravity,
			float minSpin,
			float maxSpin,
			float minScale,
			float maxScale,
			float minStartRotation,
			float maxStartRotation,
			string imageFile)
		{
			var first = new TestEmitterTemplate()
			{
				FadeSpeed = fadeSpeed,
				ParticleSize = particleSize,
				NumStartParticles = numStartParticles,
				EmitterLife = emitterLife,
				Expires = expires,
				ParticleLife = particleLife,
				CreationPeriod = creationPeriod,
				ParticleGravity = particleGravity,
				MinSpin = minSpin,
				MaxSpin = maxSpin,
				MinScale = minScale,
				MaxScale = maxScale,
				MinStartRotation = minStartRotation,
				MaxStartRotation = maxStartRotation,
				ImageFile = new Filename(imageFile)
			};

			var second = new TestEmitterTemplate()
			{
				FadeSpeed = fadeSpeed,
				ParticleSize = particleSize,
				NumStartParticles = numStartParticles,
				EmitterLife = emitterLife,
				Expires = expires,
				ParticleLife = particleLife,
				CreationPeriod = creationPeriod,
				ParticleGravity = particleGravity,
				MinSpin = minSpin,
				MaxSpin = maxSpin,
				MinScale = minScale,
				MaxScale = maxScale,
				MinStartRotation = minStartRotation,
				MaxStartRotation = maxStartRotation,
				ImageFile = new Filename(imageFile)
			};

			second.Compare(first);
		}

		[Test]
		public void ComparisonTests_Vectors()
		{
			var first = new TestEmitterTemplate()
			{
				MaxParticleVelocity = new Vector2(1000f, 2000f),
				MinParticleVelocity = new Vector2(3000f, 4000f),
				ParticleColor = Color.Wheat,
			};

			var second = new TestEmitterTemplate(first);

			first.MaxParticleVelocity.ShouldBe(second.MaxParticleVelocity);
			first.MinParticleVelocity.ShouldBe(second.MinParticleVelocity);
			first.ParticleColor.ShouldBe(second.ParticleColor);
		}

		[Test]
		public void ComparisonTests_Vectors2()
		{
			var first = new TestEmitterTemplate()
			{
				MaxParticleVelocity = new Vector2(1000f, 2000f),
				MinParticleVelocity = new Vector2(3000f, 4000f),
				ParticleColor = Color.Wheat,
			};

			var second = new TestEmitterTemplate(first);

			second.Compare(first);
		}

		#endregion //comparison
	}
}
