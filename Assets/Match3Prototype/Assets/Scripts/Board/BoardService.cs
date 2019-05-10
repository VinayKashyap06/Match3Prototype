using System;
using System.Collections.Generic;
using UnityEngine;
using Commons;

namespace Board
{
    public class BoardService
    {
        private BoardController boardController;
      

        public BoardService(BoardScriptableObject boardScriptableObject)
        {
            //Debug.Log("service created");
            boardController = new BoardController(boardScriptableObject.height,boardScriptableObject.width,boardScriptableObject.backgroundTileView);
            boardController.SetBlockViews(boardScriptableObject.blockList);
        }      
        
    }
}
