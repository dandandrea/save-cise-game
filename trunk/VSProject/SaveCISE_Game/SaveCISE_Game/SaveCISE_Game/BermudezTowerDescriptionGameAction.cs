using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveCISE_Game
{
    class BermudezTowerDescriptionGameAction : GameAction
    {
        public override void doAction()
        {
            WhitePanel.caption = towerTypes.BERMUDEZ;
        }
    }
}
