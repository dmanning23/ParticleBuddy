using FontBuddyLib;
using MenuBuddy;
using Microsoft.Xna.Framework;
using ResolutionBuddy;
using ParticleBuddy;
using GameTimer;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;
using System;

namespace ParticleExample
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// It contains: 
    ///		the title of the game
    ///		@dannobotgames button that opens a link to a webpage or something
    ///		a full screen button that says "Tap anywhere to begin!". This takes the player to the SelectLessonScreen
    /// </summary>
    public class MainScreen : WidgetScreen, IMainMenu
    {
        #region Properties

        /// <summary>
        /// The clock used to time this game
        /// </summary>
        private GameClock Clock { get; set; }

        /// <summary>
        /// The particle engine for this game
        /// </summary>
        private ParticleEngine ParticleEngine { get; set; }

        /// <summary>
        /// The particle effect to play whenever the user clicks on the screen
        /// </summary>
        private EmitterTemplate ClickEmitter { get; set; }

        #endregion //Properties

        #region Methods

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainScreen()
            : base("Main")
        {
        }

        public override async Task LoadContent()
        {
            await base.LoadContent();

            try
            {
                //Create the particle engine
                Clock = new GameClock();
                ParticleEngine = new ParticleEngine();

                ParticleEngine.PlayParticleEffect(new EmitterTemplate()
                {
                    ParticleSize = 32,
                    MinSpin = -5f,
                    MaxSpin = 5f,
                    ParticleGravity = 500f,
                    MaxParticleVelocity = new Vector2(200f, -300f),
                    MinParticleVelocity = new Vector2(-200f, 100f),
                    Texture = Content.Load<Texture2D>("WhiteStar"),
                    Expires = false,
                }, Vector2.Zero, Vector2.Zero, Vector2.Zero, Color.Red, false);

                //create the click emitter particle effect
                ClickEmitter = new EmitterTemplate()
                {
                    ParticleSize = 32,
                    MinSpin = -5f,
                    MaxSpin = 5f,
                    ParticleGravity = 500f,
                    MaxParticleVelocity = new Vector2(200f, -300f),
                    MinParticleVelocity = new Vector2(-200f, 100f),
                    Texture = Content.Load<Texture2D>("WhiteStar")
                };

                //add the studio tag
                var dannobotText = new FontBuddy();
                dannobotText.LoadContent(Content, StyleSheet.SmallFontResource, false, 32);
                var dannobot = new Label("@DannobotGames", dannobotText)
                {
                    ShadowColor = new Color(0.15f, 0.15f, 0.15f),
                    TextColor = new Color(0.85f, 0.85f, 0.85f, 0.7f),
                    TransitionObject = new WipeTransitionObject(TransitionWipeType.PopRight),
                    Horizontal = HorizontalAlignment.Right,
                    Vertical = VerticalAlignment.Bottom,
                    Position = new Point(Resolution.TitleSafeArea.Right, Resolution.TitleSafeArea.Bottom),
                    Highlightable = false
                };
                AddItem(dannobot);

                //Add the button that allows the user to continue to the next screen
                var button = new RelativeLayoutButton()
                {
                    Position = new Point(Resolution.ScreenArea.Left, Resolution.ScreenArea.Top),
                    Size = new Vector2(Resolution.ScreenArea.Width, Resolution.ScreenArea.Height),
                    Horizontal = HorizontalAlignment.Left,
                    Vertical = VerticalAlignment.Top,
                    TransitionObject = new WipeTransitionObject(TransitionWipeType.SlideLeft)
                };
                button.OnClick += ((onj, e) =>
                {
                    //Create a particle effect
                    ParticleEngine.PlayParticleEffect(ClickEmitter, Vector2.Zero, e.Position, Vector2.Zero, Color.White, false);
                });
                AddItem(button);

                //use a pulsate font for that button
                var pulsateFont = new PulsateBuddy();
                pulsateFont.LoadContent(Content, StyleSheet.SmallFontResource, false, 32);
                var pulsate = new Label("Tap  anywhere  to  create  a  particle!", pulsateFont)
                {
                    Horizontal = HorizontalAlignment.Center,
                    Vertical = VerticalAlignment.Center,
                    Highlightable = false,
                    Position = Resolution.ScreenArea.Center,
                };
                AddItem(pulsate);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            Clock.Update(gameTime);
            ParticleEngine.Update(Clock);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            //draw all the current particle effects
            ScreenManager.SpriteBatchBegin();
            ParticleEngine.Render(ScreenManager.SpriteBatch);
            ScreenManager.SpriteBatchEnd();
        }

        #endregion //Methods
    }
}