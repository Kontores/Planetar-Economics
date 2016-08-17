using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Planetar_Economics
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Texture2D stars;
        private Rectangle backgroundrectangle;

        private SpriteFont font;

        private Texture2D sun;
        private Rectangle sunrectangle;

        private Planet[] planet;

        private Ship[] ship;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            font = Content.Load<SpriteFont>("Font1");
            stars = Content.Load<Texture2D>("stars");
            sun = Content.Load<Texture2D>("sun_white_128x128");

            backgroundrectangle = new Rectangle(0, 0, 512, 512);
            sunrectangle = new Rectangle(0, 0, 128, 128);

            planet = new Planet[3];

                    
            planet[0] = new Planet(this.Content,  "carbon_dioxyde", "sulfur", 0.27f, 150, 0.004, 180, true, 1);
            planet[1] = new Planet(this.Content, "ammony",  "fuel", 0.37f, 240, 0.002, 120, false, 2);
            planet[2] = new Planet(this.Content, "nitro_oxygene", "wheat",  0.42f, 350, 0.003, 1.8, true, 3);

            ship = new Ship[5];

            ship[0] = new Ship(this.Content, 1, "Atlantis", "sulfur", new Vector2(400, 400), planet, 300.00);
            ship[1] = new Ship(this.Content, 2, "Freightliner", "fuel", new Vector2(452, 641), planet, 300.00);
            ship[2] = new Ship(this.Content, 3, "Xa'rch", "wheat", new Vector2(128, 256), planet, 300.00);
            ship[3] = new Ship(this.Content, 1, "Vega", "fuel", new Vector2(752, 241), planet, 300.00);
            ship[4] = new Ship(this.Content, 2, "TR-25", "wheat", new Vector2(828, 456), planet, 300.00);

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);

            foreach (Planet planet in planet)
                planet.Update(gameTime);

            foreach (Ship ship in ship)
                ship.Update(gameTime);


            // prevent multiple selection

            for (int i = 0; i < ship.Length; i++)
            {
                if (ship[i].active == true)
                {
                    for (int j = 0; j < ship.Length; j++)
                    {
                        if (j != i)
                            ship[j].active = false;
                    }

                    for (int p = 0; p < planet.Length; p++)
                        planet[p].active = false;
                }
            }

            for ( int a = 0; a < planet.Length; a++)
                if (planet[a].active == true)
                {
                    for (int b = 0; b < planet.Length; b++)
                    {
                        if (b != a)
                            planet[b].active = false;
                    }
                }
        }
                    
                
            

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(stars, new Vector2(0, 0), backgroundrectangle, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(stars, new Vector2(512, 0), backgroundrectangle, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(stars, new Vector2(0, 512), backgroundrectangle, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(stars, new Vector2(512, 512), backgroundrectangle, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(sun, new Vector2(445, 300), sunrectangle, Color.White, 0f, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0f);
      
            
            for(int i = 0; i < planet.Length; i++)
               planet[i].Draw(spriteBatch);

            foreach (Ship ship in ship)
                ship.Draw(spriteBatch);

            foreach (Planet p in planet)
                p.Drawmenu(spriteBatch);

            foreach (Ship ship in ship)
                ship.DrawActive(spriteBatch);

            spriteBatch.End();
        }
    }
}
