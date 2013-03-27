using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveCISE_Game
{
    class GridTile
    {
        //To do: change from string?
        //'b' => blocked, 'c' => castle (CISE), 'o' => out of bounds
        public String blocked = "";
        public int row;
        public int col;
        public int fScore = 0;
        public int gScore = 0;
        public int hScore = 0;
        public GridTile parent = null;
        //This is for debugging, can remove later.
        public bool path = false; 

        public GridTile(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public void markAsBlocked(String blockedType = "b")
        {
            this.blocked = blockedType;
        }

        //returns true if f score is changed.
        // g is the gScore of the tile we want to move from.
        // dir the diagonal or orthogonal score
        //target is our target tile. (For manhattan #)
        public bool setScore(GridTile target, int g, int dir = 10)
        {
            if (fScore != 0)
            {
                int gTemp = g + dir;
                if (gTemp < gScore)
                {
                    this.gScore = gTemp;
                    this.fScore = this.hScore + gTemp;
                    return true;
                }
                else return false;
            }
            else
            {
                int verticalDist = Math.Abs(row - target.row);
                int horizontalDist = Math.Abs(col - target.col);
                int manhattanNum = verticalDist + horizontalDist;
                this.gScore = g + dir;
                this.hScore = manhattanNum;
                this.fScore = manhattanNum + gScore;
                return true;
            }
        }

        public void setParent(GridTile parent)
        {
            this.parent = parent;
        }

        public bool isBlocked()
        {
            if (this.blocked == "")
            {
                return false;
            }
            else return true;
        }

        public GridTile getParent()
        {
            return this.parent;
        }

        //Reset all the A* data, leave all else intact. 
        public void reset()
        {
            this.fScore = 0;
            this.gScore = 0;
            this.hScore = 0;
            this.path = false;
            this.parent = null;
        }

        //For debugging using console
        public override String ToString()
        {

#if DEBUG
            if (this.path) return "X";
#endif
            /* this will print the score on each tile. Will likely need later.
            if (fScore != 0)
            {
                return fScore.ToString();
            }
             */
            if (blocked == "")
            {
                return " ";
            }
            else return blocked;
        }
    }
}
