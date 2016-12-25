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
		/// The color of this emitter
		/// </summary>
		private readonly Color _color;

		/// <summary>
		/// the list of particles
		/// </summary>
		private readonly Queue<Particle> _listParticles;

		#endregion //Members

		#region Properties

		/// <summary>
		/// Stopwatch used to time particle creation
		/// </summary>
		public GameClock CreationTimer { get; private set; }

		public Color MyColor
		{
			get { return _color; }
		}

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

		public Emitter(EmitterTemplate rTemplate, 
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
			Template = rTemplate;
			_velocity = velocity;
			_position = position;
			_offset = offset;
			_color = color;
			_positionDelegate = myPosition;
			if (null != _positionDelegate)
			{
				_position = _positionDelegate();
			}

			_rotationDelegate = myRotation;
			_ownerRotation = ownerRotation;
			Flip = isFlipped;

			//add the offset to the position
			_position += (GetOffset() * scale);

			CreationTimer = new GameClock();
			EmitterTimer = new CountdownTimer();
			_listParticles = new Queue<Particle>();

			//start the creation timer
			CreationTimer.Start();

			//does this emitter expire?
			if (rTemplate.EmitterLife >= 0.0f)
			{
				EmitterTimer.Start(rTemplate.EmitterLife);
			}
			else
			{
				EmitterTimer.Start(1.0f);
			}

			//create the correct number of start particles
			for (int i = 0; i < Template.NumStartParticles; i++)
			{
				AddParticle();
			}
		}

		protected void AddParticle()
		{
			Particle myParticle = new Particle();

			//set position and all the template particle parameters
			myParticle.Position = _position;

			//set all the random stuff for particle
			Template.SetParticle(myParticle, CreationTimer);

			//are we using a custom rotation?
			if (null != _rotationDelegate)
			{
				//Set the rotaion of this particle
				myParticle.Rotation += _rotationDelegate();

				//Rotate the velocity we shoot the particle
				float rotation = _rotationDelegate();
				//if (Flip)
				//{
				//	rotation += MathHelper.Pi;
				//}
				Matrix rotMatrix = MatrixExt.Orientation(rotation);
				myParticle.Velocity += MatrixExt.Multiply(rotMatrix, _velocity);
			}
			else if (null != _ownerRotation)
			{
				//Set the rotaion of this particle
				myParticle.Rotation += _ownerRotation();
				myParticle.Velocity += _velocity;
			}
			else
			{
				myParticle.Velocity += _velocity;
			}

			//is the emitter flipped?
			if (Flip)
			{
				myParticle.Spin *= -1.0f;
				//myParticle.VelocityX *= -1.0f;
				//myParticle.Rotation = Helper.ClampAngle(myParticle.Rotation);
				//myParticle.Rotation = MathHelper.Pi - myParticle.Rotation;
				myParticle.Rotation += MathHelper.Pi;
			}

			_listParticles.Enqueue(myParticle);
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
			foreach (var iter in _listParticles)
			{
				iter.Update(clock, Template);
			}

			//do any particles need to be removed?
			while ((_listParticles.Count > 0) && _listParticles.Peek().IsDead())
			{
				_listParticles.Dequeue();
			}

			//dont add any particles if the emitter is expired
			if (0.0f < EmitterTimer.RemainingTime())
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
			foreach (var iter in _listParticles)
			{
				if (!iter.IsDead())
				{
					iter.Render(spritebatch, this);
				}
			}
		}

		public bool IsDead()
		{
			return ((0.0f >= EmitterTimer.RemainingTime()) && (0 >= _listParticles.Count));
		}

		private Vector2 GetOffset()
		{
			Vector2 finalOffset = _offset;
			if (Vector2.Zero != _offset)
			{
				//get the rotation
				float rotation = 0.0f;
				
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

				Matrix rotMatrix = MatrixExt.Orientation(rotation);
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