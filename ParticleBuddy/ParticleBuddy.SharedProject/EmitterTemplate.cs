using FilenameBuddy;
using GameTimer;
using MathNet.Numerics;
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

		#region private properties

		private Vector2 Spin
		{
			get { return _spin; }
			set { _spin = value; }
		}

		private Vector2 Scale
		{
			get { return _scale; }
			set { _scale = value; }
		}

		private Vector2 StartRotation
		{
			get { return _startRotation; }
			set { _startRotation = value; }
		}

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

		#endregion //private properties

		/// <summary>
		/// speed to add alpha to particles
		/// Must be between 0.0 and 1.0
		/// defaults to 1.0f
		/// </summary>
		public float FadeSpeed { get; set; }

		/// <summary>
		/// Maximum range of particle velocity
		/// Defaults to 100, -100
		/// </summary>
		public Vector2 MaxParticleVelocity { get; set; }
		/// <summary>
		/// Minimum range of particle velocity
		/// Defaults to -100, 100
		/// </summary>
		public Vector2 MinParticleVelocity { get; set; }

		/// <summary>
		/// start size of a particle
		/// Defaults to 128
		/// </summary>
		public float ParticleSize { get; set; }

		/// <summary>
		/// The number of particles this emitter starts with
		/// defaults to 10
		/// </summary>
		public int NumStartParticles { get; set; }

		/// <summary>
		/// time to live of this emitter
		/// defaults to .1 second
		/// </summary>
		public float EmitterLife { get; set; }

		/// <summary>
		/// Set this to false if you want an emitter that lives the life of the particle engine.
		/// </summary>
		public bool Expires { get; set; }

		/// <summary>
		/// time to live of particles
		/// Particles are killed when this timer runs out
		/// defaults to 1 second
		/// </summary>
		public float ParticleLife { get; set; }

		/// <summary>
		/// time delta to create particles
		/// This emitter will create a particle whenever this delta expires
		/// deafults to 1 second
		/// </summary>
		public float CreationPeriod { get; set; }

		/// <summary>
		/// gravity applied to the particle's y velocity
		/// defaults to 0
		/// </summary>
		/// <value>The particle gravity.</value>
		public float ParticleGravity { get; set; }

		public Color ParticleColor
		{
			get { return _color; }
			set { _color = value; }
		}

		/// <summary>
		/// min amount to spin a particle
		/// defaults to 0
		/// </summary>
		public float MinSpin
		{
			get { return Spin.X; }
			set { _spin.X = value; }
		}

		/// <summary>
		/// max amount to spin a particle
		/// defaults to 0
		/// </summary>
		public float MaxSpin
		{
			get { return Spin.Y; }
			set { _spin.Y = value; }
		}

		/// <summary>
		/// The min amount to scale a particle
		/// defaults to -100
		/// </summary>
		public float MinScale
		{
			get { return Scale.X; }
			set { _scale.X = value; }
		}

		/// <summary>
		/// The max amount to scale a particle
		/// defaults to 100
		/// </summary>
		public float MaxScale
		{
			get { return Scale.Y; }
			set { _scale.Y = value; }
		}

		/// <summary>
		/// the min amount to start the rotation a particle
		/// defaults to 0
		/// </summary>
		public float MinStartRotation
		{
			get { return StartRotation.X; }
			set { _startRotation.X = value; }
		}

		/// <summary>
		/// the max amount to start the rotation a particle
		/// defaults to 0
		/// </summary>
		public float MaxStartRotation
		{
			get { return StartRotation.Y; }
			set { _startRotation.Y = value; }
		}

		public Filename ImageFile { get; set; }

		/// <summary>
		/// The id of the bitmap that this particle uses.
		/// </summary>
		public Texture2D Texture { get; set; }

		#endregion //Properties

		#region Methods

		public EmitterTemplate() : base("ParticleBuddy.EmitterTemplate")
		{
			FadeSpeed = 1f;
			MaxParticleVelocity = new Vector2(100f, -100f);
			MinParticleVelocity = new Vector2(-100f, 100f);
			ParticleSize = 128.0f;
			NumStartParticles = 10;
			EmitterLife = 0.1f;
			Expires = true;
			ParticleLife = 1f;
			CreationPeriod = 1.0f;
			ParticleGravity = 0.0f;
			ParticleColor = Color.White;
			Spin = Vector2.Zero;
			Scale = new Vector2(-100.0f, 100.0f);
			StartRotation = Vector2.Zero;
			ImageFile = new Filename();
			Texture = null;
		}

		public EmitterTemplate(EmitterTemplate inst) : base("ParticleBuddy.EmitterTemplate")
		{
			FadeSpeed = inst.FadeSpeed;
			MaxParticleVelocity = new Vector2(inst.MaxParticleVelocity.X, inst.MaxParticleVelocity.Y);
			MinParticleVelocity = new Vector2(inst.MinParticleVelocity.X, inst.MinParticleVelocity.Y);
			ParticleSize = inst.ParticleSize;
			NumStartParticles = inst.NumStartParticles;
			EmitterLife = inst.EmitterLife;
			Expires = inst.Expires;
			ParticleLife = inst.ParticleLife;
			CreationPeriod = inst.CreationPeriod;
			ParticleGravity = inst.ParticleGravity;
			ParticleColor = new Color(inst.ParticleColor.ToVector4());
			Spin = new Vector2(inst.Spin.X, inst.Spin.Y);
			Scale = new Vector2(inst.Scale.X, inst.Scale.Y);
			StartRotation = new Vector2(inst.StartRotation.X, inst.StartRotation.Y);
			ImageFile = new Filename(inst.ImageFile);
			Texture = inst.Texture;
		}

		public EmitterTemplate(Filename file)
			: base("ParticleBuddy.EmitterTemplate", file)
		{
			_color = Color.White;
			MaxParticleVelocity = new Vector2(100.0f, -100.0f);
			MinParticleVelocity = new Vector2(-100.0f, 100.0f);
			ParticleSize = 128.0f;
			Scale = new Vector2(-100.0f, 100.0f);
			Spin = Vector2.Zero;
			StartRotation = Vector2.Zero;
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

		public bool Compare(EmitterTemplate inst)
		{
			return FadeSpeed == inst.FadeSpeed &&
			MaxParticleVelocity.X.AlmostEqual(inst.MaxParticleVelocity.X) &&
			MaxParticleVelocity.Y.AlmostEqual(inst.MaxParticleVelocity.Y) &&
			MinParticleVelocity.X.AlmostEqual(inst.MinParticleVelocity.X) &&
			MinParticleVelocity.Y.AlmostEqual(inst.MinParticleVelocity.Y) &&
			ParticleSize == inst.ParticleSize &&
			NumStartParticles == inst.NumStartParticles &&
			EmitterLife == inst.EmitterLife &&
			Expires == inst.Expires &&
			ParticleLife == inst.ParticleLife &&
			CreationPeriod == inst.CreationPeriod &&
			ParticleGravity == inst.ParticleGravity &&
			ParticleColor == inst.ParticleColor &&
			Spin.X.AlmostEqual(inst.Spin.X) &&
			Spin.Y.AlmostEqual(inst.Spin.Y) &&
			Scale.X.AlmostEqual(inst.Scale.X) &&
			Scale.Y.AlmostEqual(inst.Scale.Y) &&
			StartRotation.X.AlmostEqual(inst.StartRotation.X) &&
			StartRotation.Y.AlmostEqual(inst.StartRotation.Y) &&
			ImageFile.File == inst.ImageFile.File;
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

#if !WINDOWS_UWP
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