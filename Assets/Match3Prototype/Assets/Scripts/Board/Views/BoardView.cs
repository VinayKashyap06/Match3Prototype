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
        private BgTileView[,] bgTiles;
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
      
    }
}