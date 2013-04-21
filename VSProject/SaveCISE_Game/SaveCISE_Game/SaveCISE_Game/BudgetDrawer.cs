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
        int maxBarHeight = 280;
        int baseY = 300;
        Sprite cover;
        SpriteFont font;
        public BudgetDrawer() : base(new Sprite(ContentStore.getTexture("Copy of spr_whitePixel")))
        {
            this.width = 30;
            this.x = 607;
            this.y = 10;
            this.cover = new Sprite(ContentStore.getTexture("Copy of spr_healthBarCover"));
            font = ContentStore.getFont("font_budget");
        }

        public override void draw(SpriteBatch sb)
        {
            base.draw(sb, Color.LimeGreen);
            cover.draw(sb, this.x, 15, 0);
            sb.Begin();
            sb.DrawString(font, "$" + GameController.getBudget(), new Vector2(x-20,y-20), Color.Lime);
            sb.End();
        }

        public override void update()
        {
            this.height = (int)(((float)GameController.getBudget() / GameController.MAX_BUDGET)*this.maxBarHeight);
            this.y = baseY - height;
        }
    }
}
