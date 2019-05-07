using System;
using System.Collections.Generic;
using UnityEngine;
using Board;

namespace Commons
{
    public class GameService: SingletonBase<GameService>
    {
        public BoardScriptableObject boardScriptableObject;
        private BoardService boardService;
        protected override void OnInitialize()
        {
            base.OnInitialize();
            boardService = new BoardService(boardScriptableObject);            
        }

    }
}
