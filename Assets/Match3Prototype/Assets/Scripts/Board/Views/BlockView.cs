using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BlockView : MonoBehaviour
    {
        public BlockEnum blockEnum;
        private Vector2 startPosition;
        private Vector2 finalPosition;
        private Camera cam;
        private BoardController boardController;
        float angle;
        private int row;
        private int column;

        private void Start()
        {
            cam = Camera.main;
        }
        public void SetBoardViewRef(BoardController boardController)
        {
            this.boardController = boardController;
        }
        private void OnMouseDown()
        {
            startPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        private void OnMouseUp()
        {
            finalPosition = cam.ScreenToWorldPoint(Input.mousePosition);
            CalculateSwipe();
        }

        private void CalculateSwipe()
        {
            angle=Mathf.Atan2(finalPosition.y - startPosition.y, finalPosition.x - startPosition.x)* 180/Mathf.PI;
            Debug.Log(angle);
            ShuffleBlocks();
        }
        private void ShuffleBlocks()
        {
            if(angle>=-45f && angle <= 45)
            {
                //right swipe
                Debug.Log("<color=red>row "+row +" column"+ column+" </color>");
                boardController.MoveRight(row, column);
            }
            else if (angle>45 && angle<=135)
            {
                //up swipe
                Debug.Log("up");
                boardController.MoveUp(row, column);
                
            }
            else if (angle>=135 || angle<=-135)
            {
                //left swipe
                Debug.Log("left");
                boardController.MoveLeft(row, column);
            }
            else if (angle>-135 && angle<=-45)
            {
                Debug.Log("down");
                boardController.MoveDown(row, column);
                //down swipe
            }
        }
            
        public void ChangeParent(Transform pos)
        {
            this.gameObject.transform.SetParent( pos);
            this.gameObject.transform.localPosition = Vector3.zero;
        }

        public void SetRowAndColumn(int row, int column)
        {
            this.row = row;
            this.column = column;
        }
    }
}