using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    public class BoardView : MonoBehaviour
    {
        private int width = 0;
        private int height = 0;
        private GameObject tilePrefab;
        public BgTileView[,] bgTiles;
        //private BlockView[,] blockViews;
        private BoardController boardController;
        
        public void SetTilePrefab(GameObject tilePrefab)
        {
            this.tilePrefab = tilePrefab;
        }
        public void SetWidthAndHeight(int width, int height)
        {
            this.width = width;
            this.height = height;

            SetupBoard();
        }

        private void SetupBoard()
        {
            bgTiles = new BgTileView[width,height];
            //blockViews = new BlockView[width,height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Vector3 pos = new Vector3(i, j,0);
                    GameObject bgTile=Instantiate(tilePrefab, pos, Quaternion.identity) as GameObject;                    
                   // Debug.Log("<color=red> position</color>"+pos,bgTile);
                    bgTile.transform.SetParent(this.transform);
                    bgTiles[i,j] = bgTile.GetComponent<BgTileView>();
                }
            }
        }

        public void SetBoardControllerRef(BoardController boardController)
        {
            this.boardController = boardController;
        }

        public void SpawnBlock(BlockView blockView,int row, int column)
        {            
            GameObject block = Instantiate(blockView.gameObject, new Vector3(0,5,0), Quaternion.identity)as GameObject;
            iTween.MoveTo(block, bgTiles[row, column].transform.position, 1f);
            block.transform.SetParent(bgTiles[row, column].transform);         
            boardController.blockViews[row, column] = block.GetComponent<BlockView>();
            boardController.blockViews[row, column].SetBoardViewRef(boardController);
            boardController.blockViews[row, column].SetRowAndColumn(row, column);
        }
        public void SwapBoards(int row1,int column1, int row2, int column2) 
        {
            //swap positions
            Vector3 tempPos = Vector3.zero;
            tempPos=bgTiles[row1, column1].transform.position;
             iTween.MoveTo(bgTiles[row1, column1].gameObject, bgTiles[row2, column2].transform.position,1);
             iTween.MoveTo(bgTiles[row2, column2].gameObject, tempPos,1);
            //bgTiles[row2, column2].transform.position = tempPos;

            //swap in data structure
            BgTileView temp;
            temp = bgTiles[row2, column2];
            bgTiles[row2, column2] = bgTiles[row1, column1];
            bgTiles[row1, column1] = temp;
        } 
    }
}