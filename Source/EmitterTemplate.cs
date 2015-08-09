using FilenameBuddy;
using GameTimer;
using Microsoft.Xna.Framework;
using RandomExtensions;
using RenderBuddy;
using System;
using System.Diagnostics;
using System.Xml;
using Vector2Extensions;
using XmlBuddy;

namespace ParticleBuddy
{
	public class EmitterTemplate : XmlFileBuddy
	{
		#region Members

		/// <summary>
		/// random number generator for particle effects
		/// </summary>
		static private Random g_Random = new Random(DateTime.Now.Millisecond);

		/// <summary>
		/// The id of the bitmap that this particle uses.
		/// </summary>
		public ITexture Bitmap { get; private set; }

		/// <summary>
		/// Color of the particle emitter
		/// </summary>
		private Color m_Color;

		/// <summary>
		/// min/max scale of a particle
		/// </summary>
		private Vector2 m_Scale;

		/// <summary>
		/// the min/max start rotation
		/// </summary>
		private Vector2 m_StartRotation;

		/// <summary>
		/// min/max spin of a particle
		/// </summary>
		private Vector2 m_Spin;

		/// <summary>
		/// used to wrap g_Random in threadsfaety
		/// </summary>
		private static readonly object _lock = new object();

		#endregion //Members

		#region Properties

		/// <summary>
		/// speed to add alpha to particles
		/// Must be between 0.0 and 1.0
		/// </summary>
		public float FadeSpeed { get; set; }

		/// <summary>
		/// Maximum range of particle velocity
		/// </summary>
		public Vector2 MaxParticleVelocity { get; set; }
		/// <summary>
		/// Minimum range of particle velocity
		/// </summary>
		public Vector2 MinParticleVelocity { get; set; }

		/// <summary>
		/// start size of a particle
		/// </summary>
		public float ParticleSize { get; set; }

		/// <summary>
		/// The number of particles this emitter starts with
		/// </summary>
		public int NumStartParticles { get; set; }

		/// <summary>
		/// time to live of this emitter
		/// </summary>
		public float EmitterLife { get; set; }

		/// <summary>
		/// time to live of particles
		/// </summary>
		public float ParticleLife { get; set; }

		/// <summary>
		/// time delta to create particles
		/// This emitter will create a particle whenever this delta expires
		/// </summary>
		public float CreationPeriod { get; set; }

		/// <summary>
		/// gravity applied to the particle's y velocity
		/// </summary>
		/// <value>The particle gravity.</value>
		public float ParticleGravity { get; set; }

		public Color ParticleColor
		{
			get { return m_Color; }
		}

		public Vector2 Spin
		{
			get { return m_Spin; }
			set { m_Spin = value; }
		}

		public float MinSpin
		{
			get { return Spin.X; }
			set { m_Spin.X = value; }
		}

		public float MaxSpin
		{
			get { return Spin.Y; }
			set { m_Spin.Y = value; }
		}

		public Vector2 Scale
		{
			get { return m_Scale; }
			set { m_Scale = value; }
		}

		public float MinScale
		{
			get { return Scale.X; }
			set { m_Scale.X = value; }
		}

		public float MaxScale
		{
			get { return Scale.Y; }
			set { m_Scale.Y = value; }
		}

		public Vector2 StartRotation
		{
			get { return m_StartRotation; }
			set { m_StartRotation = value; }
		}

		public float MinStartRotation
		{
			get { return StartRotation.X; }
			set { m_StartRotation.X = value; }
		}

		public float MaxStartRotation
		{
			get { return StartRotation.Y; }
			set { m_StartRotation.Y = value; }
		}

		public Filename BitmapFilename { get; set; }

		public byte StartAlpha
		{
			get { return m_Color.A; }
			set { m_Color.A = value; }
		}

		public byte R
		{
			get { return m_Color.R; }
			set { m_Color.R = value; }
		}

		public byte G
		{
			get { return m_Color.G; }
			set { m_Color.G = value; }
		}

		public byte B
		{
			get { return m_Color.B; }
			set { m_Color.B = value; }
		}

		#endregion //Properties

		#region Methods

		public EmitterTemplate()
			: base("ParticleBuddy.EmitterTemplate")
		{
			Init();
		}

		public EmitterTemplate(Filename file)
			: base("ParticleBuddy.EmitterTemplate", file)
		{
			Init();
		}

		private void Init()
		{
			m_Color = Color.White;
			MaxParticleVelocity = new Vector2(100.0f, -100.0f);
			MinParticleVelocity = new Vector2(-100.0f, 100.0f);
			ParticleSize = 128.0f;
			Scale = new Vector2(-100.0f, 100.0f);
			Spin = new Vector2(0.0f);
			StartRotation = new Vector2(0.0f);
			NumStartParticles = 10;
			EmitterLife = 0.1f;
			ParticleLife = 1.0f;
			CreationPeriod = 1.0f;
			FadeSpeed = 1.0f;
			ParticleGravity = 0.0f;
			BitmapFilename = new Filename();
		}

		public void SetParticle(Particle rParticle, GameClock timer)
		{
			lock (_lock)
			{
				//set all the particle parameters

				//send Y maxvelocity as min because y axis is flipped
				rParticle.SetVelocity(
					g_Random.NextFloat(MinParticleVelocity.X, MaxParticleVelocity.X),
					g_Random.NextFloat(MaxParticleVelocity.Y, MinParticleVelocity.Y));

				rParticle.Position += rParticle.Velocity * timer.TimeDelta;

				rParticle.Rotation = g_Random.NextFloat(MinStartRotation, MaxStartRotation);
				rParticle.Spin = g_Random.NextFloat(MinSpin, MaxSpin);
				rParticle.Lifespan = ParticleLife;
				rParticle.Size = ParticleSize;
				rParticle.Scale = g_Random.NextFloat(MinScale, MaxScale);
				rParticle.Alpha = m_Color.A;
			}
		}

		public bool Compare(EmitterTemplate rInst)
		{
			Debug.Assert(MaxParticleVelocity.X == rInst.MaxParticleVelocity.X);
			Debug.Assert(MaxParticleVelocity.Y == rInst.MaxParticleVelocity.Y);
			Debug.Assert(MinParticleVelocity.X == rInst.MinParticleVelocity.X);
			Debug.Assert(MinParticleVelocity.Y == rInst.MinParticleVelocity.Y);
			Debug.Assert(ParticleSize == rInst.ParticleSize);
			Debug.Assert(Scale.X == rInst.Scale.X);
			Debug.Assert(Scale.Y == rInst.Scale.Y);
			//Debug.Assert(m_fMaxSpin == rInst.m_fMaxSpin);
			//Debug.Assert(m_fMinSpin == rInst.m_fMinSpin);
			//Debug.Assert(m_fMinStartRotation == rInst.m_fMinStartRotation);
			//Debug.Assert(m_fMaxStartRotation == rInst.m_fMaxStartRotation);
			Debug.Assert(NumStartParticles == rInst.NumStartParticles);
			Debug.Assert(EmitterLife == rInst.EmitterLife);
			Debug.Assert(ParticleLife == rInst.ParticleLife);
			Debug.Assert(CreationPeriod == rInst.CreationPeriod);
			Debug.Assert(m_Color == rInst.m_Color);
			Debug.Assert(FadeSpeed == rInst.FadeSpeed);
			//Debug.Assert(m_fParticleGravity == rInst.m_fParticleGravity);
			Debug.Assert(BitmapFilename.File == rInst.BitmapFilename.File);

			return true;
		}

		#endregion //Methods

		#region File IO

		public override void ParseXmlNode(XmlNode node)
		{
			//what is in this node?
			string strName = node.Name;
			string strValue = node.InnerText;

			switch (strName)
			{
				case "Item":
				{
					//Leave this in for reading legacy file structure
					ReadChildNodes(node, ParseXmlNode);
				}
				break;
				case "Type":
				{
					//ignore this attribute
				}
				break;
				case "R":
				{
					m_Color.R = Convert.ToByte(strValue);
				}
				break;
				case "G":
				{
					m_Color.G = Convert.ToByte(strValue);
				}
				break;
				case "B":
				{
					m_Color.B = Convert.ToByte(strValue);
				}
				break;
				case "Alpha":
				{
					m_Color.A = Convert.ToByte(strValue);
				}
				break;
				case "FadeSpeed":
				{
					FadeSpeed = Convert.ToSingle(strValue);
				}
				break;
				case "MaxVelocity":
				{
					MaxParticleVelocity = strValue.ToVector2();
				}
				break;
				case "MinVelocity":
				{
					MinParticleVelocity = strValue.ToVector2();
				}
				break;
				case "ParticleSize":
				{
					ParticleSize = Convert.ToSingle(strValue);
				}
				break;
				case "Scale":
				{
					Scale = strValue.ToVector2();
				}
				break;
				case "Spin":
				{
					Spin = strValue.ToVector2();
					MinSpin = MathHelper.ToRadians(Spin.X);
					MaxSpin = MathHelper.ToRadians(Spin.Y);
				}
				break;
				case "StartRotation":
				{
					StartRotation = strValue.ToVector2();
					MinStartRotation = MathHelper.ToRadians(StartRotation.X);
					MaxStartRotation = MathHelper.ToRadians(StartRotation.Y);
				}
				break;
				case "NumStartParticles":
				{
					NumStartParticles = Convert.ToInt32(strValue);
				}
				break;
				case "EmitterLife":
				{
					EmitterLife = Convert.ToSingle(strValue);
				}
				break;
				case "ParticleLife":
				{
					ParticleLife = Convert.ToSingle(strValue);
				}
				break;
				case "CreationPeriod":
				{
					CreationPeriod = Convert.ToSingle(strValue);
				}
				break;
				case "ParticleGrav":
				{
					ParticleGravity = Convert.ToSingle(strValue);
				}
				break;
				case "BmpFileName":
				{
					BitmapFilename = new Filename(strValue);
				}
				break;
				default:
				{
					throw new ArgumentException("EmitterTemplate xml node not recognized: " + strName);
				}
			}
		}

		public override void WriteXmlNodes(System.Xml.XmlTextWriter xmlFile)
		{
			xmlFile.WriteStartElement("R");
			xmlFile.WriteString(m_Color.R.ToString());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("G");
			xmlFile.WriteString(m_Color.G.ToString());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("B");
			xmlFile.WriteString(m_Color.B.ToString());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("Alpha");
			xmlFile.WriteString(m_Color.A.ToString());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("FadeSpeed");
			xmlFile.WriteString(FadeSpeed.ToString());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("MaxVelocity");
			xmlFile.WriteString(MaxParticleVelocity.StringFromVector());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("MinVelocity");
			xmlFile.WriteString(MinParticleVelocity.StringFromVector());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("ParticleSize");
			xmlFile.WriteString(ParticleSize.ToString());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("Scale");
			xmlFile.WriteString(Scale.StringFromVector());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("StartRotation");
			Vector2 tempVector = new Vector2(MathHelper.ToDegrees(MinStartRotation), MathHelper.ToDegrees(MaxStartRotation));
			xmlFile.WriteString(tempVector.StringFromVector());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("Spin");
			tempVector = new Vector2(MathHelper.ToDegrees(MinSpin), MathHelper.ToDegrees(MaxSpin));
			xmlFile.WriteString(tempVector.StringFromVector());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("NumStartParticles");
			xmlFile.WriteString(NumStartParticles.ToString());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("EmitterLife");
			xmlFile.WriteString(EmitterLife.ToString());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("ParticleLife");
			xmlFile.WriteString(ParticleLife.ToString());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("CreationPeriod");
			xmlFile.WriteString(CreationPeriod.ToString());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("ParticleGrav");
			xmlFile.WriteString(ParticleGravity.ToString());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("BmpFileName");
			xmlFile.WriteString(BitmapFilename.GetRelFilename());
			xmlFile.WriteEndElement();
		}

		public void LoadContent(IRenderer renderer)
		{
			//try to load the file into the particle effect
			if ((null != renderer) && !String.IsNullOrEmpty(BitmapFilename.File))
			{
				Bitmap = renderer.LoadImage(BitmapFilename);
				Debug.Assert(null != Bitmap);
			}
		}

		#endregion //File IO
	}
}