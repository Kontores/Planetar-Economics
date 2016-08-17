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
    class Ship
    {
        internal string name;
        internal int type;

        internal int xframe;

        internal bool active;

        internal ContentManager content;

        internal Texture2D ship_texture;
        internal Texture2D active_texture;
        internal Texture2D menu_texture;
        internal Texture2D menu_texture2;


        internal Rectangle ship_rectangle;
        internal Rectangle active_rectangle;
        internal Rectangle menu_rectangle;
        internal Rectangle menu_rectangle2;

        // parameters

        internal double cash;
        internal float speed;
        internal float size;

        internal Planet[] planetlist;

        internal Vector2 approach;
        internal int approach_index;
        internal int approach2_index;
        internal Vector2 approach_distance;

        internal Vector2 approach2;

        internal Vector2 position;


        internal int resource_index;

        internal double[] fuel_buy_price;
        internal double[] wheat_buy_price;
        internal double[] sulfur_buy_price;

        internal int expensive_fuel;
        internal int expensive_wheat;
        internal int expensive_sulfur;

        internal double[] fuel_sell_price;
        internal double[] wheat_sell_price;
        internal double[] sulfur_sell_price;

        internal int cheap_fuel;
        internal int cheap_wheat;
        internal int cheap_sulfur;

        internal int random;

        internal double[] revenue;

        internal int cargo;
        internal string cargo_name;
        internal int cargo_amount;
        internal int cargo_limit;

        internal bool purchase;
        internal bool lastmode;
        internal string purchase_name;

        internal SpriteFont Impact;

        internal float time;

        // angle
        internal double angle;

        internal MouseState lastmousestate;

        public Ship(ContentManager newcontent, int settype, string newname, string setcargo, Vector2 coordinates, Planet[] setplanetlist, double setcash)
        {
            content = newcontent;
            type = settype;

            if (type == 1)
            {
                ship_texture = content.Load<Texture2D>("ship_texture_128x128");
                menu_texture = content.Load<Texture2D>("ship_menu_512x192");
                speed = 2.5f;
                size = 0.2f;

            }
            else if (type == 2)
            {
                ship_texture = content.Load<Texture2D>("ship_2_texture_128x128");
                menu_texture = content.Load<Texture2D>("ship_2_menu_512x192");
                speed = 2.4f;
                size = 0.24f;
            }
            else if (type == 3)
            {
                ship_texture = content.Load<Texture2D>("ship_3_texture_128x128");
                menu_texture = content.Load<Texture2D>("ship_3_menu_512x192");
                speed = 2.6f;
                size = 0.22f;
            }

            menu_texture2 = content.Load<Texture2D>("ship_menu_512x384");
            menu_rectangle = new Rectangle(0, 0, 512, 192);
            menu_rectangle2 = new Rectangle(0, 0, 512, 384);
            name = newname;
            cash = setcash;
            planetlist = setplanetlist;          
            position = coordinates;
            Impact = content.Load<SpriteFont>("impact");
            cargo_limit = 10;
            active_rectangle = new Rectangle(0, 0, 128, 128);
            active_texture = content.Load<Texture2D>("lock_128x128");
            active = false;
            random = 1;
            cargo_name = setcargo;

            if (setcargo.Contains("wheat"))
                cargo = 0;
            else if (setcargo.Contains("fuel"))
                cargo = 1;
            else if (setcargo.Contains("sulfur"))
                cargo = 2;
        }

        public void Update(GameTime gameTime)
        {

            // choose operation

            if (cargo_amount <= 0)
            {
                cargo_amount = 0;
                purchase = true;
                purchase_name = "purchase";
            }
            
            else
            {
                purchase = false;
                purchase_name = "sell";
            }

            // check prices

            wheat_buy_price = new double[planetlist.Length]; 
            fuel_buy_price =  new double[planetlist.Length];
            sulfur_buy_price = new double[planetlist.Length];

            wheat_sell_price = new double[planetlist.Length];
            fuel_sell_price =  new double[planetlist.Length];
            sulfur_sell_price = new double[planetlist.Length];

            // purchase mode

            if (purchase == true)
            {
                for (int i = 0; i < planetlist.Length; i++)
                {
                    // checking the prices
                    
                    wheat_buy_price[i] = planetlist[i].resources_buy_price[0];
                    fuel_buy_price[i] = planetlist[i].resources_buy_price[1];
                    sulfur_buy_price[i] = planetlist[i].resources_buy_price[2];

                    wheat_sell_price[i] = planetlist[i].resources_sell_price[0];
                    fuel_sell_price[i] = planetlist[i].resources_sell_price[1];
                    sulfur_sell_price[i] = planetlist[i].resources_sell_price[2];
                }

                // getting the cheapest and most expensive prices

                // wheat

                for (int i = 0; i < wheat_sell_price.Length; i++)
                {
                    if (wheat_sell_price[i] == wheat_sell_price.Min())
                        cheap_wheat = i;
                }


                for (int i = 0; i < wheat_buy_price.Length; i++)
                {
                    if (wheat_buy_price[i] == wheat_buy_price.Max())
                        expensive_wheat = i;
                }

                // fuel

                for (int i = 0; i < fuel_sell_price.Length; i++)
                {
                    if (fuel_sell_price[i] == fuel_sell_price.Min())
                        cheap_fuel = i;
                }


                for (int i = 0; i < fuel_buy_price.Length; i++)
                {
                    if (fuel_buy_price[i] == fuel_buy_price.Max())
                        expensive_fuel = i;
                }

                // sulfur

                for (int i = 0; i < sulfur_sell_price.Length; i++)
                {
                    if (sulfur_sell_price[i] == sulfur_sell_price.Min())
                        cheap_sulfur = i;
                }


                for (int i = 0; i < sulfur_buy_price.Length; i++)
                {
                    if (sulfur_buy_price[i] == sulfur_buy_price.Max())
                        expensive_sulfur = i;
                }

                // calculating the probable revenue for each type of ware

                revenue = new double[planetlist[0].resources.Length];
                revenue[0] = planetlist[expensive_wheat].resources_sell_price[0] - planetlist[cheap_wheat].resources_buy_price[0];
                revenue[1] = planetlist[expensive_fuel].resources_sell_price[1] - planetlist[cheap_fuel].resources_buy_price[1];
                revenue[2] = planetlist[expensive_sulfur].resources_sell_price[2] - planetlist[cheap_sulfur].resources_buy_price[2];

                // setting rout
                                
                    if (cargo == 0)
                    {
                        approach_index = cheap_wheat;
                        approach = planetlist[cheap_wheat].planet_coordinates;
                        approach2 = planetlist[expensive_wheat].planet_coordinates;
                        approach2_index = expensive_wheat;
                    }

                    else if (cargo == 1)
                    {
                        approach_index = cheap_fuel;
                        approach = planetlist[cheap_fuel].planet_coordinates;
                        approach2 = planetlist[expensive_fuel].planet_coordinates;
                        approach2_index = expensive_fuel;
                    }
                    else if (cargo == 2)
                    {
                        approach_index = cheap_sulfur;
                        approach = planetlist[cheap_sulfur].planet_coordinates;
                        approach2 = planetlist[expensive_sulfur].planet_coordinates;
                        approach2_index = expensive_sulfur;
                    }                    
                


                // trading

                if (position == approach && cargo_amount < cargo_limit && planetlist[approach_index].resources_amount[cargo] > 0)
                {
                    planetlist[approach_index].resources_amount[cargo] -= (cargo_limit - cargo_amount);
                    cargo_amount += (cargo_limit - cargo_amount);
                    cash -= cargo_amount * planetlist[approach_index].resources_sell_price[cargo];
                    approach = approach2;
                    approach_index = approach2_index;
                }
            }

            // sell mode

            else
            {
                approach = planetlist[approach_index].planet_coordinates;
                // trading

                if (position == approach && cargo_amount > 0)
                {
                    planetlist[approach_index].resources_amount[cargo] += (cargo_amount);
                    cash += cargo_amount * planetlist[approach_index].resources_buy_price[cargo];
                    cargo_amount = 0;
                }
            }

            lastmode = purchase;



            approach_distance = approach - position;
            approach_distance.Normalize();
            angle = Math.Atan2(approach_distance.X, approach_distance.Y);

            // ship animation

            ship_rectangle = new Rectangle(128*xframe, 0, 128, 128);

            if (Math.Abs(position.X - approach.X) > 10 || Math.Abs(position.Y - approach.Y) > 10)
            {
                if (Math.Sin(angle) >= -0.90 && Math.Sin(angle) < -0.40)
                {
                    if (approach.Y <= position.Y)
                        xframe = 1;
                    else { xframe = 7; }
                }
                else if (Math.Sin(angle) >= -0.40 && Math.Sin(angle) < 0.40)
                    if (approach.Y <= position.Y)
                        xframe = 2;
                    else { xframe = 6; }

                else if (Math.Sin(angle) >= 0.40 && Math.Sin(angle) < 0.90)
                {
                    if (approach.Y <= position.Y)
                        xframe = 3;
                    else { xframe = 5; }
                }
                else if (Math.Sin(angle) >= 0.90 && Math.Sin(angle) < 1.35)
                    xframe = 4;
                else if (Math.Sin(angle) < -0.90 && Math.Sin(angle) >= -1.35)
                    xframe = 0;

            }
            else { xframe = 6; }
            
            //
       
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // ship movement

            if (time > 0.2)
            {
                if (Math.Abs(position.X - approach.X) > 10 || Math.Abs(position.Y - approach.Y) > 10)
                    position += approach_distance * speed;
               else { position = approach; }
                     
                time = 0;

               

                // planetlist[0].resources_amount[1]++;
            }

            var newmousestate = Mouse.GetState();

            if (newmousestate.LeftButton == ButtonState.Pressed &&
               lastmousestate.LeftButton == ButtonState.Released)
            {
                if (
               position.X - 64 <= newmousestate.X &&
               position.X + 64 >= newmousestate.X &&
               position.Y - 64 <= newmousestate.Y &&
               position.Y + 64 >= newmousestate.Y
                  )
                    active = true;



                else { active = false; }



            }

            lastmousestate = newmousestate;


        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ship_texture, position, ship_rectangle, Color.White, 0f, new Vector2(0, 0), size, SpriteEffects.None, 0f);

            if (active)
            spriteBatch.Draw(active_texture, new Vector2(position.X, position.Y), active_rectangle, Color.White, 0f, new Vector2(0, 0), size, SpriteEffects.None, 0f);

        }

        public void DrawActive(SpriteBatch spriteBatch)
        {
            if(active)
            {
                spriteBatch.Draw(menu_texture, new Vector2(8, 8), menu_rectangle, Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                spriteBatch.Draw(menu_texture2, new Vector2(8, 8), menu_rectangle2, Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);

                spriteBatch.DrawString(Impact, name, new Vector2(150, 24), Color.Lime);
                spriteBatch.DrawString(Impact, cargo_name+" transportation", new Vector2(40, 218), Color.Lime);
                spriteBatch.DrawString(Impact, "Cargo: "+cargo_name, new Vector2(26, 248), Color.Lime);
                spriteBatch.DrawString(Impact, "Amount:      " +Convert.ToString(cargo_amount), new Vector2(26, 266), Color.Lime);
                spriteBatch.DrawString(Impact, "Mode:  " + purchase_name, new Vector2(26, 284), Color.Lime);
                spriteBatch.DrawString(Impact, "Arrival: Planet " + Convert.ToString(approach_index + 1), new Vector2(26, 302), Color.Lime);
                spriteBatch.DrawString(Impact, "Finances: " + cash.ToString("0.00"), new Vector2(26, 320), Color.Lime);

                spriteBatch.DrawString(Impact, "ECONOMICS", new Vector2(360, 218), Color.Lime);
                spriteBatch.DrawString(Impact, "Best Sales", new Vector2(330, 236), Color.Lime);

                spriteBatch.DrawString(Impact, "Wheat:", new Vector2(330, 254), Color.Lime);
                spriteBatch.DrawString(Impact, planetlist[cheap_wheat].resources_sell_price[0].ToString("0.00"), new Vector2(390, 254), Color.Lime);
                spriteBatch.DrawString(Impact, "Planet " + Convert.ToString(cheap_wheat + 1), new Vector2(430, 254), Color.Lime);

                spriteBatch.DrawString(Impact, "Fuel:", new Vector2(330, 272), Color.Lime);
                spriteBatch.DrawString(Impact, planetlist[cheap_fuel].resources_sell_price[1].ToString("0.00"), new Vector2(390, 272), Color.Lime);
                spriteBatch.DrawString(Impact, "Planet " + Convert.ToString(cheap_fuel + 1), new Vector2(430, 272), Color.Lime);

                spriteBatch.DrawString(Impact, "Sulfur:", new Vector2(330, 290), Color.Lime);
                spriteBatch.DrawString(Impact, planetlist[cheap_sulfur].resources_sell_price[2].ToString("0.00"), new Vector2(390, 290), Color.Lime);
                spriteBatch.DrawString(Impact, "Planet " + Convert.ToString(cheap_sulfur + 1), new Vector2(430, 290), Color.Lime);

                spriteBatch.DrawString(Impact, "Best Buyers", new Vector2(330, 308), Color.Lime);

                spriteBatch.DrawString(Impact, "Wheat:", new Vector2(330, 326), Color.Lime);
                spriteBatch.DrawString(Impact, planetlist[expensive_wheat].resources_buy_price[0].ToString("0.00"), new Vector2(390, 326), Color.Lime);
                spriteBatch.DrawString(Impact, "Planet " + Convert.ToString(expensive_wheat + 1), new Vector2(430, 326), Color.Lime);


                spriteBatch.DrawString(Impact, "Fuel:", new Vector2(330, 344), Color.Lime);
                spriteBatch.DrawString(Impact, planetlist[expensive_fuel].resources_buy_price[1].ToString("0.00"), new Vector2(390, 344), Color.Lime);
                spriteBatch.DrawString(Impact, "Planet " + Convert.ToString(expensive_fuel + 1), new Vector2(430, 344), Color.Lime);

                spriteBatch.DrawString(Impact, "Sulfur:", new Vector2(330, 362), Color.Lime);
                spriteBatch.DrawString(Impact, planetlist[expensive_sulfur].resources_buy_price[2].ToString("0.00"), new Vector2(390, 362), Color.Lime);
                spriteBatch.DrawString(Impact, "Planet " + Convert.ToString(expensive_sulfur + 1), new Vector2(430, 362), Color.Lime);



            }
        }
    }
}
