using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ParticleBuddy
{
	public class Emitter
	{
		#region Members

		//Stopwatch used to time particle creation
		private CGameClock m_CreationTimer;

		//Stopwatch used to time the life of this emitter
		private CEggTimer m_EmitterTimer;

		//This emitters base object, used to attach emitters to base objects
		private CBone m_rAttachedBone;

		//The emitter stuff for this emitter, points to the array of emitter... stuffs
		private EmitterTemplate m_rTemplate;

		//the direction that particles are shot at
		private Vector2 m_Velocity;

		//the position of this emitter
		private Vector2 m_Position;

		private Vector2 m_Offset;

		//The color of this emitter
		private Color m_Color;

		//the list of particles
		private Queue<Particle> m_listParticles;

		//list of dead particles
		static private Queue<Particle> g_Warehouse;
		
		/// <summary>
		/// whether or not the particle emitter is flipped
		/// </summary>
		private bool m_bFlip;

		#endregion //Members

		#region Properties

		public Color MyColor
		{
			get { return m_Color; }
		}

		public EmitterTemplate Template
		{
			get { return m_rTemplate; }
		}

		public CEggTimer EmitterTimer
		{
			get { return m_EmitterTimer; }
		}

		public bool Flip
		{
			get { return m_bFlip; }
		}

		#endregion //Properties

		#region Methods

		static Emitter()
		{
			g_Warehouse = new Queue<Particle>();
		}

		public Emitter(EmitterTemplate rTemplate, Vector2 Velocity, Vector2 Position, Vector2 Offset, CBone rBone, Color myColor, bool bFlip, float fScale)
		{
			m_rTemplate = rTemplate;
			m_Velocity = Velocity;
			m_Position = Position;
			m_Offset = Offset;
			m_rAttachedBone = rBone;
			if (null != m_rAttachedBone)
			{
				m_Position = m_rAttachedBone.AnchorPosition;
			}

			//if the emitter is flipped, siwtch the offset
			if (bFlip)
			{
				m_Offset.X = -m_Offset.X;
			}

			//add the offset to the position
			m_Position += (m_Offset * fScale);

			m_Color = myColor;
			m_bFlip = bFlip;

			m_CreationTimer = new CGameClock();
			m_EmitterTimer = new CEggTimer();
			m_listParticles = new Queue<Particle>();

			//start the timer
			m_EmitterTimer.Start(rTemplate.EmitterLife);
			m_CreationTimer.Start();

			//create the correct number of start particles
			for (int i = 0; i < m_rTemplate.NumStartParticles; i++)
			{
				AddParticle();
			}
		}

		protected void AddParticle()
		{
			Particle myParticle;
			if (0 != g_Warehouse.Count)
			{
				myParticle = g_Warehouse.Dequeue();
			}
			else
			{
				myParticle = new Particle();
			}

			//set position and all the template particle parameters
			myParticle.Position = m_Position;

			//set all the random stuff for particle
			m_rTemplate.SetParticle(myParticle);
			myParticle.Velocity += this.m_Velocity;

			//is the emitter flipped?
			if (m_bFlip)
			{
				myParticle.Spin *= -1.0f;
				myParticle.VelocityX *= -1.0f;
				//myParticle.Rotation = Helper.ClampAngle(myParticle.Rotation);
				//myParticle.Rotation = MathHelper.Pi - myParticle.Rotation;
			}

			m_listParticles.Enqueue(myParticle);
		}

		public void Update(CGameClock myClock, float fScale)
		{
			//update the emitter clock
			m_EmitterTimer.Update(myClock);
			m_CreationTimer.Update(myClock);

			//update position from attached bone?
			if (null != m_rAttachedBone)
			{
				m_Position = m_rAttachedBone.AnchorPosition;
				m_Position += (m_Offset * fScale);
			}

			//update all the particles
			Queue<Particle>.Enumerator iter = m_listParticles.GetEnumerator();
			while (iter.MoveNext())
			{
				iter.Current.Update(myClock, m_rTemplate);
			}

			//do any particles need to be removed?
			while ((m_listParticles.Count > 0) && m_listParticles.Peek().IsDead())
			{
				g_Warehouse.Enqueue(m_listParticles.Dequeue());
			}

			//dont add any particles if the emitter is expired
			if (0.0f < m_EmitterTimer.RemainingTime())
			{
				//do any particles need to be added?
				while (m_CreationTimer.CurrentTime >= m_rTemplate.CreationPeriod)
				{
					AddParticle();
					m_CreationTimer.SubtractTime(m_rTemplate.CreationPeriod);
				}
			}
		}

		public void Render(IRenderer myRenderer)
		{
			//draw all the particles
			Queue<Particle>.Enumerator iter = m_listParticles.GetEnumerator();
			while (iter.MoveNext())
			{
				if (!iter.Current.IsDead())
				{
					iter.Current.Render(myRenderer, this);
				}
			}
		}

		public void Flush()
		{
			//dump all the particle objects
			while ((m_listParticles.Count > 0) && (g_Warehouse.Count < 1000))
			{
				g_Warehouse.Enqueue(m_listParticles.Dequeue());
			}

			if (m_listParticles.Count > 0)
			{
				m_listParticles.Clear();
			}
		}

		public bool IsDead()
		{
			return ((0.0f >= m_EmitterTimer.RemainingTime()) && (0 >= m_listParticles.Count));
		}

		#endregion //Methods
	}
}