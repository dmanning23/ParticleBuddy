using FilenameBuddy;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;

namespace ParticleBuddy.Tests
{
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
			var first = new EmitterTemplate()
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
			var first = new EmitterTemplate()
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

			var second = new EmitterTemplate(first);

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
			var first = new EmitterTemplate()
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

			var second = new EmitterTemplate()
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

			second.Compare(first).ShouldBeTrue();
		}

		[Test]
		public void ComparisonTests_Vectors()
		{
			var first = new EmitterTemplate()
			{
				MaxParticleVelocity = new Vector2(1000f, 2000f),
				MinParticleVelocity = new Vector2(3000f, 4000f),
				ParticleColor = Color.Wheat,
			};

			var second = new EmitterTemplate(first);

			first.MaxParticleVelocity.ShouldBe(second.MaxParticleVelocity);
			first.MinParticleVelocity.ShouldBe(second.MinParticleVelocity);
			first.ParticleColor.ShouldBe(second.ParticleColor);
		}

		[Test]
		public void ComparisonTests_Vectors2()
		{
			var first = new EmitterTemplate()
			{
				MaxParticleVelocity = new Vector2(1000f, 2000f),
				MinParticleVelocity = new Vector2(3000f, 4000f),
				ParticleColor = Color.Wheat,
			};

			var second = new EmitterTemplate(first);

			second.Compare(first).ShouldBeTrue();
		}

		#endregion //comparison
	}
}
