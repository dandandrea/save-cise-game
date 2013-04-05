using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveCISE_Game
{
    enum cellTypes {
        EMPTY,
        BLOCKED, //All-Purpose
        SLOWED,
        OUTOFBOUNDS,
        CASTLE
    }

    class GridCell
    {
        public cellTypes blocked = 0;
        public int row;
        public int col;
        public int fScore = 0;
        public int gScore = 0;
        public int hScore = 0;
        public GridCell parent = null;
        //This is for debugging, can remove later.
        public bool path = false; 

        public GridCell(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public void markAsBlocked(cellTypes blockedType = cellTypes.BLOCKED)
        {
            this.blocked = blockedType;
        }

        //returns true if f score is changed.
        // g is the gScore of the tile we want to move from.
        // dir the diagonal or orthogonal score
        //target is our target tile. (For manhattan #)
        public bool setScore(GridCell target, int g, int dir = 10)
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

        public void setParent(GridCell parent)
        {
            this.parent = parent;
        }

        public bool isBlocked()
        {
            if (this.blocked == 0)
            {
                return false;
            }
            else return true;
        }

        public GridCell getParent()
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
            switch(blocked) 
            {
                case cellTypes.BLOCKED:
                    return "b";
                case cellTypes.CASTLE:
                    return "c";
                case cellTypes.OUTOFBOUNDS:
                    return "O";
                default:
                    return " ";
                    
            }
        }
    }
}
