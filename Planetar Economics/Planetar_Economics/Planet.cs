using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Planetar_Economics
{
    class Planet
    {
        internal string type;

        internal int xframe;
        internal int yframe;

        internal int orbitradius;
        internal float size;
        internal bool direction;

        internal int number;

        internal bool active;
            
        internal string product;      
        internal string required;

        internal int product_amount;
        internal int required_amount;

        internal double prod_cycle;

        internal ContentManager content;

        internal Texture2D planet_texture;
        internal Texture2D active_texture;
        internal Texture2D menu_texture;
        internal Texture2D surface_texture;
        internal Texture2D prod_cycle_texture;

        internal Rectangle planet_rectangle;
        internal Rectangle active_rectangle;
        internal Rectangle menu_rectangle;
        internal Rectangle surface_rectangle;
        internal Rectangle prod_cycle_rectangle;

        internal int cycle_frame;
        internal int resource_index;

        public string[] resources;
        public int[] resources_amount;
        public double[] resources_buy_price;
        public double[] resources_sell_price;

        internal SpriteFont Impact;

        internal float time;
        internal double cycle;
        internal double animation_cycle;
        
        // angle
        internal double a;
        internal double rspeed;

        public Vector2 planet_coordinates;

        internal MouseState lastmousestate;
      
       
        public Planet(ContentManager newcontent, string newtype, string newproduct,  float newsize, int radius, double newrspeed, double initangle, bool newdirection, int newnumber)
        {

            content = newcontent;

            type = newtype;

            if (type.Contains("nitro_oxygene"))
                planet_texture = content.Load<Texture2D>("planet_earth_128x128");
            else if (type.Contains("ammony"))
                planet_texture = content.Load<Texture2D>("planet_ammony_128x128");
            else if (type.Contains("carbon_dioxyde"))
                planet_texture = content.Load<Texture2D>("planet_carbon_128x128");
            else { planet_texture = content.Load<Texture2D>("planet_ice_128x128"); }

            orbitradius = radius;
            size = newsize;
            direction = newdirection;
            rspeed = newrspeed;
            a = initangle;
            
            active_texture = content.Load<Texture2D>("lock_128x128");
            menu_texture = content.Load<Texture2D>("planet_menu_512x384");
            prod_cycle_texture = content.Load<Texture2D>("prod_cycle_98x140");

            if (type.Contains("nitro_oxygene"))
                surface_texture = content.Load<Texture2D>("surface_oxygene_500x164");
            else if (type.Contains("ammony"))
                surface_texture = content.Load<Texture2D>("surface_ammony_500x164");
            else if (type.Contains("carbon_dioxyde"))
                surface_texture = content.Load<Texture2D>("surface_carbon_500x164");
            else { surface_texture = content.Load<Texture2D>("surface_carbon_500x164"); }

            product = newproduct;

            if (product.Contains("wheat"))
            {
                product_amount = 10;
                required = "sulfur";
                required_amount = 5;
                prod_cycle = 0.3;
            }
            else if (product.Contains("fuel"))
            {
                product_amount = 3;
                required = "wheat";
                required_amount = 10;
                prod_cycle = 0.5;
            }
            else if (product.Contains("sulfur"))
            {
                product_amount = 4;
                required = "fuel";
                required_amount = 2;
                prod_cycle = 0.2;
            }

            cycle_frame = 0;

            active_rectangle = new Rectangle(0, 0, 128, 128);
            menu_rectangle = new Rectangle(0, 0, 512, 384);
            surface_rectangle = new Rectangle(0, 0, 500, 164);
            

            number = newnumber;

            resources = new string[3];
            resources[0] = "wheat";
            resources[1] = "fuel";
            resources[2] = "sulfur";

            resources_amount = new int[3];

            resources_amount[0] = 50;
            resources_amount[1] = 42;
            resources_amount[2] = 64;

            resources_buy_price = new double[3];
            

            resources_sell_price = new double[3];

            
            
            Impact = content.Load<SpriteFont>("impact");

            active = false;      
        }

        public void Update(GameTime gameTime)
        {
            planet_rectangle = new Rectangle(128 * xframe, 128*yframe, 128, 128);

             time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            var newmousestate = Mouse.GetState();

            // start the production cycle

            prod_cycle_rectangle = new Rectangle(0, 14 * cycle_frame, 98, 14);
            this.Production(gameTime);

            // price dynamics

            /* each ware price calculates in relation with it's amount on the planet.
            There are different coefficients for each ware. Sell price calculates as
            the buy price with specific spread */

            // wheat

            if (resources_amount[0] > 0)
                resources_buy_price[0] = 384.5 / resources_amount[0];

            // fuel

            if (resources_amount[1] > 0)
                resources_buy_price[1] = 601.00 / resources_amount[1];

            // sulfur

            if (resources_amount[2] > 0)
                resources_buy_price[2] = 698.56 / resources_amount[2];

            resources_sell_price[0] = resources_buy_price[0] + 0.29;
            resources_sell_price[1] = resources_buy_price[1] + 0.30;
            resources_sell_price[2] = resources_buy_price[2] + 0.35;

            // prevent zero and less prices

            for (int i = 0; i < resources_buy_price.Length; i++)
                if (resources_buy_price[i] <= 0)
                    resources_buy_price[i] = 0.05;


            // setting the direction of movement (clockwise or not)

            if (direction)
            {
                planet_coordinates.X = Convert.ToSingle(455 + orbitradius * Math.Cos(a));
                planet_coordinates.Y = Convert.ToSingle(355 + orbitradius * Math.Sin(a));
            }
            else
            {
                planet_coordinates.X = Convert.ToSingle(455 + orbitradius * Math.Sin(a));
                planet_coordinates.Y = Convert.ToSingle(355 + orbitradius * Math.Cos(a));
            }

            // animation

            if (time >= 0.12f)
            {
                time = 0;
                xframe++;
                a+= rspeed;            
            }

            if (a > 359)
                a = 0;

            if (xframe > 5)
            {
                xframe = 0;
                yframe++;
            }

            if (yframe > 5)
                yframe = 0;


            if (newmousestate.LeftButton == ButtonState.Pressed &&
                lastmousestate.LeftButton == ButtonState.Released)
            { if (
             planet_coordinates.X - 64 <= newmousestate.X &&
             planet_coordinates.X + 64 >= newmousestate.X &&
             planet_coordinates.Y - 64 <= newmousestate.Y &&
             planet_coordinates.Y + 64 >= newmousestate.Y
                )
                    active = true;

             

                else { active = false; }

                
            
            }

            lastmousestate = newmousestate;
        }

        public void Production(GameTime gameTime)
        {
            for (int i = 0; i < resources.Length; i++)
                if (resources[i].Contains(Convert.ToString(required)))
                     resource_index = i;

            if (resources_amount[resource_index] >= required_amount)
            {
               
                animation_cycle += gameTime.ElapsedGameTime.TotalSeconds;

                if (animation_cycle >= prod_cycle*6)
                {
                    animation_cycle = 0;
                    cycle_frame++;
                }
                if (cycle_frame > 9)                           
                {
                    cycle_frame = 0;
                    resources_amount[resource_index] -= required_amount;
                    for (int p = 0; p < resources.Length; p++)
                        if (resources[p].Contains(Convert.ToString(product)))
                            resources_amount[p] += product_amount;
                }
            }
            else { cycle_frame = 0; }
        }
                        

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(planet_texture, planet_coordinates, planet_rectangle, Color.White, 0f, new Vector2(0, 0), size, SpriteEffects.None, 0f);

            spriteBatch.DrawString(Impact, Convert.ToString(number), new Vector2(planet_coordinates.X +57*size, planet_coordinates.Y + 128*size), Color.LimeGreen);

            if (active)
            spriteBatch.Draw(active_texture, new Vector2(planet_coordinates.X + 1, planet_coordinates.Y), active_rectangle, Color.White, 0f, new Vector2(0, 0), size, SpriteEffects.None, 0f);
                          
        }

        public void Drawmenu(SpriteBatch spriteBatch)
        {
            if (active)
            {
                spriteBatch.Draw(menu_texture, new Vector2(512, 384), menu_rectangle, Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                spriteBatch.Draw(surface_texture, new Vector2(518, 403), surface_rectangle, Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);

                spriteBatch.DrawString(Impact, "PLANET " + Convert.ToString(number), new Vector2(674, 581), Color.Lime);
                spriteBatch.DrawString(Impact, "PRODUCTION", new Vector2(531, 601), Color.Lime);
                spriteBatch.DrawString(Impact, product+":", new Vector2(527, 618), Color.Lime);
                spriteBatch.DrawString(Impact, Convert.ToString(product_amount), new Vector2(610, 618), Color.Lime);


                spriteBatch.DrawString(Impact, "REQUIRES", new Vector2(541, 638), Color.Lime);
                spriteBatch.DrawString(Impact, required+":", new Vector2(527, 655), Color.Lime);
                spriteBatch.DrawString(Impact, Convert.ToString(required_amount), new Vector2(610, 655), Color.Lime);

                spriteBatch.DrawString(Impact, "PROD. CYCLE", new Vector2(531, 700), Color.Lime);
                spriteBatch.DrawString(Impact, Convert.ToString(prod_cycle)+" min.", new Vector2(531, 715), Color.Lime);
                if(resources_amount[resource_index] >= required_amount)
                    spriteBatch.Draw(prod_cycle_texture, new Vector2(524, 740), prod_cycle_rectangle, Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                else
                {
                    spriteBatch.DrawString(Impact, "Not enough", new Vector2(523, 730), Color.Red);
                    spriteBatch.DrawString(Impact, "resources: "+Convert.ToString(resources[resource_index]), new Vector2(523, 742), Color.Red);
                }

                spriteBatch.DrawString(Impact, "RESOURCES", new Vector2(792, 611), Color.Lime);
                spriteBatch.DrawString(Impact, "Type", new Vector2(785, 645), Color.Lime);             
                spriteBatch.DrawString(Impact, resources[0], new Vector2(785, 665), Color.Lime);
                spriteBatch.DrawString(Impact, resources[1], new Vector2(785, 685), Color.Lime);
                spriteBatch.DrawString(Impact, resources[2], new Vector2(785, 705), Color.Lime);

                spriteBatch.DrawString(Impact, "Amount", new Vector2(845, 645), Color.Lime);
                spriteBatch.DrawString(Impact, Convert.ToString(resources_amount[0]), new Vector2(855, 665), Color.Lime);
                spriteBatch.DrawString(Impact, Convert.ToString(resources_amount[1]), new Vector2(855, 685), Color.Lime);
                spriteBatch.DrawString(Impact, Convert.ToString(resources_amount[2]), new Vector2(855, 705), Color.Lime);

                spriteBatch.DrawString(Impact, "Buy", new Vector2(915, 645), Color.Lime);
                spriteBatch.DrawString(Impact, resources_buy_price[0].ToString("0.00"), new Vector2(915, 665), Color.Lime);
                spriteBatch.DrawString(Impact, resources_buy_price[1].ToString("0.00"), new Vector2(915, 685), Color.Lime);
                spriteBatch.DrawString(Impact, resources_buy_price[2].ToString("0.00"), new Vector2(915, 705), Color.Lime);

                spriteBatch.DrawString(Impact, "Sell", new Vector2(965, 645), Color.Lime);
                spriteBatch.DrawString(Impact, resources_sell_price[0].ToString("0.00"), new Vector2(965, 665), Color.Lime);
                spriteBatch.DrawString(Impact, resources_sell_price[1].ToString("0.00"), new Vector2(965, 685), Color.Lime);
                spriteBatch.DrawString(Impact, resources_sell_price[2].ToString("0.00"), new Vector2(965, 705), Color.Lime);



            }
        }
    }
}
