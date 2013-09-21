using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GameTimer;
using RenderBuddy;

namespace ParticleBuddy
{
	public class ParticleEngine
	{
		#region Members

		//list of current emitters
		List<Emitter> m_listEmitters;

		/// <summary>
		/// How zoomed in/out the camera is
		/// </summary>
		public float CameraScale { get; set; }

		#endregion //Members

		#region Methods

		public ParticleEngine()
		{
			m_listEmitters = new List<Emitter>();
			CameraScale = 1.0f;
		}

		public void Flush()
		{
			//flush out all the emitters
			m_listEmitters.Clear();
		}

		public Emitter PlayParticleEffect(
			EmitterTemplate rTemplate, 
			Vector2 Velocity,
			Vector2 Position,
			Vector2 Offset,
			PositionDelegate myPosition,
			Color myColor,
			bool bFlip)
		{
			//spawn a particle emitter
			Emitter myEmitter = new Emitter(
				rTemplate, 
				Velocity, 
				Position, 
				Offset,
				myPosition, 
				myColor, 
				bFlip,
				CameraScale);

			//save the emitter
			m_listEmitters.Add(myEmitter);

			return myEmitter;
		}

		public void Update(GameClock rClock)
		{
			//update all the current emitters
			for (int i = 0; i < m_listEmitters.Count; i++)
			{
				m_listEmitters[i].Update(rClock, CameraScale);
			}

			//remove any expired emitters
			int iIndex = 0;
			while (iIndex < m_listEmitters.Count)
			{
				if (m_listEmitters[iIndex].IsDead())
				{
					m_listEmitters.RemoveAt(iIndex);
				}
				else
				{
					++iIndex;
				}
			}
		}

		public void Render(IRenderer rRenderer)
		{
			//render all the current emitters
			for (int i = 0; i < m_listEmitters.Count; i++)
			{
				m_listEmitters[i].Render(rRenderer);
			}
		}

		#endregion //Methods
	}
}