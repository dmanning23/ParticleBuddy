using InputHelper;
using MenuBuddy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ParticleExample
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
#if __IOS__ || ANDROID || WINDOWS_UAP
	public class Game1 : TouchGame
#else
    public class Game1 : MouseGame
#endif
    {
        public Game1()
        {
            var debug = new DebugInputComponent(this, ResolutionBuddy.Resolution.TransformationMatrix);
            //DesiredScreenResolution = new Point(720, 1280);
        }

        public override IScreen[] GetMainMenuScreenStack()
        {
            return new IScreen[]
            {
                new MainScreen()
            };
        }

        protected override void InitStyles()
        {
            //TODO: init all your style changes here

            base.InitStyles();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            //TODO: load all your content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
#if !__IOS__
            // For Mobile devices, this logic will close the Game when the Back button is pressed
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
#endif

            // TODO: Add your update logic here
            base.Update(gameTime);
        }
    }
}
