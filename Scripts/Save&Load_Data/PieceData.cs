using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PieceData
{
    public bool isRightPosition;
    public bool isFramePuzzleArea;
    public bool isBoxPiecesArea;
    public bool isConnected;
    public int  maxCount;
    public int  Id;

    public float PiecePositionX;
    public float PiecePositionY;
    public float PiecePositionZ;
}