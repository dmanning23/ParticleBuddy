﻿using GameTimer;
using MatrixExtensions;
using Microsoft.Xna.Framework;
using RenderBuddy;
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
		private readonly Vector2 _velocity;

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

		#endregion //Properties

		#region Methods

		public Emitter(EmitterTemplate rTemplate, 
			Vector2 velocity, 
			Vector2 position, 
			Vector2 offset, 
			PositionDelegate myPosition, 
			RotationDelegate myRotation, 
			Color myColor, 
			bool bFlip, 
			float fScale,
			RotationDelegate ownerRotation)
		{
			Template = rTemplate;
			_velocity = velocity;
			_position = position;
			_offset = offset;
			_color = myColor;
			_positionDelegate = myPosition;
			if (null != _positionDelegate)
			{
				_position = _positionDelegate();
			}

			_rotationDelegate = myRotation;
			_ownerRotation = ownerRotation;
			Flip = bFlip;

			//add the offset to the position
			_position += (GetOffset() * fScale);

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
				Matrix rotation = MatrixExt.Orientation( _rotationDelegate());
				myParticle.Velocity += MatrixExt.Multiply(rotation, _velocity);
			}
			else if (null != _ownerRotation)
			{
				//Set the rotaion of this particle
				myParticle.Rotation += _ownerRotation();
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
			}

			_listParticles.Enqueue(myParticle);
		}

		public void Update(GameClock myClock, float fScale)
		{
			//update the emitter clock
			if (Template.EmitterLife >= 0.0f)
			{
				EmitterTimer.Update(myClock);
			}
			CreationTimer.Update(myClock);

			//update position from attached bone?
			if (null != _positionDelegate)
			{
				_position = _positionDelegate();
				_position += (GetOffset() * fScale);
			}

			//update all the particles
			foreach (var iter in _listParticles)
			{
				iter.Update(myClock, Template);
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

		public void Render(IRenderer myRenderer)
		{
			//draw all the particles
			foreach (var iter in _listParticles)
			{
				if (!iter.IsDead())
				{
					iter.Render(myRenderer, this);
				}
			}
		}

		public bool IsDead()
		{
			return ((0.0f >= EmitterTimer.RemainingTime()) && (0 >= _listParticles.Count));
		}

		private Vector2 GetOffset()
		{
			Vector2 finalOffset = Vector2.Zero;
			if (Vector2.Zero != _offset)
			{
				Matrix rot = MatrixExt.Orientation(_ownerRotation());
				finalOffset = MatrixExt.Multiply(rot, _offset);

				//if the emitter is flipped, siwtch the offset
				if (Flip)
				{
					finalOffset.X = -finalOffset.X;
				}
			}

			return finalOffset;
		}

		#endregion //Methods
	}
}