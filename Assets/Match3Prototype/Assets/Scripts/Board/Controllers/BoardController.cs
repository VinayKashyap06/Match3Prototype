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
        //private Dictionary<int, int> elementsToDestroy = new Dictionary<int, int>();
        public BlockView[,] blockViews;
        public BlockView[,] elementsToDestroy;
        private bool hasNewBlocksSpawned = true;
        private bool canMatch = false;
        private bool checkDeadlock = true;

        public BoardController(int height, int width, BgTileView bgTileView)
        {
            this.width = width;
            this.height = height;
            blockViews = new BlockView[width, height];
            elementsToDestroy = new BlockView[width, height];
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
            //Debug.Log("spawn blocks called");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (blockViews[i, j] != null)
                    {
                        blockViews[i, j].DestroyView();
                        blockViews[i, j] = null;
                    }
                    SpawnRandomBlocks(i, j);
                }
            }
            //FindMatchAtStart();
        }

        private void SpawnRandomBlocks(int i, int j)
        {
            int rand = UnityEngine.Random.Range(0, 100);
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

        private void SpawnSingleBlock(BlockView blockView, int row, int column)
        {
            currentBoardView.SpawnBlock(blockView, row, column);
        }

        public void MoveRight(int row, int column)
        {
            if (row + 1 >= width || column >= height)
            {
              //  Debug.Log(width + "///" + height);
                return;
            }
            Swap(row, column, row + 1, column);
            bool c1 = CheckForMatchPresence(row, column);
            bool c2 = CheckForMatchPresence(row+1, column);
            if (c1 || c2)
            {
                Debug.Log("match found right");
            }
            else
            {
                //swapBack
                Swap(row + 1, column, row, column);
            }
        }
        public void MoveLeft(int row, int column)
        {
            if (row - 1 < 0 || column >= height)
            {
                //Debug.Log(width + "///" + height);
                return;
            }
            Swap(row, column, row - 1, column);
            bool c1 = CheckForMatchPresence(row, column);
            bool c2 = CheckForMatchPresence(row-1, column);
            if (c1 || c2)
            {
                Debug.Log("match found left");
            }
            else
            {
                //swapBack
                Swap(row - 1, column, row, column);
            }

        }
        public void MoveUp(int row, int column)
        {
            if (column + 1 >= height)
            {
               // Debug.Log(width + "///" + height);
                return;
            }
            Swap(row, column, row, column + 1);
            bool c1 = CheckForMatchPresence(row, column);
            bool c2 = CheckForMatchPresence(row, column + 1);
            if (c1 || c2)
            {
                Debug.Log("match found up");
               // DestroyMatchedElements();
            }
            else
            {
                //swapBack
                Swap(row, column + 1, row, column);
            }

        }
        public void MoveDown(int row, int column)
        {
            if (column - 1 < 0)
            {
               // Debug.Log(width + "///" + height);
                return;
            }
            Swap(row, column, row, column - 1);
            bool c1 = CheckForMatchPresence(row, column);
            bool c2 = CheckForMatchPresence(row, column - 1);
            if (c1 || c2)
            {
                Debug.Log("match found down");
            }
            else
            {
                //swapBack
                Swap(row, column - 1, row, column);
            }

        }
        private void Swap(int row1, int column1, int row2, int column2)
        {
            blockViews[row1, column1].SetRowAndColumn(row2, column2);
            blockViews[row2, column2].SetRowAndColumn(row1, column1);
            //swap parent
            Transform temPosA = blockViews[row1, column1].transform.parent;
            Transform temPosB = blockViews[row2, column2].transform.parent;
            blockViews[row1, column1].ChangeParent(temPosB);
            blockViews[row2, column2].ChangeParent(temPosA);

            //swap in data structure
            BlockView temp;
            temp = blockViews[row2, column2];
            blockViews[row2, column2] = blockViews[row1, column1];
            blockViews[row1, column1] = temp;
        }
        public void FindMatchAtStart()
        {
            hasNewBlocksSpawned = false;
            // Debug.Log("finding matches");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    CheckForRightPresence(i, j);
                    CheckForUpPresence(i, j);
                }
            }
            DestroyMatchedElements();
            checkDeadlock = CheckForPossibleMatches();
            if (!checkDeadlock)
            {
                //reshuffle
                Debug.Log("<color=blue>reshuffle</color>");
                SpawnBlocks();
            }

        }

        private bool CheckForPossibleMatches()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (CheckForPossibleRightMatch(i, j)||CheckForPossibleLeftMatch(i,j)||CheckForPossibleDownMatch(i,j)||CheckForPossibleUpMatch(i,j))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CheckForPossibleRightMatch(int row, int column)
        {
            if (row + 1 >= width)
            {
                return false;
            }
            if (!IsPair(row, column, row + 1, column))
            {
                return false;
            }
            else
            {
                if (row + 2 >= width)
                {
                    return false;
                }
                return CheckForOtherThreeDirections(row + 2, column, blockViews[row, column].blockEnum, "left");
            }
        }
        private bool CheckForPossibleLeftMatch(int row, int column)
        {
            if (row - 1 <0)
            {
                return false;
            }
            if (!IsPair(row, column, row -1, column))
            {
                return false;
            }
            else
            {
                if (row - 2 <0)
                {
                    return false;
                }
                return CheckForOtherThreeDirections(row - 2, column, blockViews[row, column].blockEnum, "right");
            }
        }
        private bool CheckForPossibleUpMatch(int row, int column)
        {
            if (column + 1 >= height)
            {
                return false;
            }
            if (!IsPair(row, column, row, column+1))
            {
                return false;
            }
            else
            {
                if (column + 2 >= height)
                {
                    return false;
                }
                return CheckForOtherThreeDirections(row, column+2, blockViews[row, column].blockEnum, "down");
            }
        }
        private bool CheckForPossibleDownMatch(int row, int column)
        {
            if (column - 1 <0)
            {
                return false;
            }
            if (!IsPair(row, column, row, column-1))
            {
                return false;
            }
            else
            {
                if (column-2 <0)
                {
                    return false;
                }
                return CheckForOtherThreeDirections(row, column-2, blockViews[row, column].blockEnum, "up");
            }
        }

        private bool CheckForOtherThreeDirections(int row, int column, BlockEnum blockEnum, string directionToIgnore)
        {
            if (row + 1 < width)
            {           
                if (blockViews[row + 1, column].blockEnum == blockEnum && directionToIgnore != "right")
                {
                    return true;
                }
            }
            if (row - 1 >= 0)
            {          
                if (blockViews[row - 1, column].blockEnum == blockEnum && directionToIgnore != "left")
                {
                    return true;
                }
            }
            if (column + 1 < height)
            {        
                if (blockViews[row, column + 1].blockEnum == blockEnum && directionToIgnore != "up")
                {
                    return true;
                }
            }
            if (column - 1 >= 0)
            {           
                if (blockViews[row, column - 1].blockEnum == blockEnum && directionToIgnore != "down")
                {
                    return true;
                }
            }
             
            return false;

        }
        private void DestroyMatchedElements()
        {
            for (int k = 0; k < width; k++)
            {
                for (int l = 0; l < height; l++)
                {
                    if (elementsToDestroy[k, l] != null)
                    {
                        elementsToDestroy[k, l].DestroyView();
                        elementsToDestroy[k, l] = null;
                        blockViews[k, l] = null;
                        Debug.Log("element deleted at row "+k +"column "+l);
                        SpawnRandomBlocks(k,l);
                        hasNewBlocksSpawned = true;
                    }
                }
            }
            //if (hasNewBlocksSpawned)
            //{
            //    FindMatchAtStart();
            //}
        }
        public bool CheckForMatchPresence(int row, int column)
        {
            bool c1= IsFirstElement(row, column);
            bool c2= IsMiddleElement(row, column);
            bool c3 = IsLastElement(row, column);
            if (c1||c2 ||c3)
            {
                DestroyMatchedElements();
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsFirstElement(int row, int column)
        {  
            if (IsPair(row, column, row, column + 1) && IsPair(row, column, row, column + 2))
            {
                AddElementsToDestroy(row, column);
                AddElementsToDestroy(row, column + 1);
                AddElementsToDestroy(row, column + 2);
                //  CheckForContinousElements(row,column,row, column+3);
                //int concurrentColumn = column + 3;
                //if (concurrentColumn < height)
                //{
                //    while (IsPair(row, column, row, concurrentColumn))
                //    {
                //        Debug.Log("While Is 1st element adding row" + row + " column" + concurrentColumn);
                //        AddElementsToDestroy(row, concurrentColumn);
                //        concurrentColumn++;
                //        if (concurrentColumn >= height)
                //        {
                //            break;
                //        }
                //    }
                //}
                return true;
            }
           if (IsPair(row, column, row + 1, column) && IsPair(row, column, row + 2, column))
            {
                AddElementsToDestroy(row, column);
                AddElementsToDestroy(row + 1, column);
                AddElementsToDestroy(row + 2, column);             
                return true;
            }
            else
            {
            //    Debug.Log("first element check return type false");
                return false;
            }
        }

        private void CheckForContinousElements(int row,int column,int row2, int column2)
        {
            if (!IsInBounds(row2, column2))
                return;
            int x = row2;
            int y = column2;
            while(IsPair(row,column,x,y))
            {
                AddElementsToDestroy(x, y);
                
            }
        }

        private bool IsInBounds(int row, int column)
        {
            if (row < 0 || row >= width || column < 0 || column >= height)
                return false;
            return true;
        }

        private bool IsLastElement(int row, int column)
        {
           // Debug.Log("last element check");
            if (IsPair(row, column, row, column - 1) && IsPair(row, column, row, column - 2))
            {
                AddElementsToDestroy(row, column);
                AddElementsToDestroy(row, column - 1);
                AddElementsToDestroy(row, column - 2);
              //  Debug.Log("last element check return type true");
                return true;
            }
            if (IsPair(row, column, row - 1, column) && IsPair(row, column, row - 2, column))
            {
                AddElementsToDestroy(row, column);
                AddElementsToDestroy(row - 1, column);
                AddElementsToDestroy(row - 2, column);
               // Debug.Log("last element check return type true");
                return true;
            }
            else
            {
               // Debug.Log("last element check return type false");
                return false;
            }
        }
        private bool IsMiddleElement(int row, int column)
        {         
            if (IsPair(row, column, row, column - 1) && IsPair(row, column, row, column + 1))
            {
                AddElementsToDestroy(row, column);
                AddElementsToDestroy(row, column - 1);
                AddElementsToDestroy(row, column + 1);
              //  Debug.Log("middle element check return type true");
                return true;
            }
            if (IsPair(row, column, row - 1, column) && IsPair(row, column, row + 1, column))
            {
                AddElementsToDestroy(row, column);
                AddElementsToDestroy(row - 1, column);
                AddElementsToDestroy(row + 1, column);
              //  Debug.Log("middle element check return type true");
                return true;
            }
            else
            {
              //  Debug.Log("middle element check return type false");
                return false;
            }
        }
        private void CheckForUpPresence(int row, int column)
        {
            if (column + 1 >= height)
                return ;
            if (!IsPair(row, column, row, column + 1))
            {
                return ;
            }
            else
            {
                if (column + 2 >= height)
                    return ;
                if (IsPair(row, column + 1, row, column + 2))
                {
                    AddElementsToDestroy(row, column);
                    AddElementsToDestroy(row, column + 1);
                    AddElementsToDestroy(row, column + 2);
                    
                }              
            }
        }
        private void CheckForRightPresence(int row, int column)
        {
            //Debug.Log("checking right");
            if (row + 1 >= width)
            {
                return ;
            }
            if (!IsPair(row, column, row + 1, column))
            {
                return ;
            }
            else
            {
                if (row + 2 >= width)
                    return ;
                if (IsPair(row + 1, column, row + 2, column))
                {
                    AddElementsToDestroy(row, column);
                    AddElementsToDestroy(row + 1, column);
                    AddElementsToDestroy(row + 2, column);                 
                }
            }
        }
        private void AddElementsToDestroy(int row, int column)
        {
            elementsToDestroy[row, column] = blockViews[row, column];
        }
        private bool IsPair(int row1, int column1, int row2, int column2)
        {
            if (row2 >= width || column2 >= height || row2 < 0 || column2 < 0)
                return false;

            return (blockViews[row1, column1].blockEnum == blockViews[row2, column2].blockEnum);
        }
    }
}
