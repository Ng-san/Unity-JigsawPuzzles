using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.IO;
public class PieceScript : MonoBehaviour
{ 
    public Vector3 RightPosition;
    public Vector3 CurrentPosition;
    public Vector3 FirstCurrentPos;

    public bool _isRightPosition;
    public bool _isFramePuzzleArea;
    public bool _isBoxPiecesArea;
    public bool Selected;
    public bool _isConnected;
    public int  _maxCount =0;

    public float RightDistance;
    public float LeftDistance;
    public float UpDistance;
    public float DownDistance;
    public GameObject right;
    public GameObject down;
    public GameObject left;
    public GameObject up;
    public int id;

    public float deltaX, deltaY;
    private Vector3 mousePosition;
    

    void Awake()
    {
        
        _isBoxPiecesArea = true;
        RightPosition   = transform.position;
        if(right != null)
        {
            RightDistance = right.transform.position.x - gameObject.transform.position.x;
        }
        if(left != null)
        {
            LeftDistance = left.transform.position.x - gameObject.transform.position.x;
        }
        if(up != null)
        {
            UpDistance = up.transform.position.y - gameObject.transform.position.y;
        }
        if(down != null)
        {
            DownDistance = down.transform.position.y - gameObject.transform.position.y;
        }
        
    }
    void Update()
    {
        if(!ControllerPuzzle.instance._isBlocked)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
            if(Input.GetMouseButtonDown(0))
            {
                if(_isBoxPiecesArea)
                {
                    FirstCurrentPos = CurrentPosition = transform.position;
                }
                if(_isFramePuzzleArea){
                    FirstCurrentPos = CurrentPosition = Vector3.zero;
                }
            }
        }
    }
    void Start()
    {
        GetComponent<SortingGroup>().sortingOrder = 0;
    }
    void OnMouseDown()
    { 
        if( !ControllerPuzzle.instance._isBlocked)
        {    
            if(_isFramePuzzleArea){
                ControllerPuzzle.instance.SelectedPiece = gameObject;
                // FirstCurrentPos = CurrentPosition = Vector3.zero;
                
                Selected = true;
            }
            if(!_isConnected)
            {
                deltaX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
                deltaY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;
            }
            else
            {
                deltaX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.parent.position.x;
                deltaY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.parent.position.y; 
            }
        }
    }
    void OnMouseDrag()
    {
        if( !ControllerPuzzle.instance._isBlocked && Selected)
        {
            if(!_isConnected)
            {
                transform.position = new Vector3(mousePosition.x - deltaX,mousePosition.y - deltaY,transform.position.z);
            }
            else
            {
                transform.parent.position = new Vector3(mousePosition.x - deltaX,mousePosition.y - deltaY,0);
            }
        }
    }
}
