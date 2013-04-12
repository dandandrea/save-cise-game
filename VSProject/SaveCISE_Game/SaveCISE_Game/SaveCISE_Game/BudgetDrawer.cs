using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SaveCISE_Game
{
    class BudgetDrawer : Actor
    {
        int maxBarHeight = 300;
        public BudgetDrawer() : base(new Sprite(ContentStore.getTexture("spr_whitePixel")))
        {
            this.width = 30;
            this.x = 600;
            this.y = 30;
        }

        public override void draw(SpriteBatch sb)
        {
            base.draw(sb, Color.Green);
        }

        public override void update()
        {
            this.height = (int)(((float)GameController.getBudget() / GameController.MAX_BUDGET)*this.maxBarHeight);
        }
    }
}
