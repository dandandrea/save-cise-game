﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SaveCISE_Game
{
    class Grid
    {
        private const int rows = GameController.GRID_HEIGHT+2;
        private const int cols = GameController.GRID_WIDTH+2;
        private List<GridCell> attackPoints;

        protected GridCell[,] grid;

        //Initialize the grid. Put any permanent obstacles here (such as out of bounds)
        public Grid()
        {
            grid = new GridCell[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    grid[i, j] = new GridCell(i, j);

                    if (j == 0 || j == cols-1 || i==0 || i==rows-1)
                    {
                        //Out of bounds
                        grid[i, j].markAsBlocked(cellTypes.OUTOFBOUNDS);
                    }
                }
            }
            
            //Set attack points
            attackPoints = new List<GridCell>();
            grid[10, 13].markAsBlocked(cellTypes.ATTACKPOINT);
            attackPoints.Add(grid[10,13]);
            grid[10, 14].markAsBlocked(cellTypes.ATTACKPOINT);
            attackPoints.Add(grid[10,14]);
            grid[11, 12].markAsBlocked(cellTypes.ATTACKPOINT);
            attackPoints.Add(grid[11, 12]);
            grid[12,12].markAsBlocked(cellTypes.ATTACKPOINT);
            attackPoints.Add(grid[12,12]);
            grid[13, 11].markAsBlocked(cellTypes.ATTACKPOINT);
            attackPoints.Add(grid[13, 11]);
            grid[14,11].markAsBlocked(cellTypes.ATTACKPOINT);
            attackPoints.Add(grid[14,11]);

            //CISE points
            grid[11, 13].markAsBlocked(cellTypes.CASTLE);
            grid[11, 14].markAsBlocked(cellTypes.CASTLE);
            grid[12, 13].markAsBlocked(cellTypes.CASTLE);
            grid[12, 14].markAsBlocked(cellTypes.CASTLE);
            grid[13, 12].markAsBlocked(cellTypes.CASTLE);
            grid[13, 13].markAsBlocked(cellTypes.CASTLE);
            grid[13, 14].markAsBlocked(cellTypes.CASTLE);
            grid[14, 12].markAsBlocked(cellTypes.CASTLE);
            grid[14, 13].markAsBlocked(cellTypes.CASTLE);
            grid[14, 14].markAsBlocked(cellTypes.CASTLE);
            
#if DEBUG 
            // Console.WriteLine(this);
#endif
        }

        private static Random r = new Random();
        public GridCell getRandomAttackPoint()
        {
            int rando = r.Next(0, attackPoints.Count);
            GridCell att = this.attackPoints.ElementAt(rando);
            return att;
        }

        //returns the surrounding tiles and also sets the appropriate scores.
        private List<GridCell> getSurroundingTiles(GridCell current, GridCell end, List<GridCell> closedList, List<GridCell> openList) 
        {
            //List of surrounding tiles to add to the openList
            List<GridCell> surroundingTiles = new List<GridCell>();

            //Iterate through the 8 surrounding tiles
            for(int i=-1; i <= 1; i++) 
            {
                for(int j=-1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;


                    //If diagonal movement, score differently.
                    int dir = 14;
                    if (i == 0 || j == 0)
                    {
                        dir = 10;
                    }

                    //Corner Cases
                    bool deny = false;
                    if (i == -1 && j == -1) //top left
                    {
                        if (grid[current.row, current.col - 1].isBlocked() || grid[current.row - 1, current.col].isBlocked())
                        {
                            deny = true;
                        }
                    }
                    else if (i == -1 && j == 1) //top right
                    {
                        if (grid[current.row - 1, current.col].isBlocked() || grid[current.row, current.col + 1].isBlocked())
                        {
                            deny = true;
                        }
                    }
                    else if (i == 1 && j == -1) //bottom left
                    {
                        if (grid[current.row, current.col - 1].isBlocked() || grid[current.row + 1, current.col].isBlocked())
                        {
                            deny = true;
                        }
                    }
                    else if (i == 1 && j == 1) //bottom right
                    {
                        if (grid[current.row + 1, current.col].isBlocked() || grid[current.row, current.col + 1].isBlocked())
                        {
                            deny = true;
                        }
                    }


                    GridCell nearTile = grid[current.row + i, current.col + j];

                    //To do: put a construct that will tax the dir variable if tile is a "slow down" or other. Must change isBlocked().

                    if (!nearTile.isBlocked() && !closedList.Contains(nearTile) && !deny)
                    {
                        if (openList.Contains(nearTile))
                        {
                            if (nearTile.setScore(end, current.gScore, dir))
                            {
                                nearTile.setParent(current);
                            }
                        }
                        else
                        {
                            nearTile.setScore(end, current.gScore, dir);
                            nearTile.setParent(current);
                            surroundingTiles.Add(nearTile);
                        }
                    }
                }

            }
            return surroundingTiles;
            
        }

        public void markTile(int row, int col) 
        {
            grid[row, col].markAsBlocked(cellTypes.BLOCKED);
        }

        private void markSurroundingTiles(int row, int col, cellTypes blockType)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    grid[row + i, col + j].markAsBlocked(blockType);
                }
            }

        }

        //A* algorithm
        public Stack<GridCell> astar(int row1, int col1, int row2, int col2)
        {
            
            List<GridCell> openList = new List<GridCell>();
            List<GridCell> closedList = new List<GridCell>();

            GridCell current = this.grid[row1, col1];
            GridCell end = this.grid[row2, col2];

            while (current != null && current!=end)
            {
                //check for nonLegal tiles (closedList or Blocked) and set parent to current. Also, add scores.
                List<GridCell> surroundingTiles = getSurroundingTiles(current, end, closedList, openList);
                openList.AddRange(surroundingTiles);
                closedList.Add(current);
                openList.Remove(current);
                //find lowest f score
                current = findLowestFscore(openList);
            }
            if (current == end)
            {
                //Path found

                Stack<GridCell> path = new Stack<GridCell>();
                //trace path
                while (current != null)
                {
                    path.Push(current);
#if DEBUG
                    //For debugging, drawing X's on the console grid
                    current.path = true;
#endif
                    current = current.parent;
                }
#if DEBUG
                // Console.WriteLine(this);
#endif
                this.resetGrid();
                return path;
            }
            else
            {
                //Path not found
                this.resetGrid();
                return null;
            }

        }

        private GridCell findLowestFscore(List<GridCell> list)
        {
            int f = 0;
            GridCell lowest = null;
            foreach (GridCell tile in list)
            {
                if (f == 0 || tile.fScore < f)
                {
                    f = tile.fScore;
                    lowest = tile;
                }
            }
            return lowest;
        }

        //Reset the grid for future A* use. Easier than copying (deep) grid each time.
        private void resetGrid()
        {
            for (int i = 1; i < rows - 1; i++)
            {
                for (int j = 1; j < cols - 1; j++)
                {
                    this.grid[i, j].reset();
                }
            }
        }

        //Draws a grid in console for debugging
        public override String ToString()
        {
            String print = "";
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {

                    print += "[" + grid[i, j] + "]";

                }
                print += "\n";
            }

            return print;

        }

        public bool isCellBlocked(int row, int col)
        {
            if (grid[row, col].blocked == cellTypes.ATTACKPOINT || grid[row, col].blocked == cellTypes.CASTLE)
            {
                return true;
            }
            else
            {
                return grid[row, col].isBlocked();
            }
        }
        //To do:
        //public void setBlock(int x, int y)
        //public void clearBlock(int x, int y)
        //bool checkNoCycles(int x, int y)


        internal void clearTile(int row, int col)
        {
            grid[row, col].markAsBlocked(cellTypes.EMPTY);
        }
    }
}
