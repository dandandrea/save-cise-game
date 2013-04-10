using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SaveCISE_Game
{
    class WhitePanel : Actor
    {
        private const int drawX = 645;
        private const int drawY = 15;

       public WhitePanel() : base(new Sprite(ContentStore.getTexture("spr_whitePixel"), 150, 450, 1, 1))
        {
            this.panelTier = 1;
        }

       public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
       {
           
            Color alpha = new Color(20, 90, 90, 90);
            this.sprite.draw(sb, drawX, drawY, 1, 1, 1, alpha);
            
       }
    }
}
