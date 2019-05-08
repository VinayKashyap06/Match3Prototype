using System;
using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    public class BoardController
    {
        private BoardView currentBoardView;
        private List<BlockDataStruct> blockViews;
        private List<int> cummulativeProbabilities = new List<int>();
        private int width;
        private int height;
        public BoardController(int height, int width, BgTileView bgTileView)
        {
            this.width = width;
            this.height = height;
            SpawnBoard(height, width, bgTileView);
        }

        private void SpawnBoard(int height, int width, BgTileView bgTileView)
        {
            GameObject board = new GameObject("Board");
            board.AddComponent<BoardView>();
            currentBoardView = board.GetComponent<BoardView>();
            currentBoardView.SetTilePrefab(bgTileView.gameObject);
            currentBoardView.SetWidthAndHeight(width, height);

        }
        public void SetBlockViews(List<BlockDataStruct> blockViews)
        {
            this.blockViews = blockViews;
            for (int i = 0; i < blockViews.Count; i++)
            {
                int previousProbability = 0;
                if (i - 1 != -1)
                {
                    previousProbability = blockViews[i - 1].percentage;
                }
                int newProb = previousProbability + blockViews[i].percentage;
                Debug.Log("new probablity====" + newProb);
                cummulativeProbabilities.Add(newProb);
            }
            Debug.Log(cummulativeProbabilities.Count+ cummulativeProbabilities[cummulativeProbabilities.Count-1]);
            SpawnBlocks();
        }

        private void SpawnBlocks()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    
                    int rand = UnityEngine.Random.Range(-1, cummulativeProbabilities[cummulativeProbabilities.Count-1]);
                    Debug.Log("random number + <color=blue>" + rand + "i.j"+i+j+ "</color>");
                    int k = 0;
                    
                    while (k < cummulativeProbabilities.Count)
                    {
                        int minElement = 0;
                        if (k != 0)
                        {
                            minElement = cummulativeProbabilities[k - 1];
                        }
                        if (rand > minElement && rand <= cummulativeProbabilities[k])
                        {
                            Debug.Log(blockViews[k].blockPrefab);
                            SpawnSingleBlock(blockViews[k].blockPrefab,i,j);
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
    }
}
