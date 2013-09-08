using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using FilenameBuddy;

namespace ParticleBuddy
{
	public class EmitterTemplate
	{
		#region Members

		/// <summary>
		/// Color of the particle emitter
		/// </summary>
		private Color m_Color;

		//speed to add alpha to particles
		//Must be between 0.0 and 1.0
		private float m_fFadeSpeed;

		//Maximum range of particle velocity
		public Vector2 MaxParticleVelocity { get; set; }
		//Minimum range of particle velocity
		public Vector2 MinParticleVelocity { get; set; }

		//start size of a particle
		public float ParticleSize { get; set; }

		//min/max scale of a particle
		private Vector2 m_Scale;

		/// <summary>
		/// the min/max start rotation
		/// </summary>
		private Vector2 m_StartRotation;

		//min/max spin of a particle
		private Vector2 m_Spin;

		//The number of particles this emitter starts with
		private int m_iNumStartParticles;

		//time to live of this emitter
		private float m_fEmitterLife;

		//time to live of particles
		private float m_fParticleLife;

		//time delta to create particles
		//This emitter will create a particle whenever this delta expires
		private float m_fCreationPeriod;

		//gravity applied to the particle's y velocity
		private float m_fParticleGravity;

		//The id of the bitmap that this particle uses.
		private int m_iBitmapID;

		//the filename of the bitmap of this particle
		private Filename m_strBmpFileName;

		/// <summary>
		/// random number generator for particle effects
		/// </summary>
		static private Random g_Random = new Random(DateTime.Now.Millisecond);

		#endregion //Members

		#region Properties

		public float ParticleLife
		{
			get { return m_fParticleLife; }
			set { m_fParticleLife = value; }
		}

		public float CreationPeriod
		{
			get { return m_fCreationPeriod; }
			set { m_fCreationPeriod = value; }
		}

		public float FadeSpeed
		{
			get { return m_fFadeSpeed; }
			set { m_fFadeSpeed = value; }
		}

		public float ParticleGravity
		{
			get { return m_fParticleGravity; }
			set { m_fParticleGravity = value; }
		}

		public float EmitterLife
		{
			get { return m_fEmitterLife; }
			set { m_fEmitterLife = value; }
		}

		public int ImageID
		{
			get { return m_iBitmapID; }
		}

		public int NumStartParticles
		{
			get { return m_iNumStartParticles; }
			set { m_iNumStartParticles = value; }
		}

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
			m_iBitmapID = -1;
			m_iNumStartParticles = 10;
			m_fEmitterLife = 0.1f;
			m_fParticleLife = 1.0f;
			m_fCreationPeriod = 1.0f;
			m_fFadeSpeed = 1.0f;
			m_fParticleGravity = 0.0f;
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
			rParticle.Lifespan = m_fParticleLife;
			rParticle.Size = ParticleSize;
			rParticle.Scale = g_Random.NextFloat(MinScale, MaxScale);
			rParticle.Alpha = m_Color.A;
		}

		public bool SetFilename(string strBitmapFile, IRenderer rRenderer)
		{
			//grab the filename
			m_strBmpFileName.SetRelFilename(strBitmapFile);

			//try to load the file into the particle effect
			if (null != rRenderer)
			{
				m_iBitmapID = rRenderer.LoadBitmap(m_strBmpFileName);
				if (-1 == m_iBitmapID)
				{
					return false;
				}
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
			Debug.Assert(m_iBitmapID == rInst.m_iBitmapID);
			Debug.Assert(m_iNumStartParticles == rInst.m_iNumStartParticles);
			Debug.Assert(m_fEmitterLife == rInst.m_fEmitterLife);
			Debug.Assert(m_fParticleLife == rInst.m_fParticleLife);
			Debug.Assert(m_fCreationPeriod == rInst.m_fCreationPeriod);
			Debug.Assert(m_Color == rInst.m_Color);
			Debug.Assert(m_fFadeSpeed == rInst.m_fFadeSpeed);
			//Debug.Assert(m_fParticleGravity == rInst.m_fParticleGravity);
			Debug.Assert(m_strBmpFileName.Filename == rInst.m_strBmpFileName.Filename);

			return true;
		}

		#endregion //Methods

		#region File IO

#if WINDOWS

		public bool ReadSerializedFile(CFilename strFilename, IRenderer rRenderer)
		{
			//Open the file.
			FileStream stream = File.Open(strFilename.Filename, FileMode.Open, FileAccess.Read);
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
				if (!ReadXML(rootNode.FirstChild, rRenderer))
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

		public bool ReadXML(XmlNode rXMLNode, IRenderer rRenderer)
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
						m_fFadeSpeed = Convert.ToSingle(strValue);
					}
					else if (strName == "MaxVelocity")
					{
						MaxParticleVelocity = CStringUtils.ReadVectorFromString(strValue);
					}
					else if (strName == "MinVelocity")
					{
						MinParticleVelocity = CStringUtils.ReadVectorFromString(strValue);
					}
					else if (strName == "ParticleSize")
					{
						ParticleSize = Convert.ToSingle(strValue);
					}
					else if (strName == "Scale")
					{
						Scale = CStringUtils.ReadVectorFromString(strValue);
					}
					else if (strName == "Spin")
					{
						Spin = CStringUtils.ReadVectorFromString(strValue);
						MinSpin = MathHelper.ToRadians(Spin.X);
						MaxSpin = MathHelper.ToRadians(Spin.Y);
					}
					else if (strName == "StartRotation")
					{
						StartRotation = CStringUtils.ReadVectorFromString(strValue);
						MinStartRotation = MathHelper.ToRadians(StartRotation.X);
						MaxStartRotation = MathHelper.ToRadians(StartRotation.Y);
					}
					else if (strName == "NumStartParticles")
					{
						m_iNumStartParticles = Convert.ToInt32(strValue);
					}
					else if (strName == "EmitterLife")
					{
						m_fEmitterLife = Convert.ToSingle(strValue);
					}
					else if (strName == "ParticleLife")
					{
						m_fParticleLife = Convert.ToSingle(strValue);
					}
					else if (strName == "CreationPeriod")
					{
						m_fCreationPeriod = Convert.ToSingle(strValue);
					}
					else if (strName == "ParticleGrav")
					{
						m_fParticleGravity = Convert.ToSingle(strValue);
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
		public void WriteXMLFormat(string strFileName)
		{
			//open the file, create it if it doesnt exist yet
			XmlTextWriter rFile = new XmlTextWriter(strFileName, null);
			rFile.Formatting = Formatting.Indented;
			rFile.Indentation = 1;
			rFile.IndentChar = '\t';

			rFile.WriteStartDocument();
			WriteXML(rFile, true);
			rFile.WriteEndDocument();

			// Close the file.
			rFile.Flush();
			rFile.Close();
		}

		/// <summary>
		/// write out particle emitter template to XML file
		/// </summary>
		/// <param name="rXMLFile"></param>
		public void WriteXML(XmlTextWriter rXMLFile, bool bStartElement)
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
			rXMLFile.WriteString(m_fFadeSpeed.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("MaxVelocity");
			rXMLFile.WriteString(CStringUtils.StringFromVector(MaxParticleVelocity));
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("MinVelocity");
			rXMLFile.WriteString(CStringUtils.StringFromVector(MinParticleVelocity));
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("ParticleSize");
			rXMLFile.WriteString(ParticleSize.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("Scale");
			rXMLFile.WriteString(CStringUtils.StringFromVector(Scale));
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("StartRotation");
			Vector2 tempVector = new Vector2(MathHelper.ToDegrees(MinStartRotation), MathHelper.ToDegrees(MaxStartRotation));
			rXMLFile.WriteString(CStringUtils.StringFromVector(tempVector));
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("Spin");
			tempVector = new Vector2(MathHelper.ToDegrees(MinSpin), MathHelper.ToDegrees(MaxSpin));
			rXMLFile.WriteString(CStringUtils.StringFromVector(tempVector));
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("NumStartParticles");
			rXMLFile.WriteString(m_iNumStartParticles.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("EmitterLife");
			rXMLFile.WriteString(m_fEmitterLife.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("ParticleLife");
			rXMLFile.WriteString(m_fParticleLife.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("CreationPeriod");
			rXMLFile.WriteString(m_fCreationPeriod.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("ParticleGrav");
			rXMLFile.WriteString(m_fParticleGravity.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("BmpFileName");
			rXMLFile.WriteString(m_strBmpFileName.GetRelFilename());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteEndElement(); //Item
		}

#endif

		public bool ReadSerializedFile(ContentManager rXmlContent, string strResource, IRenderer rRenderer)
		{
			//load the resource
			ParticleXML myDude = rXmlContent.Load<ParticleXML>(strResource);
			return ReadSerializedObject(myDude, rRenderer);
		}

		public bool ReadSerializedObject(ParticleXML myParticle, IRenderer rRenderer)
		{
			//copy data from the serialized object

			m_Color.R = myParticle.R;
			m_Color.G = myParticle.G;
			m_Color.B = myParticle.B;
			m_Color.A = myParticle.Alpha;
			m_fFadeSpeed = myParticle.FadeSpeed;

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

			m_iNumStartParticles = myParticle.NumStartParticles;
			m_fEmitterLife = myParticle.EmitterLife;
			m_fParticleLife = myParticle.ParticleLife;
			m_fCreationPeriod = myParticle.CreationPeriod;
			m_fParticleGravity = myParticle.ParticleGrav;
			SetFilename(myParticle.BmpFileName, rRenderer);

			return true;
		}

		#endregion //File IO
	}
}