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
        public BlockView[,] blockViews;
        public BlockView[,] elementsToDestroy;
        private bool hasNewBlocksSpawned = true;
        private bool canMatch = false;
        private bool checkDeadlock = true;
        private IndexListStruct upperElementIndex;

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
            FindMatchAtStart();
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
            if (row + 1 >= width)
            {               
                return;
            }
            Swap(row, column, row + 1, column);
            bool c1 = CheckForMatchPresence(row, column);
            bool c2 = CheckForMatchPresence(row + 1, column);
            if (!c1 && !c2)
            {
                Swap(row + 1, column, row, column);
            }
        }
        public void MoveLeft(int row, int column)
        {
            if (row - 1 < 0)
            {                
                return;
            }
            Swap(row, column, row - 1, column);
            bool c1 = CheckForMatchPresence(row, column);
            bool c2 = CheckForMatchPresence(row - 1, column);
            if (!c1 && !c2)
            {
                //swapBack
                Swap(row - 1, column, row, column);
            }

        }
        public void MoveUp(int row, int column)
        {
            if (column + 1 >= height)
            {             
                return;
            }
            Swap(row, column, row, column + 1);
            bool c1 = CheckForMatchPresence(row, column);
            bool c2 = CheckForMatchPresence(row, column + 1);
            if (!c1 && !c2)
            {
                //swapBack
                Swap(row, column + 1, row, column);
            }

        }
        public void MoveDown(int row, int column)
        {
            if (column - 1 < 0)
            {
                return;
            }
            Swap(row, column, row, column - 1);
            bool c1 = CheckForMatchPresence(row, column);
            bool c2 = CheckForMatchPresence(row, column - 1);
            if (!c1 && !c2)
            {
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
        public async void FindMatchAtStart()
        {
            await new WaitForSeconds(1);
            hasNewBlocksSpawned = false;
            // Debug.Log("finding matches");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    CheckForMatchPresence(i, j);
                }
            }
            await new WaitForSeconds(1f);
            DestroyMatchedElements();
            checkDeadlock = CheckForPossibleMatches();
            if (!checkDeadlock)
            {
                //reshuffle
               // Debug.Log("<color=blue>reshuffle</color>");
                SpawnBlocks();
            }

        }
        private bool CheckForPossibleMatches()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (CheckForPossibleRightMatch(i, j) || CheckForPossibleLeftMatch(i, j) || CheckForPossibleDownMatch(i, j) || CheckForPossibleUpMatch(i, j))
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
            if (row - 1 < 0)
            {
                return false;
            }
            if (!IsPair(row, column, row - 1, column))
            {
                return false;
            }
            else
            {
                if (row - 2 < 0)
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
            if (!IsPair(row, column, row, column + 1))
            {
                return false;
            }
            else
            {
                if (column + 2 >= height)
                {
                    return false;
                }
                return CheckForOtherThreeDirections(row, column + 2, blockViews[row, column].blockEnum, "down");
            }
        }
        private bool CheckForPossibleDownMatch(int row, int column)
        {
            if (column - 1 < 0)
            {
                return false;
            }
            if (!IsPair(row, column, row, column - 1))
            {
                return false;
            }
            else
            {
                if (column - 2 < 0)
                {
                    return false;
                }
                return CheckForOtherThreeDirections(row, column - 2, blockViews[row, column].blockEnum, "up");
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
            List<IndexListStruct> destroyedElement = new List<IndexListStruct>();
            for (int k = 0; k < width; k++)
            {
                for (int l = 0; l < height; l++)
                {
                    if (elementsToDestroy[k, l] != null)
                    {
                        elementsToDestroy[k, l].DestroyView();
                        elementsToDestroy[k, l] = null;
                        blockViews[k, l] = null;
                        SpawnRandomBlocks(k, l);
                        hasNewBlocksSpawned = true;
                    }
                }
            }
            if (hasNewBlocksSpawned)
            {
                FindMatchAtStart();
            }
        }    
        public bool CheckForMatchPresence(int row, int column)
        {
            List<IndexListStruct> positiveVerticalMatches;
            List<IndexListStruct> negativeVerticalMatches;
            List<IndexListStruct> negativeHorizontalMatches;
            List<IndexListStruct> positiveHorizontalMatches;
            positiveHorizontalMatches = CheckForMatchRecursive(row, column, "x", 1);
            positiveVerticalMatches = CheckForMatchRecursive(row, column, "y", 1);
            negativeVerticalMatches = CheckForMatchRecursive(row, column, "y", -1);
            negativeHorizontalMatches = CheckForMatchRecursive(row, column, "x", -1);
            bool isMatch = false;
            if ((negativeHorizontalMatches.Count <= 1 && positiveVerticalMatches.Count <= 1))
            {
                isMatch = false;
            }
            if ((negativeHorizontalMatches.Count <= 1 && negativeVerticalMatches.Count <= 1))
            {
                isMatch = false;
            }
            if ((positiveHorizontalMatches.Count <= 1 && negativeVerticalMatches.Count <= 1))
            {
                isMatch = false;
            }
            if ((positiveHorizontalMatches.Count <= 1 && positiveVerticalMatches.Count <= 1))
            {
                isMatch = false;
            }
            if ((negativeVerticalMatches.Count >= 1 && positiveVerticalMatches.Count >= 1))
            {
                isMatch = true;
                for (int i = 0; i < positiveVerticalMatches.Count; i++)
                {
                    AddElementsToDestroy(positiveVerticalMatches[i].row, positiveVerticalMatches[i].column);
                }
                for (int i = 0; i < negativeVerticalMatches.Count; i++)
                {
                    AddElementsToDestroy(negativeVerticalMatches[i].row, negativeVerticalMatches[i].column);
                }
            }
            if ((positiveHorizontalMatches.Count >= 1 && negativeHorizontalMatches.Count >= 1))
            {
                isMatch = true;
                for (int i = 0; i < positiveHorizontalMatches.Count; i++)
                {
                    AddElementsToDestroy(positiveHorizontalMatches[i].row, positiveHorizontalMatches[i].column);
                }
                for (int i = 0; i < negativeHorizontalMatches.Count; i++)
                {
                    AddElementsToDestroy(negativeHorizontalMatches[i].row, negativeHorizontalMatches[i].column);
                }

            }
            if ((positiveHorizontalMatches.Count >= 2 || negativeHorizontalMatches.Count >= 2))
            {
                isMatch = true;
                for (int i = 0; i < positiveHorizontalMatches.Count; i++)
                {
                    AddElementsToDestroy(positiveHorizontalMatches[i].row, positiveHorizontalMatches[i].column);
                }
                for (int i = 0; i < negativeHorizontalMatches.Count; i++)
                {
                    AddElementsToDestroy(negativeHorizontalMatches[i].row, negativeHorizontalMatches[i].column);
                }
            }
            if ((positiveVerticalMatches.Count >= 2 || negativeVerticalMatches.Count >= 2))
            {

                isMatch = true;
                for (int i = 0; i < positiveVerticalMatches.Count; i++)
                {
                    AddElementsToDestroy(positiveVerticalMatches[i].row, positiveVerticalMatches[i].column);
                }
                for (int i = 0; i < negativeVerticalMatches.Count; i++)
                {
                    AddElementsToDestroy(negativeVerticalMatches[i].row, negativeVerticalMatches[i].column);
                }
            }

            if (isMatch)
            { AddElementsToDestroy(row, column); }
            else
            { return isMatch; }

            DestroyMatchedElements();

            return isMatch;
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
        private List<IndexListStruct> CheckForMatchRecursive(int row, int column, string direction, int directionAxis)
        {
            int rowIterator = 0;
            int columnIterator = 0;
            List<IndexListStruct> listOfIndexes = new List<IndexListStruct>();
            List<IndexListStruct> recursiveResult = new List<IndexListStruct>();

            if (direction == "x" && directionAxis == 1)
            {
                rowIterator = row + 1;
                columnIterator = column;
            }
            else if (direction == "x" && directionAxis == -1)
            {
                rowIterator = row - 1;
                columnIterator = column;
            }
            else if (direction == "y" && directionAxis == +1)
            {

                rowIterator = row;
                columnIterator = column + 1;

            }
            else if (direction == "y" && directionAxis == -1)
            {
                rowIterator = row;
                columnIterator = column - 1;
            }

            if (IsPair(row, column, rowIterator, columnIterator))
            {
                IndexListStruct indexListStruct = new IndexListStruct();
                indexListStruct.row = rowIterator;
                indexListStruct.column = columnIterator;
                listOfIndexes.Add(indexListStruct);
                recursiveResult = CheckForMatchRecursive(rowIterator, columnIterator, direction, directionAxis);
                foreach (IndexListStruct item in recursiveResult)
                {
                    listOfIndexes.Add(item);
                }

            }
            else
            {
                return listOfIndexes;
            }

            return listOfIndexes;
        }

        
        /// Haven't Tested these functions/features will do it after wards.
        //private void SwapToEmptySpot(int row1, int column1, int row2, int column2)
        //{
        //    if (blockViews[row2, column2] == null)
        //    {
        //        return;
        //    }
        //    blockViews[row1, column1] = blockViews[row2, column2];
        //    blockViews[row1, column1].ChangeParent(currentBoardView.bgTiles[row2, column2].transform);
        //    blockViews[row2, column2].ChangeParent(currentBoardView.bgTiles[row1, column1].transform);
        //    blockViews[row2, column2] = null;

        //}
        //private BlockView GetUpperElement(int row, int column)
        //{
        //    int columnIterator = column + 1;
        //    BlockView blockView;
        //    if (columnIterator >= height)
        //    {
        //        return null;
        //    }
        //    if (blockViews[row, columnIterator] == null)
        //    {
        //        blockView = GetUpperElement(row, columnIterator);
        //    }
        //    else
        //    {
        //        blockView = blockViews[row, columnIterator];
        //        // blockViews[row, columnIterator] = null;                
        //    }
        //    upperElementIndex.row = row;
        //    upperElementIndex.column = columnIterator;
        //    return blockView;
        //}

        //for (int i = 0; i < width; i++)
        //{
        //    for (int j = 0; j < height; j++)
        //    {
        //        if (blockViews[i, j] == null)
        //        {
        //            blockViews[i, j] = GetUpperElement(i, j);
        //            if (blockViews[i, j] == null)
        //            {
        //                Debug.Log("upper block null" + i + "fffrfrfr" + j);
        //                SpawnRandomBlocks(i, j);
        //            }
        //            else
        //            {
        //                Debug.Log("swap at loaction : " + i + " row with" + upperElementIndex.row + " column " + j + "  column" + upperElementIndex.column);
        //                SwapToEmptySpot(i, j, upperElementIndex.row, upperElementIndex.column);
        //                SpawnRandomBlocks(upperElementIndex.row, upperElementIndex.column);
        //            }
        //            hasNewBlocksSpawned = true;
        //        }
        //    }
        //}

        //private void ReShuffle()
        //{
        //    BlockView[,] tempArr = new BlockView[width, height];

        //    int randWidth = UnityEngine.Random.Range(0, width);
        //    int prevWidth = randWidth;
        //    int randHeight = UnityEngine.Random.Range(0, height);
        //    int prevHeight = randHeight;
        //    int count = 0;
        //    while (count <= width * height)
        //    {
        //        randWidth = UnityEngine.Random.Range(0, width);
        //        randHeight = UnityEngine.Random.Range(0, height);
        //        if (tempArr[randWidth, randHeight] == blockViews[randWidth, randHeight])
        //        {
        //            continue;
        //        }
        //        tempArr[randWidth, randHeight] = blockViews[randWidth, randHeight];
        //        count++;
        //    }
        //    blockViews = tempArr;
        //}
    }
}
