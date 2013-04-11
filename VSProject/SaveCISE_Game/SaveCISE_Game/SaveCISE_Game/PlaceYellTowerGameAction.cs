using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveCISE_Game
{
    class PlaceYellTowerGameAction : GameAction
    {
        public override void doAction()
        {
            GameController.beginPlacingTower(towerTypes.HARM);
        }
    }
}
