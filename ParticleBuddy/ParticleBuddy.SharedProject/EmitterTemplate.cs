using FilenameBuddy;
using GameTimer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RandomExtensions;
using RenderBuddy;
using System;
using System.Xml;
using Vector2Extensions;
using XmlBuddy;

namespace ParticleBuddy
{
	public class EmitterTemplate : XmlFileBuddy
	{
		#region Properties

		/// <summary>
		/// random number generator for particle effects
		/// </summary>
		static private Random _random = new Random(DateTime.Now.Millisecond);

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

		protected Vector2 Spin
		{
			get { return _spin; }
			set { _spin = value; }
		}

		protected Vector2 Scale
		{
			get { return _scale; }
			set { _scale = value; }
		}

		protected Vector2 StartRotation
		{
			get { return _startRotation; }
			set { _startRotation = value; }
		}

		private byte StartAlpha
		{
			get { return _color.A; }
			set { _color.A = value; }
		}

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

		public Vector2 Gravity { get; set; }

		/// <summary>
		/// gravity applied to the particle's y velocity
		/// defaults to 0
		/// </summary>
		/// <value>The particle gravity.</value>
		public float ParticleGravity
		{
			get
			{
				return Gravity.Y;
			}
			set
			{
				Gravity = new Vector2(0, value);
			}
		}

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

		/// <summary>
		/// Relative filename from the content file
		/// </summary>
		public Filename ImageFile { get; set; } = new Filename();

		/// <summary>
		/// Relative file from the emitter xml
		/// </summary>
		public Filename RelativeFile { get; set; } = new Filename();

		/// <summary>
		/// The id of the bitmap that this particle uses.
		/// </summary>
		public Texture2D Texture { get; set; }

		#endregion //Properties

		#region Methods

		public EmitterTemplate() : base("ParticleBuddy.EmitterTemplate")
		{
			Init();
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
			Gravity = new Vector2(inst.Gravity.X, inst.Gravity.Y);
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
			Init();
		}

		private void Init()
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
			Gravity = Vector2.Zero;
			ParticleColor = Color.White;
			Spin = Vector2.Zero;
			Scale = new Vector2(-100.0f, 100.0f);
			StartRotation = Vector2.Zero;
			ImageFile = new Filename();
			Texture = null;
		}

		public void SetParticle(Particle particle, GameClock timer)
		{
			lock (_lock)
			{
				//set all the particle parameters

				//send Y maxvelocity as min because y axis is flipped
				particle.SetVelocity(
					_random.NextFloat(MinParticleVelocity.X, MaxParticleVelocity.X),
					_random.NextFloat(MaxParticleVelocity.Y, MinParticleVelocity.Y));

				particle.Position += particle.Velocity * timer.TimeDelta;

				particle.Rotation = _random.NextFloat(MinStartRotation, MaxStartRotation);
				particle.Spin = _random.NextFloat(MinSpin, MaxSpin);
				particle.Lifespan = ParticleLife;
				particle.Size = ParticleSize;
				particle.Scale = _random.NextFloat(MinScale, MaxScale);
				particle.Alpha = _color.A;
			}
		}

		#endregion //Methods

		#region File IO

		public override void ParseXmlNode(XmlNode node)
		{
			//what is in this node?
			string name = node.Name;
			string value = node.InnerText;

			switch (name)
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
				case "color":
					{
						_color = value.ToColor();
					}
					break;
				case "R":
					{
						_color.R = Convert.ToByte(value);
					}
					break;
				case "G":
					{
						_color.G = Convert.ToByte(value);
					}
					break;
				case "B":
					{
						_color.B = Convert.ToByte(value);
					}
					break;
				case "Alpha":
					{
						_color.A = Convert.ToByte(value);
					}
					break;
				case "FadeSpeed":
					{
						FadeSpeed = Convert.ToSingle(value);
					}
					break;
				case "MaxVelocity":
					{
						MaxParticleVelocity = value.ToVector2();
					}
					break;
				case "MinVelocity":
					{
						MinParticleVelocity = value.ToVector2();
					}
					break;
				case "ParticleSize":
					{
						ParticleSize = Convert.ToSingle(value);
					}
					break;
				case "Scale":
					{
						Scale = value.ToVector2();
					}
					break;
				case "Spin":
					{
						Spin = value.ToVector2();
						MinSpin = MathHelper.ToRadians(Spin.X);
						MaxSpin = MathHelper.ToRadians(Spin.Y);
					}
					break;
				case "StartRotation":
					{
						StartRotation = value.ToVector2();
						MinStartRotation = MathHelper.ToRadians(StartRotation.X);
						MaxStartRotation = MathHelper.ToRadians(StartRotation.Y);
					}
					break;
				case "NumStartParticles":
					{
						NumStartParticles = Convert.ToInt32(value);
					}
					break;
				case "EmitterLife":
					{
						EmitterLife = Convert.ToSingle(value);
					}
					break;
				case "ParticleLife":
					{
						ParticleLife = Convert.ToSingle(value);
					}
					break;
				case "CreationPeriod":
					{
						CreationPeriod = Convert.ToSingle(value);
					}
					break;
				case "Gravity":
					{
						Gravity = value.ToVector2();
					}
					break;
				case "ParticleGrav":
					{
						ParticleGravity = Convert.ToSingle(value);
					}
					break;
				case "BmpFileName":
					{
						if (!string.IsNullOrEmpty(value))
						{
							ImageFile = new Filename(value);
						}
					}
					break;
				case "RelativeFile":
					{
						if (!Filename.HasFilename)
						{
							throw new Exception("The Filename of the EmitterTemplate is not set.");
						}

						if (!string.IsNullOrEmpty(value))
						{
							RelativeFile.SetFilenameRelativeToPath(Filename, value);
						}
					}
					break;
				default:
					{
						throw new ArgumentException("EmitterTemplate xml node not recognized: " + name);
					}
			}
		}

		public override void WriteXmlNodes(XmlTextWriter xmlFile)
		{
			xmlFile.WriteStartElement("color");
			xmlFile.WriteString(_color.StringFromColor());
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

			xmlFile.WriteStartElement("Gravity");
			xmlFile.WriteString(Gravity.StringFromVector());
			xmlFile.WriteEndElement();

			if (RelativeFile.HasFilename)
			{
				xmlFile.WriteStartElement("RelativeFile");
				xmlFile.WriteString(RelativeFile.GetFilenameRelativeToPath(Filename));
				xmlFile.WriteEndElement();
			}
			else if (ImageFile.HasFilename)
			{
				xmlFile.WriteStartElement("BmpFileName");
				xmlFile.WriteString(ImageFile.GetRelFilename());
				xmlFile.WriteEndElement();
			}
		}

		public void LoadContent(IRenderer renderer)
		{
			//try to load the file into the particle effect
			if (null != renderer)
			{
				if (RelativeFile.HasFilename)
				{
					var textureInfo = renderer.LoadImage(RelativeFile);
					Texture = textureInfo.Texture;
				}
				else if (ImageFile.HasFilename)
				{
					var textureInfo = renderer.LoadImage(ImageFile);
					Texture = textureInfo.Texture;
				}
			}
		}

		public void LoadContent(ContentManager content)
		{
			//try to load the file into the particle effect
			if (null != content)
			{
				if (RelativeFile.HasFilename)
				{
					Texture = content.Load<Texture2D>(RelativeFile.GetPathFileNoExt());
				}
				else if (ImageFile.HasFilename)
				{
					Texture = content.Load<Texture2D>(ImageFile.GetPathFileNoExt());
				}
			}
		}

		#endregion //File IO
	}
}