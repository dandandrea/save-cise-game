using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveCISE_Game
{
    class DavisTowerDescriptionGameAction : GameAction
    {
        public override void doAction()
        {
            WhitePanel.caption = towerTypes.DAVIS;
        }
    }
}
