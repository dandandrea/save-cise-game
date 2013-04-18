using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SaveCISE_Game
{
    class WhitePanel : Actor
    {
        private const int drawX = 645;
        private const int drawY = 15;
        private const int panelWidth = 150;
        private const int panelHeight = 450;


       public WhitePanel() : base(new Sprite(ContentStore.getTexture("spr_whitePixel"), panelWidth, panelHeight, 1, 1))
        {
            this.panelTier = 1;
        }

       public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
       {
           
            Color alpha = new Color(10, 10, 10, 10);
            this.sprite.draw(sb, drawX, drawY, 1, 1, 1, alpha);

            SpriteFont font1 = ContentStore.getFont("font_fontName1");

            sb.Begin();
            //"Enthusiasm"
            string output = "Enthusiasm";
            Vector2 FontOrigin = font1.MeasureString(output) / 2;
            Vector2 FontPos = new Vector2(720.0f, 250.0f);
            sb.DrawString(font1, output, FontPos, Color.Black,
                          0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            
           //Enthusiasm value
            int value = GameController.enthusiasm;
            FontOrigin = font1.MeasureString(value.ToString()) / 2;
            FontPos = new Vector2(720.0f, 280.0f);
            sb.DrawString(font1, "$" + value.ToString(), FontPos, Color.Black,
                          0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);


            output = "Time to Next Wave";
            FontOrigin = font1.MeasureString(output) / 2;
            FontPos = new Vector2(720.0f, 390.0f);
            sb.DrawString(font1, output, FontPos, Color.Black,
                          0, FontOrigin, 0.8f, SpriteEffects.None, 0.5f);

            // "Time to Next Wave" value
            value = GameController.nextWaveCountdown;
            FontOrigin = font1.MeasureString(value.ToString()) / 2;
            FontPos = new Vector2(695.0f, 420.0f);
            sb.DrawString(font1, value + " secs", FontPos, Color.Black,
                          0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);

            sb.End();

            
       }
    }
}
