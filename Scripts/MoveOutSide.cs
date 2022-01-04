using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class MoveOutSide
{
    public void BlockPieceMoveOutSide(GameObject SelectedPiece,List<GameObject> _BlockPieces,GameObject FramePuzzle)
    {
        if(SelectedPiece != null && SelectedPiece.GetComponent<PieceScript>()._isConnected)
                {
                    if(_BlockPieces.Count!=0){
                        float Xmin = _BlockPieces[0].transform.position.x;
                        float Ymin = _BlockPieces[0].transform.position.y;
                        float Xmax = _BlockPieces[0].transform.position.x;
                        float Ymax = _BlockPieces[0].transform.position.y;
                    
                        for(int k=0; k < _BlockPieces.Count; k++)
                        {
                            if(Xmin >= _BlockPieces[k].transform.position.x)
                            {
                                Xmin = _BlockPieces[k].transform.position.x;
                            }
                            if(Xmax <= _BlockPieces[k].transform.position.x)
                            {
                                Xmax = _BlockPieces[k].transform.position.x;
                            }
                            if(Ymin >= _BlockPieces[k].transform.position.y)
                            {
                                Ymin = _BlockPieces[k].transform.position.y;
                            }
                            if(Ymax <= _BlockPieces[k].transform.position.y)
                            {
                                Ymax = _BlockPieces[k].transform.position.y;
                            }
                        }
                        if(Ymax - FramePuzzle.transform.position.y >= 6.1f) 
                        {
                            float deltaY = Ymax - SelectedPiece.transform.parent.position.y;
                            SelectedPiece.transform.parent.position = new Vector2(SelectedPiece.transform.parent.position.x, FramePuzzle.transform.position.y + 5.9f - deltaY);
                            // checkoutside();
                        }
                        if(Ymin - FramePuzzle.transform.position.y <= -6.1f) 
                        {
                            float deltaY = Ymin - SelectedPiece.transform.parent.position.y;
                            SelectedPiece.transform.parent.position = new Vector2(SelectedPiece.transform.parent.position.x, FramePuzzle.transform.position.y - 5.9f - deltaY);
                            // checkoutside();
                        }
                        if(Xmax - FramePuzzle.transform.position.x >= 4.5f )
                        {
                            float deltaX = Xmax - SelectedPiece.transform.parent.position.x;
                            SelectedPiece.transform.parent.position = new Vector2(FramePuzzle.transform.position.x + 4.2f - deltaX, SelectedPiece.transform.parent.position.y);
                            // checkoutside();
                        }
                        if(Xmin - FramePuzzle.transform.position.x <= -4.5f )
                        {
                            float deltaX = Xmin - SelectedPiece.transform.parent.position.x;
                            SelectedPiece.transform.parent.position = new Vector2(FramePuzzle.transform.position.x - 4.2f - deltaX, SelectedPiece.transform.parent.position.y);
                            
                        }
                        foreach(Transform Piece in SelectedPiece.transform.parent.transform)
                        {
                            Piece.gameObject.GetComponent<PieceScript>()._isFramePuzzleArea = true;
                                        
                        }
                    }
                    
                } 
    }
}
