using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveCISE_Game
{
    class BlockTowerDescriptionGameAction : GameAction
    {
        public override void doAction()
        {
            WhitePanel.caption = towerTypes.BLOCK;
        }
    }
}
