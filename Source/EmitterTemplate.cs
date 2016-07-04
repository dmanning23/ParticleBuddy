using FilenameBuddy;
using GameTimer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RandomExtensions;
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
		public Texture2D Texture { get; private set; }

		/// <summary>
		/// Color of the particle emitter
		/// </summary>
		private Color _color;

		/// <summary>
		/// min/max scale of a particle
		/// </summary>
		private Vector2 _scale;

		/// <summary>
		/// the min/max start rotation
		/// </summary>
		private Vector2 _startRotation;

		/// <summary>
		/// min/max spin of a particle
		/// </summary>
		private Vector2 _spin;

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
			get { return _color; }
		}

		private Vector2 Spin
		{
			get { return _spin; }
			set { _spin = value; }
		}

		public float MinSpin
		{
			get { return Spin.X; }
			set { _spin.X = value; }
		}

		public float MaxSpin
		{
			get { return Spin.Y; }
			set { _spin.Y = value; }
		}

		private Vector2 Scale
		{
			get { return _scale; }
			set { _scale = value; }
		}

		public float MinScale
		{
			get { return Scale.X; }
			set { _scale.X = value; }
		}

		public float MaxScale
		{
			get { return Scale.Y; }
			set { _scale.Y = value; }
		}

		private Vector2 StartRotation
		{
			get { return _startRotation; }
			set { _startRotation = value; }
		}

		public float MinStartRotation
		{
			get { return StartRotation.X; }
			set { _startRotation.X = value; }
		}

		public float MaxStartRotation
		{
			get { return StartRotation.Y; }
			set { _startRotation.Y = value; }
		}

		public Filename ImageFile { get; set; }

		private byte StartAlpha
		{
			get { return _color.A; }
			set { _color.A = value; }
		}

		private byte R
		{
			get { return _color.R; }
			set { _color.R = value; }
		}

		private byte G
		{
			get { return _color.G; }
			set { _color.G = value; }
		}

		private byte B
		{
			get { return _color.B; }
			set { _color.B = value; }
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
			_color = Color.White;
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
			ImageFile = new Filename();
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
				rParticle.Alpha = _color.A;
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
			Debug.Assert(_color == rInst._color);
			Debug.Assert(FadeSpeed == rInst.FadeSpeed);
			//Debug.Assert(m_fParticleGravity == rInst.m_fParticleGravity);
			Debug.Assert(ImageFile.File == rInst.ImageFile.File);

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
					_color.R = Convert.ToByte(strValue);
				}
				break;
				case "G":
				{
					_color.G = Convert.ToByte(strValue);
				}
				break;
				case "B":
				{
					_color.B = Convert.ToByte(strValue);
				}
				break;
				case "Alpha":
				{
					_color.A = Convert.ToByte(strValue);
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
						ImageFile = new Filename(strValue);
				}
				break;
				default:
				{
					throw new ArgumentException("EmitterTemplate xml node not recognized: " + strName);
				}
			}
		}

#if NETFX_CORE
		public override void WriteXmlNodes()
		{
		}
#else
		public override void WriteXmlNodes(XmlTextWriter xmlFile)
		{
			xmlFile.WriteStartElement("R");
			xmlFile.WriteString(_color.R.ToString());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("G");
			xmlFile.WriteString(_color.G.ToString());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("B");
			xmlFile.WriteString(_color.B.ToString());
			xmlFile.WriteEndElement();

			xmlFile.WriteStartElement("Alpha");
			xmlFile.WriteString(_color.A.ToString());
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
			xmlFile.WriteString(ImageFile.GetRelFilename());
			xmlFile.WriteEndElement();
		}
#endif

		public void LoadContent(ContentManager content)
		{
			//try to load the file into the particle effect
			if ((null != content) && !string.IsNullOrEmpty(ImageFile.File))
			{
				Texture = content.Load<Texture2D>(ImageFile.GetPathFileNoExt());
			}
		}

#endregion //File IO
	}
}