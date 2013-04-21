using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveCISE_Game
{
    class ReplayGameAction : GameAction
    {
        Stage stage;
        int index;
        Scene scene;
        public ReplayGameAction(Stage stage, int index, Scene scene)
        {
            this.stage = stage;
            this.index = index;
            this.scene = scene;
        }
        public override void doAction()
        {
            GameController.reset();
            stage.removeScene(scene);
            scene = GameController.getGameScene();
            stage.insertScene(index, scene);
            stage.gotoScene(0);
        }
    }
}
