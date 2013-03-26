using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SaveCISE_Game
{
    class QuitGameAction : GameAction
    {
        Game game;
        public QuitGameAction(Game game)
        {
            this.game = game;
        }

        public override void doAction()
        {
            game.Exit();
        }
    }
}
