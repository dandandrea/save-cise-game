using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveCISE_Game
{
    class ChangeSceneGameAction : GameAction
    {
        Stage stage;
        int index;
        public ChangeSceneGameAction(Stage stage, int index)
        {
            this.stage = stage;
            this.index = index;
        }
        public override void doAction()
        {
            stage.gotoScene(index);
        }
    }
}
