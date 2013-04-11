using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveCISE_Game
{
    class PlaceSlowTowerGameAction : GameAction
    {
        public override void doAction()
        {
            GameController.beginPlacingTower(towerTypes.SLOW);
        }
    }
}
