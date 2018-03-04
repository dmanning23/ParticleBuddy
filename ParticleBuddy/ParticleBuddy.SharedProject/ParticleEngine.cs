using GameTimer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParticleBuddy
{
	/// <summary>
	/// The main interface for interacting with particle effects
	/// </summary>
	public class ParticleEngine
	{
		#region Members

		/// <summary>
		/// list of current emitters
		/// </summary>
		public List<Emitter> Emitters { get; private set; }

		/// <summary>
		/// How zoomed in/out the camera is
		/// </summary>
		public float CameraScale { get; set; }

		private readonly object _lock = new object();

		#endregion //Members

		#region Methods

		/// <summary>
		/// Constructor!
		/// </summary>
		public ParticleEngine()
		{
			Emitters = new List<Emitter>();
			CameraScale = 1f;
		}

		/// <summary>
		/// Clean out the particle engine
		/// </summary>
		public void Flush()
		{
			lock (_lock)
			{
				//flush out all the emitters
				Emitters.Clear();
			}
		}

		/// <summary>
		/// Add a particle effect to the game
		/// </summary>
		/// <param name="template"></param>
		/// <param name="velocity"></param>
		/// <param name="position"></param>
		/// <param name="offset"></param>
		/// <param name="color"></param>
		/// <param name="isFlipped"></param>
		/// <param name="myPosition"></param>
		/// <param name="myRotation"></param>
		/// <returns></returns>
		public Emitter PlayParticleEffect(
			EmitterTemplate template,
			Vector2 velocity,
			Vector2 position,
			Vector2 offset,
			Color color,
			bool isFlipped,
			PositionDelegate myPosition = null,
			RotationDelegate myRotation = null,
			RotationDelegate ownerRotation = null)
		{
			if (null == template.Texture)
			{
				return null;
			}

			//spawn a particle emitter
			var myEmitter = new Emitter(
				template,
				velocity,
				position,
				offset,
				myPosition,
				myRotation,
				color,
				isFlipped,
				CameraScale,
				ownerRotation);

			lock (_lock)
			{
				//save the emitter
				Emitters.Add(myEmitter);
			}

			return myEmitter;
		}

		/// <summary>
		/// Called every frame to update the positions of emitter & particles
		/// </summary>
		/// <param name="rClock"></param>
		public void Update(GameClock clock)
		{
			List<Task> tasks = new List<Task>();

			//update all the current emitters
			for (var i = 0; i < Emitters.Count; i++)
			{
				var copy = i;
				tasks.Add(Task.Factory.StartNew(() => { Emitters[copy].Update(clock, CameraScale); }));
			}

			Task.WaitAll(tasks.ToArray());

			lock (_lock)
			{
				//remove any expired emitters
				int index = 0;
				while (index < Emitters.Count)
				{
					if (Emitters[index].IsDead())
					{
						Emitters.RemoveAt(index);
					}
					else
					{
						++index;
					}
				}
			}
		}

		/// <summary>
		/// Render all the particles.
		/// </summary>
		/// <param name="spritebatch"></param>
		public void Render(SpriteBatch spritebatch)
		{
			//render all the current emitters
			for (int i = 0; i < Emitters.Count; i++)
			{
				Emitters[i].Render(spritebatch);
			}
		}

		#endregion //Methods
	}
}