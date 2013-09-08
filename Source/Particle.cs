using System.Diagnostics;
using Microsoft.Xna.Framework;
using GameTimer;
using MatrixExtensions;
using RenderBuddy;

namespace ParticleBuddy
{
	public class Particle
	{
		#region Members

		//Position and orientation of the particle.
		public Vector2 Position { get; set; }

		//Velocity of this particle
		private Vector2 m_Velocity;

		//the rotation of the particle
		public float Rotation { get; set; }

		//how fast this particle is spinning, the angle in radians
		public float Spin { get; set; }

		//How long this particle has left to live
		public float Lifespan { get; set; }

		//The size of the square in meters that defines this particle (length of one side)
		public float Size { get; set; }

		//how fast this particle is scaling
		public float Scale { get; set; }

		//The alpha value of this particular particle
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

		public void Render(Renderer myRenderer, Emitter rEmitter)
		{
			//get the upper left/lower right positions
			Vector2 vUpperLeft = new Vector2(Size / -2.0f, Size / -2.0f);

			//create rotation matrix
			Matrix myMatrix = MatrixExt.Orientation(Rotation);
			vUpperLeft = myMatrix.Mutliply(vUpperLeft);

			//get the rotated position
			vUpperLeft = Position + vUpperLeft;

			//get the correct color
			Color myColor = rEmitter.MyColor;
			myColor.A = Alpha;

			//get teh correct rectangle to send to the renderer
			Rectangle myRect = new Rectangle((int)vUpperLeft.X, (int)vUpperLeft.Y, (int)Size, (int)Size);

			myRenderer.Draw(rEmitter.Template.Bitmap, myRect, myColor, Rotation, rEmitter.Flip);
		}

		#endregion //Methods
	}
}