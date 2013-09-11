using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using FilenameBuddy;
using RandomExtensions;
using RenderBuddy;
using Vector2Extensions;

namespace ParticleBuddy
{
	public class EmitterTemplate
	{
		#region Members

		/// <summary>
		/// random number generator for particle effects
		/// </summary>
		static private Random g_Random = new Random(DateTime.Now.Millisecond);

		/// <summary>
		/// Color of the particle emitter
		/// </summary>
		private Color m_Color;

		//The id of the bitmap that this particle uses.
		public Texture2D Bitmap { get; private set; }

		//the filename of the bitmap of this particle
		private Filename m_strBmpFileName;

		//min/max scale of a particle
		private Vector2 m_Scale;

		/// <summary>
		/// the min/max start rotation
		/// </summary>
		private Vector2 m_StartRotation;

		//min/max spin of a particle
		private Vector2 m_Spin;

		/// <summary>
		/// speed to add alpha to particles
		/// Must be between 0.0 and 1.0
		/// </summary>
		public float FadeSpeed { get; set; }

		//Maximum range of particle velocity
		public Vector2 MaxParticleVelocity { get; set; }
		//Minimum range of particle velocity
		public Vector2 MinParticleVelocity { get; set; }

		//start size of a particle
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

		#endregion //Members

		#region Properties

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

		public Filename Filename
		{
			get { return m_strBmpFileName; }
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

		public byte StartAlpha
		{
			get { return m_Color.A; }
			set { m_Color.A = value; }
		}

		#endregion //Properties

		#region Methods

		public EmitterTemplate()
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
			m_strBmpFileName = new Filename();
		}

		public void SetParticle(Particle rParticle)
		{
			//set all the particle parameters

			//send Y maxvelocity as min because y axis is flipped
			rParticle.SetVelocity(
				g_Random.NextFloat(MinParticleVelocity.X, MaxParticleVelocity.X),
				g_Random.NextFloat(MaxParticleVelocity.Y, MinParticleVelocity.Y));

			rParticle.Rotation = g_Random.NextFloat(MinStartRotation, MaxStartRotation);
			rParticle.Spin = g_Random.NextFloat(MinSpin, MaxSpin);
			rParticle.Lifespan = ParticleLife;
			rParticle.Size = ParticleSize;
			rParticle.Scale = g_Random.NextFloat(MinScale, MaxScale);
			rParticle.Alpha = m_Color.A;
		}

		public bool SetFilename(string strBitmapFile, Renderer rRenderer)
		{
			//grab the filename
			m_strBmpFileName.SetRelFilename(strBitmapFile);

			//try to load the file into the particle effect
			if (null != rRenderer)
			{
				Bitmap = rRenderer.Content.Load<Texture2D>(m_strBmpFileName.ToString());
				Debug.Assert(null != Bitmap);
			}

			return true;
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
			Debug.Assert(m_strBmpFileName.File == rInst.m_strBmpFileName.File);

			return true;
		}

		#endregion //Methods

		#region File IO

		public bool ReadXmlFile(Filename strFilename, Renderer rRenderer)
		{
			//Open the file.
			FileStream stream = File.Open(strFilename.File, FileMode.Open, FileAccess.Read);
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(stream);
			XmlNode rootNode = xmlDoc.DocumentElement;

			//make sure it is actually an xml node
			if (rootNode.NodeType == XmlNodeType.Element)
			{
				//eat up the name of that xml node
				string strElementName = rootNode.Name;
				if (("XnaContent" != strElementName) || !rootNode.HasChildNodes)
				{
					return false;
				}

				//make sure to read from the the next node
				if (!ReadXmlObject(rootNode.FirstChild, rRenderer))
				{
					return false;
				}
			}
			else
			{
				//should be an xml node!!!
				return false;
			}

			// Close the file.
			stream.Close();
			return true;
		}

		public bool ReadXmlObject(XmlNode rXMLNode, Renderer rRenderer)
		{
			//should have an attribute Type
			XmlNamedNodeMap mapAttributes = rXMLNode.Attributes;
			for (int i = 0; i < mapAttributes.Count; i++)
			{
				//will only have the name attribute
				string strName = mapAttributes.Item(i).Name;
				string strValue = mapAttributes.Item(i).Value;
				if ("Type" == strName)
				{
					if (strValue != "SPFSettings.ParticleXML")
					{
						Debug.Assert(false);
						return false;
					}
				}
				else
				{
					Debug.Assert(false);
					return false;
				}
			}

			//Read in child nodes
			if (rXMLNode.HasChildNodes)
			{
				for (XmlNode childNode = rXMLNode.FirstChild;
					null != childNode;
					childNode = childNode.NextSibling)
				{
					//what is in this node?
					string strName = childNode.Name;
					string strValue = childNode.InnerText;

					if (strName == "R")
					{
						m_Color.R = Convert.ToByte(strValue);
					}
					else if (strName == "G")
					{
						m_Color.G = Convert.ToByte(strValue);
					}
					else if (strName == "B")
					{
						m_Color.B = Convert.ToByte(strValue);
					}
					else if (strName == "Alpha")
					{
						m_Color.A = Convert.ToByte(strValue);
					}
					else if (strName == "FadeSpeed")
					{
						FadeSpeed = Convert.ToSingle(strValue);
					}
					else if (strName == "MaxVelocity")
					{
						MaxParticleVelocity = strValue.ToVector2();
					}
					else if (strName == "MinVelocity")
					{
						MinParticleVelocity = strValue.ToVector2();
					}
					else if (strName == "ParticleSize")
					{
						ParticleSize = Convert.ToSingle(strValue);
					}
					else if (strName == "Scale")
					{
						Scale = strValue.ToVector2();
					}
					else if (strName == "Spin")
					{
						Spin = strValue.ToVector2();
						MinSpin = MathHelper.ToRadians(Spin.X);
						MaxSpin = MathHelper.ToRadians(Spin.Y);
					}
					else if (strName == "StartRotation")
					{
						StartRotation = strValue.ToVector2();
						MinStartRotation = MathHelper.ToRadians(StartRotation.X);
						MaxStartRotation = MathHelper.ToRadians(StartRotation.Y);
					}
					else if (strName == "NumStartParticles")
					{
						NumStartParticles = Convert.ToInt32(strValue);
					}
					else if (strName == "EmitterLife")
					{
						EmitterLife = Convert.ToSingle(strValue);
					}
					else if (strName == "ParticleLife")
					{
						ParticleLife = Convert.ToSingle(strValue);
					}
					else if (strName == "CreationPeriod")
					{
						CreationPeriod = Convert.ToSingle(strValue);
					}
					else if (strName == "ParticleGrav")
					{
						ParticleGravity = Convert.ToSingle(strValue);
					}
					else if (strName == "BmpFileName")
					{
						SetFilename(strValue, rRenderer);
					}
					else
					{
						Debug.Assert(false);
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Open an xml file and dump model data to it
		/// </summary>
		/// <param name="strFileName">name of the file to dump to</param>
		public void WriteXmlFile(string strFileName)
		{
			//open the file, create it if it doesnt exist yet
			XmlTextWriter rFile = new XmlTextWriter(strFileName, null);
			rFile.Formatting = Formatting.Indented;
			rFile.Indentation = 1;
			rFile.IndentChar = '\t';

			rFile.WriteStartDocument();
			WriteXmlObject(rFile, true);
			rFile.WriteEndDocument();

			// Close the file.
			rFile.Flush();
			rFile.Close();
		}

		/// <summary>
		/// write out particle emitter template to XML file
		/// </summary>
		/// <param name="rXMLFile"></param>
		public void WriteXmlObject(XmlTextWriter rXMLFile, bool bStartElement)
		{
			if (bStartElement)
			{
				rXMLFile.WriteStartElement("XnaContent");
				rXMLFile.WriteStartElement("Asset");
			}
			else
			{
				rXMLFile.WriteStartElement("Item");
			}
			rXMLFile.WriteAttributeString("Type", "SPFSettings.ParticleXML");

			rXMLFile.WriteStartElement("R");
			rXMLFile.WriteString(m_Color.R.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("G");
			rXMLFile.WriteString(m_Color.G.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("B");
			rXMLFile.WriteString(m_Color.B.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("Alpha");
			rXMLFile.WriteString(m_Color.A.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("FadeSpeed");
			rXMLFile.WriteString(FadeSpeed.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("MaxVelocity");
			rXMLFile.WriteString(MaxParticleVelocity.StringFromVector());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("MinVelocity");
			rXMLFile.WriteString(MinParticleVelocity.StringFromVector());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("ParticleSize");
			rXMLFile.WriteString(ParticleSize.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("Scale");
			rXMLFile.WriteString(Scale.StringFromVector());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("StartRotation");
			Vector2 tempVector = new Vector2(MathHelper.ToDegrees(MinStartRotation), MathHelper.ToDegrees(MaxStartRotation));
			rXMLFile.WriteString(tempVector.StringFromVector());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("Spin");
			tempVector = new Vector2(MathHelper.ToDegrees(MinSpin), MathHelper.ToDegrees(MaxSpin));
			rXMLFile.WriteString(tempVector.StringFromVector());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("NumStartParticles");
			rXMLFile.WriteString(NumStartParticles.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("EmitterLife");
			rXMLFile.WriteString(EmitterLife.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("ParticleLife");
			rXMLFile.WriteString(ParticleLife.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("CreationPeriod");
			rXMLFile.WriteString(CreationPeriod.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("ParticleGrav");
			rXMLFile.WriteString(ParticleGravity.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("BmpFileName");
			rXMLFile.WriteString(m_strBmpFileName.GetRelFilename());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteEndElement(); //Item
		}

		public bool ReadSerializedFile(ContentManager rXmlContent, string strResource, Renderer rRenderer)
		{
			//load the resource
			ParticleXML myDude = rXmlContent.Load<ParticleXML>(strResource);
			return ReadSerializedObject(myDude, rRenderer);
		}

		public bool ReadSerializedObject(ParticleXML myParticle, Renderer rRenderer)
		{
			//copy data from the serialized object

			m_Color.R = myParticle.R;
			m_Color.G = myParticle.G;
			m_Color.B = myParticle.B;
			m_Color.A = myParticle.Alpha;
			FadeSpeed = myParticle.FadeSpeed;

			MaxParticleVelocity = myParticle.MaxVelocity;
			MinParticleVelocity = myParticle.MinVelocity;
			Debug.Assert(MinParticleVelocity.X <= MaxParticleVelocity.X);
			Debug.Assert(MinParticleVelocity.Y >= MaxParticleVelocity.Y);

			ParticleSize = myParticle.ParticleSize;

			Scale = myParticle.Scale;
			Debug.Assert(MinScale <= MaxScale);

			MinStartRotation = MathHelper.ToRadians(myParticle.StartRotation.X);
			MaxStartRotation = MathHelper.ToRadians(myParticle.StartRotation.Y);
			Debug.Assert(MinStartRotation <= MaxStartRotation);

			MinSpin = MathHelper.ToRadians(myParticle.Spin.X);
			MaxSpin = MathHelper.ToRadians(myParticle.Spin.Y);
			Debug.Assert(MinSpin <= MaxSpin);

			NumStartParticles = myParticle.NumStartParticles;
			EmitterLife = myParticle.EmitterLife;
			ParticleLife = myParticle.ParticleLife;
			CreationPeriod = myParticle.CreationPeriod;
			ParticleGravity = myParticle.ParticleGrav;
			SetFilename(myParticle.BmpFileName, rRenderer);

			return true;
		}

		#endregion //File IO
	}
}