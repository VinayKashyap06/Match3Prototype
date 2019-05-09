using System;
using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    public class BoardController
    {
        private BoardView currentBoardView;
        private List<BlockDataStruct> blockViewsList;
        private List<int> cummulativeProbabilities = new List<int>();
        private int width;
        private int height;
        public  BlockView[,] blockViews;
        public BoardController(int height, int width, BgTileView bgTileView)
        {
            this.width = width;
            this.height = height;
            blockViews = new BlockView[width, height];
            SpawnBoard(height, width, bgTileView);
        }

        private void SpawnBoard(int height, int width, BgTileView bgTileView)
        {
            GameObject board = new GameObject("Board");
            board.AddComponent<BoardView>();
            currentBoardView = board.GetComponent<BoardView>();
            currentBoardView.SetTilePrefab(bgTileView.gameObject);
            currentBoardView.SetWidthAndHeight(width, height);
            currentBoardView.SetBoardControllerRef(this);

        }
        public void SetBlockViews(List<BlockDataStruct> blockViews)
        {
            this.blockViewsList = blockViews;
            int previousProbability = 0;
            for (int i = 0; i < blockViews.Count; i++)
            {
                if (i - 1 != -1)
                {
                    previousProbability = cummulativeProbabilities[i - 1];
                }
                int newProb = previousProbability + blockViews[i].percentage;    
                cummulativeProbabilities.Add(newProb);
            }
           // Debug.Log(cummulativeProbabilities[cummulativeProbabilities.Count - 1]);
            SpawnBlocks();
        }

        private void SpawnBlocks()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {                    
                    int rand = UnityEngine.Random.Range(0, 100);
                    //Debug.Log("<color=blue>" + "i -- j"+i+j+ "</color>");
                      int k = 0;                    
                    while (k < cummulativeProbabilities.Count)
                    {
                        int minElement = 0;
                        if (k != 0)
                        {
                            minElement = cummulativeProbabilities[k - 1];
                        }                      
                        if (rand >= minElement && rand <= cummulativeProbabilities[k])
                        {
                            //Debug.Log(rand + "<-rand no.|| current probability->"  + cummulativeProbabilities[k]+" k value"+k+"min element "+minElement);
                            BlockView element = null;
                            int diff = cummulativeProbabilities[k] - minElement;                           
                            for (int ki = 0; ki < blockViewsList.Count; ki++)
                            {
                                if (blockViewsList[ki].percentage == diff)
                                {
                                    element = blockViewsList[ki].blockPrefab;
                                    break;
                                }
                            }
                            SpawnSingleBlock(element, i, j);
                            break;
                        }
                        k++;
                    }
                }
            }
        }

        private void SpawnSingleBlock(BlockView blockView,int row , int column)
        {
            currentBoardView.SpawnBlock(blockView,row,column);
        }

        public void MoveRight(int row, int column)
        {
            if (row + 1 >= width || column >= height)
            {
                Debug.Log(width + "///" + height);
                return;
            }
            //swap current row and column
            blockViews[row, column].SetRowAndColumn(row + 1, column);
            blockViews[row + 1, column].SetRowAndColumn(row, column);
            //swap parent
            Transform temPosA = blockViews[row, column].transform.parent;
            Transform temPosB = blockViews[row + 1, column].transform.parent;
            blockViews[row, column].ChangeParent(temPosB);
            blockViews[row + 1, column].ChangeParent(temPosA);

            //swap in data structure
            BlockView temp;
            temp = blockViews[row + 1, column];
            blockViews[row + 1, column] = blockViews[row, column];
            blockViews[row, column] = temp;
        }
        public void MoveLeft(int row, int column)
        {
            if (row - 1 <0 || column >= height)
            {
                Debug.Log(width + "///" + height);
                return;
            }
            //swap current row and column
            blockViews[row, column].SetRowAndColumn(row - 1, column);
            blockViews[row - 1, column].SetRowAndColumn(row, column);
            //swap parent
            Transform temPosA = blockViews[row, column].transform.parent;
            Transform temPosB = blockViews[row - 1, column].transform.parent;
            blockViews[row, column].ChangeParent(temPosB);
            blockViews[row - 1, column].ChangeParent(temPosA);

            //swap in data structure
            BlockView temp;
            temp = blockViews[row - 1, column];
            blockViews[row - 1, column] = blockViews[row, column];
            blockViews[row, column] = temp;
        }
        public void MoveUp(int row, int column)
        {
            if (column +1>= height)
            {
                Debug.Log(width + "///" + height);
                return;
            }
            //swap current row and column
            blockViews[row, column].SetRowAndColumn(row, column+1);
            blockViews[row, column+1].SetRowAndColumn(row, column);
            //swap parent
            Transform temPosA = blockViews[row, column].transform.parent;
            Transform temPosB = blockViews[row, column+1].transform.parent;
            blockViews[row, column].ChangeParent(temPosB);
            blockViews[row, column+1].ChangeParent(temPosA);

            //swap in data structure
            BlockView temp;
            temp = blockViews[row, column+1];
            blockViews[row, column+1] = blockViews[row, column];
            blockViews[row, column] = temp;
        }
        public void MoveDown(int row, int column)
        {
            if (column -1<0)
            {
                Debug.Log(width + "///" + height);
                return;
            }
            //swap current row and column
            blockViews[row, column].SetRowAndColumn(row, column-1);
            blockViews[row, column-1].SetRowAndColumn(row, column);
            //swap parent
            Transform temPosA = blockViews[row, column].transform.parent;
            Transform temPosB = blockViews[row, column-1].transform.parent;
            blockViews[row, column].ChangeParent(temPosB);
            blockViews[row, column-1].ChangeParent(temPosA);

            //swap in data structure
            BlockView temp;
            temp = blockViews[row, column-1];
            blockViews[row, column-1] = blockViews[row, column];
            blockViews[row, column] = temp;
        }

        public void FindMatch()
        {
            //BlockView tempView1, tempView2;
            ////if()
        }
    }
}
