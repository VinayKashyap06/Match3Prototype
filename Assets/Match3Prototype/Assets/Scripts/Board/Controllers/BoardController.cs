using System;
using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    public class BoardController
    {
        private BoardView currentBoardView;
        public BoardController(int height, int width,BgTileView bgTileView)
        {
            SpawnBoard(height,width,bgTileView);
        }

        private void SpawnBoard(int height, int width,BgTileView bgTileView)
        {
            GameObject board = new GameObject("Board");
            board.AddComponent<BoardView>();
            currentBoardView=board.GetComponent<BoardView>();
            currentBoardView.SetTilePrefab(bgTileView.gameObject);
            currentBoardView.SetWidthAndHeight(width,height);

        }
    }
}
