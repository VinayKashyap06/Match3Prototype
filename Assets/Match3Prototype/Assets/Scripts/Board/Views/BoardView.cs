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
        // Start is called before the first frame update
        void Start()
        {

        }
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
                    Vector2 pos = new Vector2(i, j);
                    GameObject bgTile=Instantiate(tilePrefab, pos, Quaternion.identity) as GameObject;                    
                    bgTile.transform.SetParent(this.transform.parent);
                }
            }
        }
    }
}