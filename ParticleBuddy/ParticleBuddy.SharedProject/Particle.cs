using GameTimer;
using MatrixExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace ParticleBuddy
{
	public class Particle
	{
		#region Members

		/// <summary>
		/// Velocity of this particle
		/// </summary>
		private Vector2 _velocity;

		/// <summary>
		/// The alpha value of this particular particle
		/// </summary>
		private float _alpha;

		#endregion //Members

		#region Properties

		/// <summary>
		/// Position and orientation of the particle.
		/// </summary>
		public Vector2 Position { get; set; }

		/// <summary>
		/// The property converts from float to byte
		/// </summary>
		public byte Alpha
		{
			get
			{
				return (byte)(_alpha * 255.0f);
			}
			set
			{
				_alpha = (float)value / 255.0f;
			}
		}

		public Vector2 Velocity
		{
			get { return _velocity; }
			set { _velocity = value; }
		}

		/// <summary>
		/// the rotation of the particle
		/// </summary>
		public float Rotation { get; set; }

		/// <summary>
		/// how fast this particle is spinning, the angle in radians
		/// </summary>
		public float Spin { get; set; }

		/// <summary>
		/// How long this particle has left to live
		/// </summary>
		public float Lifespan { get; set; }

		/// <summary>
		/// The size of the square in meters that defines this particle (length of one side)
		/// </summary>
		public float Size { get; set; }

		/// <summary>
		/// how fast this particle is scaling
		/// </summary>
		public float Scale { get; set; }

		#endregion //Properties

		#region Methods

		public Particle()
		{
			Position = new Vector2(0.0f);
			Velocity = new Vector2(0.0f);
			Rotation = 0.0f;
			Spin = 0.0f;
			Lifespan = 0.0f;
			Size = 0.0f;
			Scale = 0.0f;
			_alpha = 1.0f;
		}

		public void SetVelocity(float x, float y)
		{
			_velocity.X = x;
			_velocity.Y = y;
		}

		public bool IsDead()
		{
			return ((Lifespan <= 0.0f) || (_alpha <= 0.0f) || (Size <= 0.0f));
		}

		public void Update(GameClock clock, EmitterTemplate template)
		{
			Debug.Assert(clock.TimeDelta >= 0.0f);

			//update the particle time
			Lifespan -= clock.TimeDelta;

			//update the alpha of the particle
			_alpha -= template.FadeSpeed * clock.TimeDelta;

#if DEBUG
			float fOldSize = Size;
#endif

			//update the size of the particle
			Size += Scale * clock.TimeDelta;

#if DEBUG
			if (Scale < 0.0f)
			{
				Debug.Assert(Size <= fOldSize);
			}
			else
			{
				Debug.Assert(Size >= fOldSize);
			}
#endif

			if (IsDead())
			{
				//don't update if this particle is dead
				return;
			}

			//update the position of the particle
			Position += Velocity * clock.TimeDelta;

			//update the rotation of the particle
			Rotation += Spin * clock.TimeDelta;
		
			//update the velocity by adding the gravity
			_velocity.Y += template.ParticleGravity * clock.TimeDelta;
		}

		public void Render(SpriteBatch spriteBatch, Emitter emitter)
		{
			//get the upper left/lower right positions
			Vector2 position = new Vector2(Size / -2.0f, Size / -2.0f);

			//create rotation matrix
			Matrix myMatrix = MatrixExt.Orientation(Rotation);
			position = myMatrix.Multiply(position);

			//get the rotated position
			position = Position + position;

			//get the correct color
			Color color = emitter.Template.ParticleColor;
			if (Color.White != emitter.Color)
			{
				color = emitter.Color;
			}
			color.A = Alpha;

			//get the correct amount to scale the image
			float scale = Size / emitter.Template.Texture.Width;

			spriteBatch.Draw(
				emitter.Template.Texture,
				position,
				null,
				color,
				Rotation,
				Vector2.Zero,
				scale,
				(emitter.Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None),
				0.0f);
		}

		#endregion //Methods
	}
}