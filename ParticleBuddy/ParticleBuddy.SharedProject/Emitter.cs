using GameTimer;
using MatrixExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ParticleBuddy
{
	public class Emitter
	{
		#region Members

		/// <summary>
		/// A callback method to get the position of this emitter.
		/// null to figure out our own dang position
		/// </summary>
		private readonly PositionDelegate _positionDelegate;

		/// <summary>
		/// A callback method to get the rotation of new particles.
		/// null to use the rotation from the emitter template.
		/// </summary>
		private readonly RotationDelegate _rotationDelegate;

		/// <summary>
		/// the direction that particles are shot at
		/// </summary>
		private Vector2 _velocity;

		/// <summary>
		/// the position of this emitter
		/// </summary>
		private Vector2 _position;

		/// <summary>
		/// A callback method to rotate the offset.  Should be owner rotation
		/// </summary>
		private readonly RotationDelegate _ownerRotation;

		private readonly Vector2 _offset;

		/// <summary>
		/// the list of particles
		/// </summary>
		private readonly Queue<Particle> _particles;

		#endregion //Members

		#region Properties

		/// <summary>
		/// Stopwatch used to time particle creation
		/// </summary>
		public GameClock CreationTimer { get; private set; }

		/// <summary>
		/// The color of this emitter
		/// </summary>
		public Color Color { get; set; }

		/// <summary>
		/// The emitter stuff for this emitter, points to the array of emitter... stuffs
		/// </summary>
		public EmitterTemplate Template { get ;private set; }

		/// <summary>
		/// Stopwatch used to time the life of this emitter
		/// </summary>
		public CountdownTimer EmitterTimer { get; private set; }

		/// <summary>
		/// whether or not the particle emitter is flipped
		/// </summary>
		public bool Flip { get; private set; }

		public bool Expires { get; private set; }

		public Vector2 Velocity
		{
			get
			{
				return _velocity;
			}
			set
			{
				_velocity = value;
			}
		}

		public Vector2 Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
			}
		}

		#endregion //Properties

		#region Methods

		public Emitter(EmitterTemplate template, 
			Vector2 velocity, 
			Vector2 position, 
			Vector2 offset, 
			PositionDelegate myPosition, 
			RotationDelegate myRotation, 
			Color color, 
			bool isFlipped, 
			float scale,
			RotationDelegate ownerRotation)
		{
			Template = template;
			_velocity = velocity;
			_position = position;
			_offset = offset;
			Color = color;
			_positionDelegate = myPosition;
			if (null != _positionDelegate)
			{
				_position = _positionDelegate();
			}

			_rotationDelegate = myRotation;
			_ownerRotation = ownerRotation;
			Flip = isFlipped;
			Expires = template.Expires;

			//add the offset to the position
			_position += (GetOffset() * scale);

			CreationTimer = new GameClock();
			EmitterTimer = new CountdownTimer();
			_particles = new Queue<Particle>();

			//start the creation timer
			CreationTimer.Start();

			//does this emitter expire?
			if (template.EmitterLife >= 0.0f)
			{
				EmitterTimer.Start(template.EmitterLife);
			}
			else
			{
				EmitterTimer.Start(1.0f);
			}

			//create the correct number of start particles
			for (var i = 0; i < Template.NumStartParticles; i++)
			{
				AddParticle();
			}
		}

		protected void AddParticle()
		{
			var particle = new Particle();

			//set position and all the template particle parameters
			particle.Position = _position;

			//set all the random stuff for particle
			Template.SetParticle(particle, CreationTimer);

			//are we using a custom rotation?
			if (null != _rotationDelegate)
			{
				//Set the rotaion of this particle
				particle.Rotation += _rotationDelegate();

				//Rotate the velocity we shoot the particle
				var rotation = _rotationDelegate();
				//if (Flip)
				//{
				//	rotation += MathHelper.Pi;
				//}
				var rotMatrix = MatrixExt.Orientation(rotation);
				particle.Velocity += MatrixExt.Multiply(rotMatrix, _velocity);
			}
			else if (null != _ownerRotation)
			{
				//Set the rotaion of this particle
				particle.Rotation += _ownerRotation();
				particle.Velocity += _velocity;
			}
			else
			{
				particle.Velocity += _velocity;
			}

			//is the emitter flipped?
			if (Flip)
			{
				particle.Spin *= -1.0f;
				particle.Rotation += MathHelper.Pi;
			}

			_particles.Enqueue(particle);
		}

		public void Update(GameClock clock, float scale)
		{
			//update the emitter clock
			if (Template.EmitterLife >= 0.0f)
			{
				EmitterTimer.Update(clock);
			}
			CreationTimer.Update(clock);

			//update position from attached bone?
			if (null != _positionDelegate)
			{
				_position = _positionDelegate();
				_position += (GetOffset() * scale);
			}

			//update all the particles
			foreach (var particle in _particles)
			{
				particle.Update(clock, Template);
			}

			//do any particles need to be removed?
			while ((_particles.Count > 0) && _particles.Peek().IsDead())
			{
				_particles.Dequeue();
			}

			//dont add any particles if the emitter is expired
			if (HasRemainingTime())
			{
				//do any particles need to be added?
				while (CreationTimer.CurrentTime >= Template.CreationPeriod)
				{
					AddParticle();
					CreationTimer.SubtractTime(Template.CreationPeriod);
				}
			}
		}

		public void Render(SpriteBatch spritebatch)
		{
			//draw all the particles
			foreach (var particle in _particles)
			{
				if (!particle.IsDead())
				{
					particle.Render(spritebatch, this);
				}
			}
		}

		protected bool HasRemainingTime()
		{
			return (EmitterTimer.HasTimeRemaining) || !Expires;
		}

		public bool IsDead()
		{
			return (!HasRemainingTime() && (0 >= _particles.Count));
		}

		public void Stop()
		{
			Expires = true;
			EmitterTimer.Stop();
		}

		private Vector2 GetOffset()
		{
			var finalOffset = _offset;
			if (Vector2.Zero != _offset)
			{
				//get the rotation
				var rotation = 0f;
				
				//get the bone rotation
				if (null != _rotationDelegate)
				{
					rotation += _rotationDelegate();
				}
				else if (null != _ownerRotation)
				{
					//add the owner rotation
					rotation += _ownerRotation();
				}

				var rotMatrix = MatrixExt.Orientation(rotation);
				finalOffset = MatrixExt.Multiply(rotMatrix, finalOffset);

				//if the emitter is flipped, siwtch the offset
				if (Flip)
				{
					if (null == _rotationDelegate)
					{
						finalOffset.X = -finalOffset.X;
					}
				}
			}

			return finalOffset;
		}

		#endregion //Methods
	}
}