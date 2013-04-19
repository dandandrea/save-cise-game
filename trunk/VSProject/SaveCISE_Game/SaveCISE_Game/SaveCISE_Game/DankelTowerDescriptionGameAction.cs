using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveCISE_Game
{
    class DankelTowerDescriptionGameAction : GameAction
    {
        public override void doAction()
        {
            WhitePanel.caption = towerTypes.DANKEL;
        }
    }
}
