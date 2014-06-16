using System.Threading.Tasks;
using GameTimer;
using Microsoft.Xna.Framework;
using RenderBuddy;
using System.Collections.Generic;

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
		private List<Emitter> Emitters { get; set; }

		/// <summary>
		/// How zoomed in/out the camera is
		/// </summary>
		public float CameraScale { get; set; }

		#endregion //Members

		#region Methods

		/// <summary>
		/// Constructor!
		/// </summary>
		public ParticleEngine()
		{
			Emitters = new List<Emitter>();
			CameraScale = 1.0f;
		}

		/// <summary>
		/// Clean out the particle engine
		/// </summary>
		public void Flush()
		{
			//flush out all the emitters
			Emitters.Clear();
		}

		/// <summary>
		/// Add a particle effect to the game
		/// </summary>
		/// <param name="rTemplate"></param>
		/// <param name="velocity"></param>
		/// <param name="position"></param>
		/// <param name="offset"></param>
		/// <param name="myColor"></param>
		/// <param name="bFlip"></param>
		/// <param name="myPosition"></param>
		/// <param name="myRotation"></param>
		/// <returns></returns>
		public Emitter PlayParticleEffect(
			EmitterTemplate rTemplate, 
			Vector2 velocity,
			Vector2 position,
			Vector2 offset,
			Color myColor,
			bool bFlip,
			PositionDelegate myPosition = null,
			RotationDelegate myRotation = null,
			RotationDelegate ownerRotation = null)
		{
			if (null == rTemplate.Bitmap)
			{
				return null;
			}

			//spawn a particle emitter
			Emitter myEmitter = new Emitter(
				rTemplate,
				velocity,
				position,
				offset,
				myPosition, 
				myRotation,
				myColor, 
				bFlip,
				CameraScale,
				ownerRotation);

			//save the emitter
			Emitters.Add(myEmitter);

			return myEmitter;
		}

		/// <summary>
		/// Called every frame to update the positions of emitter & particles
		/// </summary>
		/// <param name="rClock"></param>
		public void Update(GameClock rClock)
		{
			//List<Task> tasks = new List<Task>();

			//update all the current emitters
			for (int i = 0; i < Emitters.Count; i++)
			{
				Emitters[i].Update(rClock, CameraScale);
				//tasks.Add(Task.Run(() => { Emitters[i].Update(rClock, CameraScale); }));
			}

			//Task.WaitAll(tasks.ToArray());

			//remove any expired emitters
			int iIndex = 0;
			while (iIndex < Emitters.Count)
			{
				if (Emitters[iIndex].IsDead())
				{
					Emitters.RemoveAt(iIndex);
				}
				else
				{
					++iIndex;
				}
			}
		}

		/// <summary>
		/// Render all the particles.
		/// </summary>
		/// <param name="rRenderer"></param>
		public void Render(IRenderer rRenderer)
		{
			//render all the current emitters
			for (int i = 0; i < Emitters.Count; i++)
			{
				Emitters[i].Render(rRenderer);
			}
		}

		#endregion //Methods
	}
}