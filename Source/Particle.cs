using GameTimer;
using MatrixExtensions;
using Microsoft.Xna.Framework;
using RenderBuddy;
using System.Diagnostics;

namespace ParticleBuddy
{
	public class Particle
	{
		#region Members

		/// <summary>
		/// Position and orientation of the particle.
		/// </summary>
		public Vector2 Position { get; set; }

		/// <summary>
		/// Velocity of this particle
		/// </summary>
		private Vector2 m_Velocity;

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

		/// <summary>
		/// The alpha value of this particular particle
		/// </summary>
		private float m_fAlpha;

		#endregion //Members

		#region Properties

		/// <summary>
		/// The property converts from float to byte
		/// </summary>
		public byte Alpha
		{
			get
			{
				return (byte)(m_fAlpha * 255.0f);
			}
			set
			{
				m_fAlpha = (float)value / 255.0f;
			}
		}

		public Vector2 Velocity
		{
			get { return m_Velocity; }
			set { m_Velocity = value; }
		}

		public float VelocityX
		{
			get { return m_Velocity.X; }
			set { m_Velocity.X = value; }
		}

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
			m_fAlpha = 1.0f;
		}

		public void SetVelocity(float x, float y)
		{
			m_Velocity.X = x;
			m_Velocity.Y = y;
		}

		public bool IsDead()
		{
			return ((Lifespan <= 0.0f) || (m_fAlpha <= 0.0f) || (Size <= 0.0f));
		}

		public void Update(GameClock myClock, EmitterTemplate rTemplate)
		{
			Debug.Assert(myClock.TimeDelta >= 0.0f);

			//update the particle time
			Lifespan -= myClock.TimeDelta;

			//update the alpha of the particle
			m_fAlpha -= rTemplate.FadeSpeed * myClock.TimeDelta;

#if DEBUG
			float fOldSize = Size;
#endif

			//update the size of the particle
			Size += Scale * myClock.TimeDelta;

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
			Position += Velocity * myClock.TimeDelta;

			//update the rotation of the particle
			Rotation += Spin * myClock.TimeDelta;
		
			//update the velocity by adding the gravity
			m_Velocity.Y += rTemplate.ParticleGravity * myClock.TimeDelta;
		}

		public void Render(IRenderer myRenderer, Emitter rEmitter)
		{
			//get the upper left/lower right positions
			Vector2 vUpperLeft = new Vector2(Size / -2.0f, Size / -2.0f);

			//create rotation matrix
			Matrix myMatrix = MatrixExt.Orientation(Rotation);
			vUpperLeft = myMatrix.Multiply(vUpperLeft);

			//get the rotated position
			vUpperLeft = Position + vUpperLeft;

			//get the correct color
			Color myColor = rEmitter.Template.ParticleColor;
			if (Color.White != rEmitter.MyColor)
			{
				myColor = rEmitter.MyColor;
			}
			myColor.A = Alpha;

			//get the correct amount to scale the image
			float scale = Size / rEmitter.Template.Bitmap.Width;

			myRenderer.Draw(rEmitter.Template.Bitmap, vUpperLeft, myColor, Color.White, Rotation, rEmitter.Flip, scale);
		}

		#endregion //Methods
	}
}