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

        public void SpawnBlock(BlockView blockView,int row, int column)
        {            
            GameObject block = Instantiate(blockView.gameObject, bgTiles[row, column].transform.position, Quaternion.identity)as GameObject;
            block.transform.SetParent(bgTiles[row, column].transform);
        }
    }
}