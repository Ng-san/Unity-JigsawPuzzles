using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Rendering;
using GoogleMobileAds.Api;

public class ControllerPuzzle : MonoBehaviour
{
    private static ControllerPuzzle _instance;
    public static ControllerPuzzle instance
    {
        get{
            if(_instance == null){
                _instance = FindObjectOfType<ControllerPuzzle>();
            }
            return _instance;
        }
    }
    public GameObject confettiFx; 
    //Frame
    public  GameObject SelectedPiece;
    public  GameObject FramePuzzle;
    public  GameObject BoxPieces;
    public  GameObject[] _ArrayPiece;
    public  List<GameObject> _ListPieceInBoxArea;
    // public int Blank;
    private List<GameObject> _ChildObject;
    public  List<GameObject> _BlockPieces;

    //Level
    public  List<GameObject> ListLevel;
    public GameObject Container;
    public List<GameObject> listNewGameobject;
    public GameObject BigContainer;
    public List<GameObject> ParentObject;
    private GameObject PieceSelect;
    private GameObject Piece;
    private GameObject SavePiece;
    public  int CurrentLevel;
    public  int LevelUnlocked;
    public int  SaveCurrentIndex;
    public  string Url;
    float xMin;
    float xMax;
    private float xminswipe;
    private float xmaxswipe;

    //Swipe
    private  Vector3  MousePoint;
    private  Vector3 startTouchPos, endTouchPos;
    float startTime;
    private bool _isTouchArea;
    private int onthetop=25;
    // private bool _isRun;
    private Vector3 StartPiecesPosition, EndPiecesPositon;
    private float moveTime;
    private float moveDuration = 0.1f;
    public int _hasWon;
    public bool _isloadData;
    public bool _isBlocked;
    private bool check=false;
    public bool _isselected;
    private bool _isEmpty;
    float deltaXswipe;
    bool mouseInBoxPiece;

    // int Empty;

    private void Awake()
    {
        RemoteConfig.instance.FetchDataAsync(); 
        Admob.instance.RequestBanner();
        float h = ControllerUI.instance.canvas.GetComponent<RectTransform>().rect.height;
        float w = ControllerUI.instance.canvas.GetComponent<RectTransform>().rect.width;
        ControllerUI.instance.rectTransform.sizeDelta = new Vector2(w*10,h);
        for(int i=0; i <ListLevel.Count; i++)
        {
            ListLevel[i].transform.parent.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(w,h);
        }
        _ArrayPiece = GameObject.FindGameObjectsWithTag("Puzzle");
        Admob.instance.fulltime=0;
        LevelUnlocked = PlayerPrefs.GetInt("LevelUnlocked",1);
        CurrentLevel = PlayerPrefs.GetInt("CurrentLevel",1);
        SaveCurrentIndex = PlayerPrefs.GetInt("SaveCurrentIndex",0);
        for(int i=0; i < ListLevel.Count; i++)
        {
            ListLevel[i].transform.Find("Image").gameObject.GetComponent<Image>().color = Color.grey;
        }
        for(int i=0; i < LevelUnlocked; i++)
        {
            ListLevel[i].transform.Find("Image").gameObject.GetComponent<Image>().color = Color.white;
            ListLevel[i].GetComponent<LevelScript>().unlocked  = true;
            ListLevel[i].transform.Find("Image/Lock").gameObject.SetActive(false);
        }
        ControllerUI.instance.PlayAgainButton.GetComponent<Button>().interactable = false;
        ControllerUI.instance.BackButton.SetActive(false);
        ControllerUI.instance.LevelBox.SetActive(false);
    }
    void Start()
    {
        _isEmpty = false;
        _isBlocked = true;    
        BoxPieces.transform.localScale = new Vector2(getScreenWidth(),getScreenHeight() / 78f);

        xMin = BoxPieces.transform.position.x - getScreenWidth()/3f;
        xMax = BoxPieces.transform.position.x + getScreenWidth()/3f;
        for(int j=0; j< _ArrayPiece.Length;j++)
        {
            if(_ArrayPiece[j].GetComponent<PieceScript>()._isBoxPiecesArea)
            {
                _ArrayPiece[j].GetComponent<PieceScript>().id = j;
                _ListPieceInBoxArea.Add(_ArrayPiece[j]);
                _ArrayPiece[j].GetComponent<SortingGroup>().sortingOrder = onthetop;    
            }
        }
    }
    void Update()
    {

        if(!_isBlocked)
        {
            if(_isloadData)
            {
                LoadDataJson();
                LoadDataPieces();
                BoxPieces.GetComponent<SortingGroup>().sortingOrder =  -1;
                if(_hasWon == 48)
                {
                    _isBlocked = true;
                    ControllerUI.instance.LevelCompleteBox.SetActive(true);
                    FramePuzzle.SetActive(false);
                    BoxPieces.SetActive(false);
                    for(int i=0; i <_ArrayPiece.Length; i++)
                    {
                        _ArrayPiece[i].gameObject.SetActive(false);
                    }
                    PieceSelect = null;
                }
                else{
                    SetParent();
                }
                while(_ListPieceInBoxArea.Count != 0)
                {
                    for(int m=0; m<_ListPieceInBoxArea.Count; m++)
                    {
                        _ListPieceInBoxArea.Remove(_ListPieceInBoxArea[0]);
                    }
                } 
                for(int i=0; i<_ArrayPiece.Length; i++)
                {
                    if(_ArrayPiece[i].GetComponent<PieceScript>()._isBoxPiecesArea)
                    {
                        _ListPieceInBoxArea.Add(_ArrayPiece[i]);
                    }
                }
                // Empty = _ListPieceInBoxArea.Count;
                _isloadData = false;
                
            }
            
            MousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            if(Input.GetMouseButtonDown(0))
            {
                if(_isTouchArea == false)
                {
                    if(Mathf.Abs(MousePoint.x - BoxPieces.transform.position.x) <= 20f && Mathf.Abs(MousePoint.y - BoxPieces.transform.position.y) <= 1.6f)
                    {
                        moveTime = moveDuration;
                        _isTouchArea = true;
                        // SelectedPiece=null;
                        // _isRun=true;
                        startTouchPos = MousePoint;
                        // tạo container chứa Pieces
                        GameObject obj = new GameObject();
                        listNewGameobject.Add(obj);
                        obj.transform.parent = BigContainer.transform;
                            
                        for(int n=0; n<_ArrayPiece.Length; n++)
                        {
                            if(_ArrayPiece[n].GetComponent<PieceScript>()._isBoxPiecesArea)
                            {
                                 _ArrayPiece[n].transform.parent = obj.transform;
                            }
                        }
                        Container = obj;
                        
                        deltaXswipe = MousePoint.x - Container.transform.position.x;
                        // deltaYswipe = MousePoint.y - Container.transform.position.y;
                        //check select piece in 
                        for(int j=0;j<_ListPieceInBoxArea.Count;j++){
                            if(Mathf.Abs(startTouchPos.x - _ListPieceInBoxArea[j].transform.position.x)<0.7f && Mathf.Abs(startTouchPos.y - _ListPieceInBoxArea[j].transform.position.y)<0.7f){
                                check=true;
                                Piece = _ListPieceInBoxArea[j];
                            }
                            
                        }      
                    }
                    if(SelectedPiece != null && SelectedPiece.GetComponent<PieceScript>()._isFramePuzzleArea){
                        _isTouchArea = false;
                        // _isRun   =false;
                        for(int m=0; m< _ArrayPiece.Length; m++)
                        {
                            if(_ArrayPiece[m].GetComponent<PieceScript>()._isBoxPiecesArea)
                            {
                                _ArrayPiece[m].transform.parent = BigContainer.transform;
                                _ArrayPiece[m].GetComponent<SortingGroup>().sortingOrder =  onthetop+2;

                            }
                            BoxPieces.GetComponent<SortingGroup>().sortingOrder =  onthetop+1;
                            // if(_ArrayPiece[m].GetComponent<PieceScript>()._isFramePuzzleArea)
                            // {
                            //     _ArrayPiece[m].GetComponent<SortingGroup>().sortingOrder =  onthetop-2;

                            // }
                            if(SelectedPiece.GetComponent<PieceScript>().id == _ArrayPiece[m].GetComponent<PieceScript>().id)
                            {
                                _BlockPieces.Add(_ArrayPiece[m]);
                                // _ArrayPiece[m].GetComponent<PieceScript>()._isConnected=true;
                                _ArrayPiece[m].GetComponent<SortingGroup>().sortingOrder =  onthetop+1;
                                _ArrayPiece[m].transform.position = new Vector3(_ArrayPiece[m].transform.position.x,_ArrayPiece[m].transform.position.y,0);
                                _ArrayPiece[m].GetComponent<PieceScript>().RightPosition.z = 0;
                                _ArrayPiece[m].GetComponent<PieceScript>()._isFramePuzzleArea=true;
                            }

                        }
                        if(!SelectedPiece.GetComponent<PieceScript>()._isConnected){
                            PieceSelect = SelectedPiece;
                            SelectedPiece.GetComponent<SortingGroup>().sortingOrder = onthetop;
                        }  
                    } 
                }

            }
            else if(Input.GetMouseButtonUp(0))
            { 
                //----------------------------------------------CHECK-------------------------------------//
                    if(SelectedPiece != null)
                    {
                        MovetoIndex();
                        SavePiece = SelectedPiece;
                        StartCoroutine(MoveToBoxPiece());
                        SelectedPiece.GetComponent<PieceScript>().Selected = false;
                        SelectedPiece = null;
                        StartCoroutine(Connect());
                        StartCoroutine(WaitSave());
                    }
                    
                    if(_ListPieceInBoxArea.Count ==48)
                    {
                        ControllerUI.instance.PlayAgainButton.GetComponent<Button>().interactable = false;
                    }
                    else
                    {
                        ControllerUI.instance.PlayAgainButton.GetComponent<Button>().interactable = true;
                    }
                     _isEmpty= false;

                //---------------------------------------------SWIPE UP------------------------------------//
                if(Container!=null&& _isTouchArea)
                {
                    _isTouchArea = false;
                    endTouchPos = MousePoint;
                    if(startTime < 0.3f)
                    {
                        if(_ListPieceInBoxArea.Count !=0 && !_isselected){
                            StartPiecesPosition = Container.transform.position;
                                if(endTouchPos.x < startTouchPos.x)
                                {
                                    if(xmaxswipe >= (BoxPieces.transform.position.x + getScreenWidth()/3f))
                                    {
                                        StartCoroutine(Move(true));
                                    }
                                }
                                if(endTouchPos.x > startTouchPos.x)
                                {
                                    if(xminswipe <= (BoxPieces.transform.position.x - getScreenWidth()/3f))
                                    {
                                        StartCoroutine(Move(false));
                                    }
                                }
                            }
                            startTime =0f;
                    }
                    else{
                        startTime =0f;
                        startTouchPos=MousePoint;
                    }
                } 
                startTime =0f;
            }
            else if(Input.GetMouseButton(0))
            {
                if(check){
                    if((MousePoint.y - startTouchPos.y>0.25f) &&  Mathf.Abs(MousePoint.x - startTouchPos.x)<0.4f){
                        _isselected=true;
                        for(int m=0; m< _ArrayPiece.Length; m++)
                        {
                            if(_ArrayPiece[m].GetComponent<PieceScript>()._isBoxPiecesArea)
                            {
                                _ArrayPiece[m].transform.parent = BigContainer.transform;
                                // _ArrayPiece[m].GetComponent<PieceScript>().CurrentPosition = _ArrayPiece[m].GetComponent<PieceScript>().FirstCurrentPos = _ArrayPiece[m].transform.position;
                            }
                                
                        } 
                        check=false;
                        if(_isselected)
                        {
                            _isselected =false;
                            SelectedPiece = Piece;
                            PieceSelect = SelectedPiece;
                            _BlockPieces.Add(SelectedPiece);
                            SelectedPiece.GetComponent<PieceScript>().Selected = true;
                            SelectedPiece.GetComponent<PieceScript>().CurrentPosition = SelectedPiece.GetComponent<PieceScript>().FirstCurrentPos=SelectedPiece.transform.position;
                            _isTouchArea = false;
                            // _isRun   =false;
                            SelectedPiece.GetComponent<SortingGroup>().sortingOrder = onthetop+1;   
                            if(!_isEmpty)
                            {
                                _isEmpty =true;                                
                                for(int i=0; i < _ArrayPiece.Length; i++)
                                {
                                    if(_ArrayPiece[i].GetComponent<PieceScript>()._isBoxPiecesArea && _ArrayPiece[i].name != SelectedPiece.name)
                                    {
                                        if(SelectedPiece.GetComponent<PieceScript>().CurrentPosition.x <= _ArrayPiece[i].transform.position.x)
                                        {
                                            if((xmaxswipe > BoxPieces.transform.position.x + getScreenWidth()/3f) 
                                            || (xmaxswipe  <= BoxPieces.transform.position.x + getScreenWidth()/3f && xminswipe >= BoxPieces.transform.position.x - getScreenWidth()/3f))
                                            {
                                                // _ArrayPiece[i].GetComponent<PieceScript>()._isBoxPiecesArea=true;
                                                Vector3 startPosition = _ArrayPiece[i].transform.position;
                                                Vector3 endPosition = new Vector3(_ArrayPiece[i].transform.position.x - 3f, _ArrayPiece[i].transform.position.y,_ArrayPiece[i].transform.position.z);
                                                StartCoroutine(LerpPosition(_ArrayPiece[i],startPosition,endPosition,0.07f));
                                            }
                                        }
                                        else if(SelectedPiece.GetComponent<PieceScript>().CurrentPosition.x >= _ArrayPiece[i].transform.position.x) 
                                        {
                                            if(xmaxswipe <= BoxPieces.transform.position.x + getScreenWidth()/3f && xminswipe < BoxPieces.transform.position.x - getScreenWidth()/3f)
                                            {
                                                // _ArrayPiece[i].GetComponent<PieceScript>()._isBoxPiecesArea=true;
                                                Vector3 startPosition = _ArrayPiece[i].transform.position;
                                                Vector3 endPosition = new Vector3(_ArrayPiece[i].transform.position.x + 3f, _ArrayPiece[i].transform.position.y,_ArrayPiece[i].transform.position.z);
                                                StartCoroutine(LerpPosition(_ArrayPiece[i],startPosition,endPosition,0.07f));
                                            }
                                        }
                                    }
                                }
                                // Debug.Log("00000000");
                            }    
                        }
                    }
                }
                CheckPosition();
                MoveOutSide Moveout = new MoveOutSide();
                Moveout.BlockPieceMoveOutSide(SelectedPiece,_BlockPieces,FramePuzzle);
                if(SelectedPiece!=null && !SelectedPiece.GetComponent<PieceScript>()._isConnected)
                {
                    if(_isEmpty)
                    {
                        if(SelectedPiece.GetComponent<PieceScript>()._isBoxPiecesArea)
                        {
                            if(SelectedPiece.GetComponent<PieceScript>().CurrentPosition == Vector3.zero)
                            {
                                if(_ListPieceInBoxArea.Count >5)
                                {
                                    _isEmpty =false;
                                    int nearest = NearestIndex(_ListPieceInBoxArea, SelectedPiece);
                                    SelectedPiece.GetComponent<PieceScript>().CurrentPosition =  _ListPieceInBoxArea[nearest].transform.position;
                                    for(int p=0;p<_ListPieceInBoxArea.Count;p++)
                                    {
                                        if(_ListPieceInBoxArea[p].transform.position.x >= SelectedPiece.GetComponent<PieceScript>().CurrentPosition.x && _ListPieceInBoxArea[p].name != SelectedPiece.name)
                                        {
                                            // _ListPieceInBoxArea[p].transform.position = new Vector3(_ListPieceInBoxArea[p].transform.position.x+3f,_ListPieceInBoxArea[p].transform.position.y,_ListPieceInBoxArea[p].transform.position.z);
                                            Vector3 newstartPos = new Vector3();
                                            newstartPos =_ListPieceInBoxArea[p].transform.position ;
                                            Vector3 newendPos= new Vector3(_ListPieceInBoxArea[p].transform.position.x+3f,_ListPieceInBoxArea[p].transform.position.y,_ListPieceInBoxArea[p].transform.position.z);
                                            StartCoroutine(LerpPosition(_ListPieceInBoxArea[p],newstartPos,newendPos,0.05f));

                                        }
                                    }   
                                    // Debug.Log("1111111111");
                                }   
                                if(_ListPieceInBoxArea.Count <=5 && _ListPieceInBoxArea.Count>1)    
                                {
                                    _isEmpty =false;
                                    int nearest = NearestIndex(_ListPieceInBoxArea, SelectedPiece);
                                    SelectedPiece.GetComponent<PieceScript>().CurrentPosition = new Vector3(_ListPieceInBoxArea[nearest].transform.position.x +3f,_ListPieceInBoxArea[nearest].transform.position.y,_ListPieceInBoxArea[nearest].transform.position.z);
                                    for(int h=0;h<_ListPieceInBoxArea.Count;h++)
                                    {
                                        if(_ListPieceInBoxArea[h].name != SelectedPiece.name)
                                        {
                                            if(_ListPieceInBoxArea[h].transform.position.x >=  SelectedPiece.GetComponent<PieceScript>().CurrentPosition.x)
                                            {
                                                _ListPieceInBoxArea[h].transform.position = new Vector3(_ListPieceInBoxArea[h].transform.position.x+3f,_ListPieceInBoxArea[h].transform.position.y,_ListPieceInBoxArea[h].transform.position.z);
                                            }
                                        }
                                    }
                                    // Debug.Log("2222222222");
                                }                  
                            }
                            else if(SelectedPiece.GetComponent<PieceScript>().CurrentPosition != Vector3.zero)
                            {
                                for(int j=0;j<_ListPieceInBoxArea.Count;j++)
                                {
                                    if(SelectedPiece.name != _ListPieceInBoxArea[j].name)
                                    {                           
                                        if(Mathf.Abs(SelectedPiece.transform.position.x - SelectedPiece.GetComponent<PieceScript>().CurrentPosition.x) > 1.5f)
                                        {
                                            if(Mathf.Abs(SelectedPiece.transform.position.x - _ListPieceInBoxArea[j].transform.position.x) < 1.5f)
                                            {
                                                if(_ListPieceInBoxArea[j].transform.position.x > SelectedPiece.GetComponent<PieceScript>().CurrentPosition.x)
                                                {
                                                    _isEmpty =false;
                                                    for(int m=0;m<_ListPieceInBoxArea.Count;m++)
                                                    {
                                                        if(_ListPieceInBoxArea[m].transform.position.x > _ListPieceInBoxArea[j].transform.position.x && _ListPieceInBoxArea[m].name != _ListPieceInBoxArea[j].name)
                                                        {
                                                            _ListPieceInBoxArea[m].transform.position = new Vector3(_ListPieceInBoxArea[m].transform.position.x+3f,_ListPieceInBoxArea[m].transform.position.y,_ListPieceInBoxArea[m].transform.position.z);
                                                        }
                                                    }
                                                     _ListPieceInBoxArea[j].transform.position = new Vector3(_ListPieceInBoxArea[j].transform.position.x+3f,_ListPieceInBoxArea[j].transform.position.y,_ListPieceInBoxArea[j].transform.position.z);
                                                    SelectedPiece.GetComponent<PieceScript>().CurrentPosition =  new Vector3(_ListPieceInBoxArea[j].transform.position.x-3f,_ListPieceInBoxArea[j].transform.position.y,_ListPieceInBoxArea[j].transform.position.z);
                                                    SelectedPiece.GetComponent<PieceScript>().FirstCurrentPos = Vector3.zero;
                                                    // Debug.Log("33333333");
                                                    
                                                }
                                                if(_ListPieceInBoxArea[j].transform.position.x < SelectedPiece.GetComponent<PieceScript>().CurrentPosition.x)
                                                {
                                                    _isEmpty =false;
                                                    for(int m=0;m<_ListPieceInBoxArea.Count;m++)
                                                    {
                                                        if(_ListPieceInBoxArea[m].transform.position.x < _ListPieceInBoxArea[j].transform.position.x && _ListPieceInBoxArea[m].name != _ListPieceInBoxArea[j].name)
                                                        {
                                                            _ListPieceInBoxArea[m].transform.position = new Vector3(_ListPieceInBoxArea[m].transform.position.x-3f,_ListPieceInBoxArea[m].transform.position.y,_ListPieceInBoxArea[m].transform.position.z);
                                                        }
                                                    }
                                                    _ListPieceInBoxArea[j].transform.position = new Vector3(_ListPieceInBoxArea[j].transform.position.x-3f,_ListPieceInBoxArea[j].transform.position.y,_ListPieceInBoxArea[j].transform.position.z);
                                                    SelectedPiece.GetComponent<PieceScript>().CurrentPosition =  new Vector3(_ListPieceInBoxArea[j].transform.position.x+3f,_ListPieceInBoxArea[j].transform.position.y,_ListPieceInBoxArea[j].transform.position.z);
                                                    SelectedPiece.GetComponent<PieceScript>().FirstCurrentPos = Vector3.zero;
                                                    // Debug.Log("4444444444");
                                                }
                                                
                                            }
                                        
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if(!_isEmpty)
                    {
                        if(!SelectedPiece.GetComponent<PieceScript>()._isBoxPiecesArea && SelectedPiece.GetComponent<PieceScript>().CurrentPosition !=Vector3.zero)
                        {
                            _isEmpty=true;
                            for(int n=0;n<_ListPieceInBoxArea.Count;n++)
                            {
                                if(SelectedPiece.GetComponent<PieceScript>().CurrentPosition.x < _ListPieceInBoxArea[n].transform.position.x && SelectedPiece.name != _ListPieceInBoxArea[n].name)
                                {
                                    // _ListPieceInBoxArea[n].transform.position = new Vector3(_ListPieceInBoxArea[n].transform.position.x-3f,_ListPieceInBoxArea[n].transform.position.y,_ListPieceInBoxArea[n].transform.position.z);
                                    Vector3 startPos = new Vector3();
                                    startPos =_ListPieceInBoxArea[n].transform.position ;
                                    Vector3 endPos=  new Vector3(_ListPieceInBoxArea[n].transform.position.x-3f,_ListPieceInBoxArea[n].transform.position.y,_ListPieceInBoxArea[n].transform.position.z);
                                    StartCoroutine(LerpPosition(_ListPieceInBoxArea[n],startPos,endPos,0.05f));
                                }
                                
                            }
                            SelectedPiece.GetComponent<PieceScript>().CurrentPosition = SelectedPiece.GetComponent<PieceScript>().FirstCurrentPos =  Vector3.zero;
                            // Debug.Log("5555555555");
                        }
                        if(SelectedPiece.GetComponent<PieceScript>()._isBoxPiecesArea && SelectedPiece.GetComponent<PieceScript>().CurrentPosition != Vector3.zero &&_ListPieceInBoxArea.Count>2)
                        {                      
                            for(int k=0;k<_ListPieceInBoxArea.Count;k++)
                            {
                                if(SelectedPiece.name != _ListPieceInBoxArea[k].name)
                                {                           
                                    if(Mathf.Abs(SelectedPiece.transform.position.x - SelectedPiece.GetComponent<PieceScript>().CurrentPosition.x) > 1.6f)
                                    {                                  
                                        if(Mathf.Abs(SelectedPiece.transform.position.x - _ListPieceInBoxArea[k].transform.position.x) < 1.4f && _ListPieceInBoxArea[k].transform.position != SelectedPiece.GetComponent<PieceScript>().CurrentPosition)
                                        {
                                            // _isEmpty=true;
                                            Vector3 swapPos = _ListPieceInBoxArea[k].transform.position;
                                            Vector3 startPosition = _ListPieceInBoxArea[k].transform.position ;
                                            Vector3 endPosition = SelectedPiece.GetComponent<PieceScript>().CurrentPosition;
                                            StartCoroutine(LerpPosition(_ListPieceInBoxArea[k],startPosition,endPosition,0.0001f));                                            
                                            SelectedPiece.GetComponent<PieceScript>().CurrentPosition =  swapPos;
                                            SelectedPiece.GetComponent<PieceScript>().FirstCurrentPos =  Vector3.zero;
                                            // Debug.Log("6666666666");
                                        }
                                    }
                                }
                            }
                        }
                        if(SelectedPiece.GetComponent<PieceScript>()._isBoxPiecesArea && SelectedPiece.GetComponent<PieceScript>().CurrentPosition == Vector3.zero)
                        {                      
                            _isEmpty=true;
                            // Debug.Log("4");
                        }
                    }
                }

                if(_ListPieceInBoxArea.Count !=0 && !_isloadData && SelectedPiece==null)
                {
                    if(_isTouchArea)
                    {
                        if(!mouseInBoxPiece)
                        {
                            startTime += Time.deltaTime;
                        }
                        if(mouseInBoxPiece)
                        {
                            if( startTime >=0.3f)
                            {
                                startTime =0f;
                                // Debug.Log(startTime);
                                startTouchPos = MousePoint;
                            }else{
                                startTime += Time.deltaTime;
                            }
                            endTouchPos = MousePoint;
                            CheckSwipe();
                            if(Container!= null)
                            {
                                if((endTouchPos.x < startTouchPos.x))
                                {   
                                    if(xmaxswipe > (BoxPieces.transform.position.x + getScreenWidth()/3f))
                                    {
                                        Container.transform.position = new Vector3(MousePoint.x-deltaXswipe, Container.transform.position.y,Container.transform.position.z);
                                    }
                                    if(xmaxswipe <= (BoxPieces.transform.position.x + getScreenWidth()/3f))
                                    {
                                        deltaXswipe = MousePoint.x - Container.transform.position.x;
                                        startTouchPos = MousePoint;
                                    }
                                    
                                }
                                if((endTouchPos.x > startTouchPos.x))
                                {
                                    if(xminswipe < (BoxPieces.transform.position.x - getScreenWidth()/3f))
                                    {
                                        Container.transform.position = new Vector3(MousePoint.x-deltaXswipe, Container.transform.position.y,Container.transform.position.z);
                                    }
                                    else if(xminswipe >= (BoxPieces.transform.position.x - getScreenWidth()/3f))
                                    {
                                        deltaXswipe = MousePoint.x - Container.transform.position.x;
                                        startTouchPos = MousePoint;
                                    }
                                    
                                }
                            }
                        }
                        
                    }
                }
                
            }
            if(SelectedPiece == null )
            {
                if(_BlockPieces.Count != 0)
                {
                    for(int j=0; j< _BlockPieces.Count; j++)
                    {
                        _BlockPieces.Remove(_BlockPieces[0]);
                    }
                }
                for(int j=0; j<listNewGameobject.Count;j++)
                {
                    if(listNewGameobject[j].transform.childCount ==0){
                        Destroy(listNewGameobject[j]);
                        listNewGameobject.Remove(listNewGameobject[j]);
                    }
                }
                if(xmaxswipe <= (BoxPieces.transform.position.x + getScreenWidth()/3f) || xminswipe >= (BoxPieces.transform.position.x - getScreenWidth()/3f))
                {
                    moveTime = moveDuration;
                }
            }
        }
        
    }

    // private bool _isLerpingPosPiece;

    IEnumerator LerpPosition(GameObject Piece, Vector3 startPosition,Vector3 endPosition,float duration)
    {
        float time = 0;
        while (time < duration)
        {
            Piece.transform.position = Vector3.Lerp(startPosition, endPosition, time / duration);
            time += Time.deltaTime;
            //yield return null;
        }

        yield return new WaitForSeconds(0.001f);
        Piece.transform.position = endPosition;
    }

    private IEnumerator WaitSave()
    {
        SelectedPiece = null;
        yield return new WaitForSeconds(0.075f);
        // Debug.Log("!1111");
        SaveDataPieces();
        SaveDataJson();
    }
    // ApplicationQuit
    private IEnumerator Move(bool WhereToMove)
    {
        switch(WhereToMove)
        {
            case true:
                moveTime = 0f;
                if(xmaxswipe <= (BoxPieces.transform.position.x + getScreenWidth()/3f) + (startTouchPos.x - endTouchPos.x))
                {
                    EndPiecesPositon    = new Vector3(StartPiecesPosition.x - BoxPieces.transform.position.x - getScreenWidth()/3f, StartPiecesPosition.y,StartPiecesPosition.z);
                }
                else
                {
                    EndPiecesPositon    = new Vector3(StartPiecesPosition.x - (startTouchPos.x - endTouchPos.x)*(3f-startTime), StartPiecesPosition.y,StartPiecesPosition.z);
                }
                if(Container!=null)
                {
                    while(moveTime < moveDuration)
                    {
                        moveTime += 0.14f * Time.deltaTime;
                    
                            Container.transform.position = Vector3.Lerp(StartPiecesPosition,EndPiecesPositon,moveTime/moveDuration);
                        
                        CheckSwipe();
                        yield return null;
                    }
                }
                break;
            case false:
                moveTime =0f;
                if(xminswipe >= (BoxPieces.transform.position.x - getScreenWidth()/3f) - (endTouchPos.x - startTouchPos.x))
                {
                    EndPiecesPositon    = new Vector3( StartPiecesPosition.x+  BoxPieces.transform.position.x + getScreenWidth()/3f, StartPiecesPosition.y,StartPiecesPosition.z);
                }
                else
                {
                    EndPiecesPositon    = new Vector3(StartPiecesPosition.x + (endTouchPos.x - startTouchPos.x)*(3f-startTime), StartPiecesPosition.y,StartPiecesPosition.z);
                }
                if(Container!=null)
                {
                    while(moveTime < moveDuration)
                    {
                        moveTime += 0.14f * Time.deltaTime;
                            Container.transform.position = Vector3.Lerp(StartPiecesPosition,EndPiecesPositon,moveTime/moveDuration);
                        
                        CheckSwipe();
                        yield return null;
                    }
                }
                break;
        }
    }
    public float getScreenHeight()
    {
        return Camera.main.orthographicSize * 2.0f;

    }
    public float getScreenWidth()
    {
        return getScreenHeight() * Screen.width / Screen.height;
    }
    public void SaveDataPieces()
    {
        if(CurrentLevel != 0)
        {
            for(int i=0; i< _ArrayPiece.Length; i++)
            {
                PieceData savePiceData = new PieceData();
                savePiceData.isRightPosition   = _ArrayPiece[i].GetComponent<PieceScript>()._isRightPosition;
                savePiceData.isFramePuzzleArea = _ArrayPiece[i].GetComponent<PieceScript>()._isFramePuzzleArea;
                savePiceData.isBoxPiecesArea   = _ArrayPiece[i].GetComponent<PieceScript>()._isBoxPiecesArea;
                savePiceData.isConnected       = _ArrayPiece[i].GetComponent<PieceScript>()._isConnected;
                savePiceData.maxCount          = _ArrayPiece[i].GetComponent<PieceScript>()._maxCount;
                savePiceData.Id                = _ArrayPiece[i].GetComponent<PieceScript>().id;             

                savePiceData.PiecePositionX    = _ArrayPiece[i].GetComponent<PieceScript>().transform.position.x;
                savePiceData.PiecePositionY    = _ArrayPiece[i].GetComponent<PieceScript>().transform.position.y;
                savePiceData.PiecePositionZ    = _ArrayPiece[i].GetComponent<PieceScript>().transform.position.z;


                string Json     = JsonUtility.ToJson(savePiceData);
                StreamWriter sw = new StreamWriter(Application.persistentDataPath +"/"+CurrentLevel+"_"+_ArrayPiece[i].name+"zw.text");
                sw.Write(Json);
                sw.Close();
            }
        }
    }
    public void LoadDataPieces()
    {
        float xdiff = BoxPieces.transform.position.x - getScreenWidth()/3f;
        for(int i=0; i< _ArrayPiece.Length; i++)
        {
            if(File.Exists(Application.persistentDataPath +"/"+CurrentLevel+"_"+_ArrayPiece[i].name+"zw.text"))
            {
                
                StreamReader sr = new StreamReader(Application.persistentDataPath +"/"+CurrentLevel+"_"+_ArrayPiece[i].name+"zw.text");
                string Json     = sr.ReadToEnd();
                sr.Close();
                PieceData savePiceData = JsonUtility.FromJson<PieceData>(Json);
                //Debug.Log("__________Data Loaded___________");
                
                _ArrayPiece[i].GetComponent<PieceScript>()._isRightPosition   = savePiceData.isRightPosition;
                _ArrayPiece[i].GetComponent<PieceScript>()._isFramePuzzleArea = savePiceData.isFramePuzzleArea;
                _ArrayPiece[i].GetComponent<PieceScript>()._isBoxPiecesArea   = savePiceData.isBoxPiecesArea;
                _ArrayPiece[i].GetComponent<PieceScript>()._isConnected       = savePiceData.isConnected;
                _ArrayPiece[i].GetComponent<PieceScript>()._maxCount          = savePiceData.maxCount;
                _ArrayPiece[i].GetComponent<PieceScript>().id                 = savePiceData.Id;

                _ArrayPiece[i].GetComponent<PieceScript>().transform.position = new Vector3(savePiceData.PiecePositionX,savePiceData.PiecePositionY,savePiceData.PiecePositionZ);

            }
            else
            {
                _ArrayPiece[i].GetComponent<PieceScript>()._isRightPosition   = false;
                _ArrayPiece[i].GetComponent<PieceScript>()._isFramePuzzleArea = false;
                _ArrayPiece[i].GetComponent<PieceScript>()._isBoxPiecesArea   = true;
                _ArrayPiece[i].GetComponent<PieceScript>()._isConnected       = false;
                _ArrayPiece[i].GetComponent<PieceScript>()._maxCount          = 0;
                _ArrayPiece[i].GetComponent<PieceScript>().id                 = i;
                _ArrayPiece[i].GetComponent<SortingGroup>().sortingOrder      = 0;
                _ArrayPiece[i].transform.parent                               = null;
                _ArrayPiece[i].transform.parent                               = GameObject.Find("Container").transform;

                _ArrayPiece[i].transform.position = new Vector2(xdiff, BoxPieces.transform.position.y);
                xdiff += 3f;
            }
        }
        
    }
    public void SaveDataJson()
    {
        if(CurrentLevel !=0)
        {
            LevelData saveLevelData = createSaveData();  
            string Json     = JsonUtility.ToJson(saveLevelData);
            StreamWriter sw = new StreamWriter(Application.persistentDataPath +"/"+CurrentLevel+"zw.text");
            sw.Write(Json);
            sw.Close();
        }
    }
    public void LoadDataJson()
    {
        if(CurrentLevel != 0)
        {
            if(File.Exists(Application.persistentDataPath +"/"+CurrentLevel+"zw.text"))
            {
                StreamReader sr         = new StreamReader(Application.persistentDataPath +"/"+CurrentLevel+"zw.text");
                string Json             = sr.ReadToEnd();
                sr.Close();
                LevelData saveLevelData = JsonUtility.FromJson<LevelData>(Json);
                _hasWon                 = saveLevelData.HasWon;
            }
            else
            {
                for(int i=0; i<_ArrayPiece.Length;i++)
                {
                    if(_ArrayPiece[i].GetComponent<PieceScript>()._isBoxPiecesArea)
                    {
                        _ArrayPiece[i].GetComponent<PieceScript>().id = i;
                        _ListPieceInBoxArea.Add(_ArrayPiece[i]);
                    }
                    GameObject obj = _ArrayPiece[i];
                    int RandArray  = Random.Range(0,i);
                    _ArrayPiece[i] = _ArrayPiece[RandArray];
                    _ArrayPiece[RandArray] = obj;   
                } 
                while(_ListPieceInBoxArea.Count != 0)
                {
                    for(int m=0; m<_ListPieceInBoxArea.Count; m++)
                    {
                        _ListPieceInBoxArea.Remove(_ListPieceInBoxArea[0]);
                    }
                }

                for(int n=0; n< _ArrayPiece.Length; n++)
                {
                    _ListPieceInBoxArea.Add(_ArrayPiece[n]);
                }   
                _hasWon =0;
            }
        }
    }

    private LevelData createSaveData()
    {
        LevelData saveLevelData          = new LevelData();
        saveLevelData.HasWon             = _hasWon;
        return saveLevelData;
    }   

    private int NearestIndex(List<GameObject> _ListPieceInBoxArea, GameObject SelectedPiece)
    {
        float minDist = float.MaxValue;
        int nearestIndex = -1;
        for (int i = 0; i < _ListPieceInBoxArea.Count; i++)
        {
            if(SelectedPiece.name != _ListPieceInBoxArea[i].name){
            var dist = Vector3.Distance(SelectedPiece.transform.position, _ListPieceInBoxArea[i].transform.position);
            if (dist < minDist)
            {
                nearestIndex = i;
                minDist = dist;
            } 
            }
        }
        return nearestIndex;
    }
    private IEnumerator MoveToBoxPiece()
    {
        SelectedPiece = SavePiece;
        yield return new WaitForSeconds(0.01f);
        if(SavePiece!=null)
        {
            if(_ListPieceInBoxArea.Count==1)
            {
                if(SavePiece.GetComponent<PieceScript>()._isBoxPiecesArea){
                    Vector3 startPosition = SavePiece.transform.position;
                    Vector3 endPosition = new Vector3(BoxPieces.transform.position.x - getScreenWidth()/3f - 0.1f,BoxPieces.transform.position.y,0);
                    StartCoroutine(LerpPosition(SavePiece,startPosition,endPosition,0.03f));
                }
            }
            else if(_ListPieceInBoxArea.Count>1)
            {
                if(SavePiece.GetComponent<PieceScript>()._isBoxPiecesArea){
                    if(SavePiece.GetComponent<PieceScript>().CurrentPosition!= Vector3.zero)
                    {
                        if(SavePiece.GetComponent<PieceScript>().FirstCurrentPos ==Vector3.zero)
                        {
                            // SavePiece.transform.position = SavePiece.GetComponent<PieceScript>().CurrentPosition;
                            Vector3 startPosition = SavePiece.transform.position;
                            Vector3 endPosition = SavePiece.GetComponent<PieceScript>().CurrentPosition;
                            StartCoroutine(LerpPosition(SavePiece,startPosition,endPosition,0.04f));
                            Debug.Log("111111"); 
                        }
                        else{
                            SavePiece.transform.position = SavePiece.GetComponent<PieceScript>().FirstCurrentPos;
                            for(int h=0; h < _ArrayPiece.Length; h++)
                            {
                                if(_ArrayPiece[h].GetComponent<PieceScript>()._isBoxPiecesArea)
                                {
                                    if(SavePiece.GetComponent<PieceScript>().FirstCurrentPos.x-0.3f < _ArrayPiece[h].transform.position.x && _ArrayPiece[h].name != SavePiece.name )
                                    {
                                        _ArrayPiece[h].transform.position = new Vector2(_ArrayPiece[h].transform.position.x + 3f, BoxPieces.transform.position.y);
                                    }
                                } 
                                                    
                            }
                            Debug.Log("2222222");
                            
                        } 
                    }
                    else if(SavePiece.GetComponent<PieceScript>().CurrentPosition== Vector3.zero)
                    {
                        int nearest = NearestIndex(_ListPieceInBoxArea, SavePiece);
                        SavePiece.transform.position = _ListPieceInBoxArea[nearest].transform.position;
                        for(int h=0;h<_ListPieceInBoxArea.Count;h++)
                        {
                            if(_ListPieceInBoxArea[h].transform.position.x >=  SavePiece.transform.position.x-0.1f && _ListPieceInBoxArea[h].name != SavePiece.name)
                            {
                                _ListPieceInBoxArea[h].transform.position = new Vector3(_ListPieceInBoxArea[h].transform.position.x+3f,_ListPieceInBoxArea[h].transform.position.y,_ListPieceInBoxArea[h].transform.position.z);
                            }
                        }
                        Debug.Log("3333333");
                    }
                }
            }
        }
    }
    private void CheckSwipe()
    {
        xminswipe = _ListPieceInBoxArea[0].transform.position.x;
        xmaxswipe = _ListPieceInBoxArea[0].transform.position.x;
        foreach(GameObject obj in _ListPieceInBoxArea)
        {
            if(obj.transform.position.x <= xminswipe)
            {
                xminswipe = obj.transform.position.x;
            }
            if(obj.transform.position.x >= xmaxswipe)
            {
                xmaxswipe = obj.transform.position.x;
            }
        }
    }
    private IEnumerator WaitConfettiFx()
    {
       
        ControllerUI.instance.VictorySound.Play();
        ControllerUI.instance.backGroundSound.Stop();
        GameObject ob = Instantiate(confettiFx);
        Destroy(ob,5);
        yield return new WaitForSeconds(5);
        ControllerUI.instance.LevelCompleteBox.SetActive(true);
        ControllerUI.instance.VictorySound.Stop();
        ControllerUI.instance.LoadSound();
        FramePuzzle.SetActive(false);
        BoxPieces.SetActive(false);
        for(int i=0; i <_ArrayPiece.Length; i++)
        {
            _ArrayPiece[i].gameObject.SetActive(false);
        }
    }
    private void CheckPosition()
    {
        if(SelectedPiece!=null)
        {
            if(Mathf.Abs(SelectedPiece.transform.position.x - BoxPieces.transform.position.x) <= 20f && Mathf.Abs(SelectedPiece.transform.position.y - BoxPieces.transform.position.y) < 1.6f && !SelectedPiece.GetComponent<PieceScript>()._isConnected)
            {
                if(!SelectedPiece.GetComponent<PieceScript>()._isConnected && !SelectedPiece.GetComponent<PieceScript>()._isBoxPiecesArea){
                    _ListPieceInBoxArea.Add(SelectedPiece);
                }
                SelectedPiece.GetComponent<PieceScript>()._isBoxPiecesArea = true;
            }
            else
            {
                SelectedPiece.GetComponent<PieceScript>()._isBoxPiecesArea = false;
                 _ListPieceInBoxArea.Remove(SelectedPiece);
            }
            if(Mathf.Abs(SelectedPiece.transform.position.x - FramePuzzle.transform.position.x) <= 4.4f && Mathf.Abs(SelectedPiece.transform.position.y - FramePuzzle.transform.position.y) <= 6.5f)
            {
                SelectedPiece.GetComponent<PieceScript>()._isFramePuzzleArea = true;
            }
            else
            {
                SelectedPiece.GetComponent<PieceScript>()._isFramePuzzleArea = false;
            }
        }
        else{
            if(Mathf.Abs(MousePoint.x - BoxPieces.transform.position.x) <= 20f && Mathf.Abs(MousePoint.y - BoxPieces.transform.position.y ) <= 1.6f)
            {
                mouseInBoxPiece =true;
            }
            else{
                mouseInBoxPiece = false;
                // _isTouchArea =false;
            }
        }
    }
    private IEnumerator Connect()
    {
        if(SelectedPiece == null )
        {
            yield return new WaitForSeconds(0.075f);
            for(int i=0; i< _ArrayPiece.Length; i++)
            {
                if(_ArrayPiece[i].GetComponent<PieceScript>()._isFramePuzzleArea)
                {
                    for(int k=0; k < _ArrayPiece.Length; k++)
                    {
                        if(_ArrayPiece[k].GetComponent<PieceScript>()._isFramePuzzleArea)
                        {
                            if(_ArrayPiece[k].GetComponent<PieceScript>().id != _ArrayPiece[i].GetComponent<PieceScript>().id && _ArrayPiece[k].name != _ArrayPiece[i].name)
                            {
                                if(_ArrayPiece[k] == _ArrayPiece[i].GetComponent<PieceScript>().right)
                                {

                                    if(Mathf.Abs(_ArrayPiece[k].transform.position.x - _ArrayPiece[i].transform.position.x - _ArrayPiece[i].GetComponent<PieceScript>().RightDistance) <=0.02f 
                                    && Mathf.Abs(_ArrayPiece[k].transform.position.y - _ArrayPiece[i].transform.position.y)<=0.02f)
                                    {
                                            // Debug.Log("right");
                                        for(int j=0; j < _ArrayPiece.Length; j++)
                                        {
                                            if(_ArrayPiece[j].GetComponent<PieceScript>().id == _ArrayPiece[i].GetComponent<PieceScript>().id && _ArrayPiece[j].name != _ArrayPiece[i].name)
                                            {
                                                _ArrayPiece[j].GetComponent<PieceScript>().id = _ArrayPiece[k].GetComponent<PieceScript>().id ;
                                                _ArrayPiece[j].GetComponent<PieceScript>()._isConnected=true;
                                            }
                                        }
                                        _ArrayPiece[i].GetComponent<PieceScript>().id = _ArrayPiece[k].GetComponent<PieceScript>().id ;
                                        _ArrayPiece[i].GetComponent<PieceScript>()._isConnected = true;
                                        _ArrayPiece[k].GetComponent<PieceScript>()._isConnected = true;
                                    }
                                }
                                if(_ArrayPiece[k] == _ArrayPiece[i].GetComponent<PieceScript>().left)
                                {
                                    if(Mathf.Abs(_ArrayPiece[k].transform.position.x - _ArrayPiece[i].transform.position.x - _ArrayPiece[i].GetComponent<PieceScript>().LeftDistance)<=0.02f 
                                    && Mathf.Abs(_ArrayPiece[k].transform.position.y - _ArrayPiece[i].transform.position.y)<=0.02f)
                                    {
                                            // Debug.Log("left");
                                        for(int j=0; j < _ArrayPiece.Length; j++)
                                        {
                                            if(_ArrayPiece[j].GetComponent<PieceScript>().id == _ArrayPiece[i].GetComponent<PieceScript>().id && _ArrayPiece[j].name != _ArrayPiece[i].name)
                                            {
                                                _ArrayPiece[j].GetComponent<PieceScript>().id = _ArrayPiece[k].GetComponent<PieceScript>().id ;
                                                _ArrayPiece[j].GetComponent<PieceScript>()._isConnected=true;
                                            }
                                        }
                                         _ArrayPiece[i].GetComponent<PieceScript>().id = _ArrayPiece[k].GetComponent<PieceScript>().id ;
                                        _ArrayPiece[i].GetComponent<PieceScript>()._isConnected = true;
                                        _ArrayPiece[k].GetComponent<PieceScript>()._isConnected = true;
                                    }
                                }
                                if(_ArrayPiece[k] == _ArrayPiece[i].GetComponent<PieceScript>().up)
                                {
                                        
                                    if( Mathf.Abs(_ArrayPiece[k].transform.position.y - _ArrayPiece[i].transform.position.y - _ArrayPiece[i].GetComponent<PieceScript>().UpDistance)<=0.02f
                                    && Mathf.Abs(_ArrayPiece[k].transform.position.x - _ArrayPiece[i].transform.position.x)<=0.02f)
                                    {
                                    // Debug.Log("up");
                                        for(int j=0; j < _ArrayPiece.Length; j++)
                                        {
                                            if(_ArrayPiece[j].GetComponent<PieceScript>().id == _ArrayPiece[i].GetComponent<PieceScript>().id && _ArrayPiece[j].name != _ArrayPiece[i].name)
                                            {
                                                _ArrayPiece[j].GetComponent<PieceScript>().id = _ArrayPiece[k].GetComponent<PieceScript>().id ;
                                                _ArrayPiece[j].GetComponent<PieceScript>()._isConnected=true;
                                            }
                                        }
                                        _ArrayPiece[i].GetComponent<PieceScript>().id = _ArrayPiece[k].GetComponent<PieceScript>().id ;
                                        _ArrayPiece[i].GetComponent<PieceScript>()._isConnected = true;
                                        _ArrayPiece[k].GetComponent<PieceScript>()._isConnected = true;
                                    }
                                }
                                if(_ArrayPiece[k] == _ArrayPiece[i].GetComponent<PieceScript>().down)
                                {
                                        
                                    if( Mathf.Abs(_ArrayPiece[k].transform.position.y - _ArrayPiece[i].transform.position.y - _ArrayPiece[i].GetComponent<PieceScript>().DownDistance)<=0.02f
                                    && Mathf.Abs(_ArrayPiece[k].transform.position.x - _ArrayPiece[i].transform.position.x)<=0.02f)
                                    {
                                            // Debug.Log("down");
                                        for(int j=0; j < _ArrayPiece.Length; j++)
                                        {
                                            if(_ArrayPiece[j].GetComponent<PieceScript>().id == _ArrayPiece[i].GetComponent<PieceScript>().id && _ArrayPiece[j].name != _ArrayPiece[i].name)
                                            {
                                                _ArrayPiece[j].GetComponent<PieceScript>().id = _ArrayPiece[k].GetComponent<PieceScript>().id ;
                                                _ArrayPiece[j].GetComponent<PieceScript>()._isConnected=true;
                                            }
                                        }
                                         _ArrayPiece[i].GetComponent<PieceScript>().id = _ArrayPiece[k].GetComponent<PieceScript>().id ;
                                        _ArrayPiece[i].GetComponent<PieceScript>()._isConnected = true;
                                        _ArrayPiece[k].GetComponent<PieceScript>()._isConnected = true;
                                    }
                                }
                            }
                        }
                        else if(_ArrayPiece[k].GetComponent<PieceScript>()._isBoxPiecesArea)
                        {
                            _ArrayPiece[k].GetComponent<PieceScript>().CurrentPosition = _ArrayPiece[k].transform.position;
                        }
                    }
                }
            }
            SetParent();
            if(PieceSelect != null && !PieceSelect.GetComponent<PieceScript>()._isConnected){
                PieceSelect.GetComponent<SortingGroup>().sortingOrder = onthetop+1;
            }
            
            onthetop+=2;
            
        }
        
    }
    private void CheckWon()
    {
        if(_hasWon==48)
        {
            _isBlocked = true; 
            if(CurrentLevel == LevelUnlocked )
            {
                LevelUnlocked +=1;
                if(LevelUnlocked > PlayerPrefs.GetInt("LevelUnlocked",1))
                {
                    PlayerPrefs.SetInt("LevelUnlocked",LevelUnlocked);
                }
                for(int j=0; j < PlayerPrefs.GetInt("LevelUnlocked",1); j++)
                {
                    ListLevel[j].transform.Find("Image").gameObject.GetComponent<Image>().color = Color.white;
                    ListLevel[j].GetComponent<LevelScript>().unlocked  = true;
                    ListLevel[j].transform.Find("Image/Lock").gameObject.SetActive(false);
                }
            }
            StartCoroutine(WaitConfettiFx());                              
        }
    }
    public void SetParent()
    {
        for(int m=0; m< _ArrayPiece.Length;m++)
                {
                    for(int n=m+1; n< _ArrayPiece.Length; n++)
                    {
                        if(_ArrayPiece[m].GetComponent<PieceScript>().id == _ArrayPiece[n].GetComponent<PieceScript>().id && _ArrayPiece[m].name != _ArrayPiece[n].name)
                        {
                            if(_ArrayPiece[m].transform.parent == GameObject.Find("Container").transform && _ArrayPiece[n].transform.parent == GameObject.Find("Container").transform)
                            {
                                GameObject ob = new GameObject();
                                ob.transform.parent = GameObject.Find("GameController").transform;
                                _ArrayPiece[m].transform.parent = ob.transform;
                                _ArrayPiece[n].transform.parent =  ob.transform;
                                ParentObject.Add(ob);
                            }
                            if(_ArrayPiece[m].transform.parent != GameObject.Find("Container").transform && _ArrayPiece[n].transform.parent == GameObject.Find("Container").transform)
                            {
                                _ArrayPiece[n].transform.parent = _ArrayPiece[m].transform.parent;
                            }
                            if(_ArrayPiece[m].transform.parent == GameObject.Find("Container").transform && _ArrayPiece[n].transform.parent != GameObject.Find("Container").transform )
                            {
                                _ArrayPiece[m].transform.parent = _ArrayPiece[n].transform.parent;
                                                
                            }
                            if(_ArrayPiece[m].transform.parent != GameObject.Find("Container").transform && _ArrayPiece[n].transform.parent != GameObject.Find("Container").transform )
                            {
                                _ArrayPiece[n].transform.parent = _ArrayPiece[m].transform.parent;
                                                
                            }
                        }
                    }
                }
            for(int a=0; a< ParentObject.Count-1;a++)
            {
                for(int b=a+1;b<ParentObject.Count;b++)
                {
                    if(ParentObject[a].transform.childCount < ParentObject[b].transform.childCount)
                    {
                        GameObject temp = ParentObject[a];
                        ParentObject[a] = ParentObject[b];
                        ParentObject[b] = temp;
                    }
                }
            }
            for(int j=0; j<ParentObject.Count;j++)
            {
                foreach(Transform Piece in ParentObject[j].transform)
                {

                    Piece.gameObject.GetComponent<SortingGroup>().sortingOrder = j;
                    Piece.gameObject.transform.position = new Vector3(Piece.gameObject.transform.position.x,Piece.gameObject.transform.position.y,24-j);
                    Piece.gameObject.GetComponent<PieceScript>().RightPosition.z = 1;
                }
            }
    }
    private void MovetoIndex()
    {
        if(SelectedPiece != null && !SelectedPiece.GetComponent<PieceScript>()._isBoxPiecesArea)
        {
            float minDist = float.MaxValue;
            int nearestIndex = -1;
            for (int i = 0; i < _ArrayPiece.Length; i++)
            {
                // if(SelectedPiece.name != _ArrayPiece[i].name){
                    var dist = Vector3.Distance(SelectedPiece.transform.position, _ArrayPiece[i].GetComponent<PieceScript>().RightPosition);
                    if (dist < minDist)
                    {
                        nearestIndex = i;
                        minDist = dist;
                    } 
                // }
            }
            
            
            if(!SelectedPiece.GetComponent<PieceScript>()._isConnected)
            {
                Vector3 startPos = SelectedPiece.transform.position;
                Vector3 endPos   = _ArrayPiece[nearestIndex].GetComponent<PieceScript>().RightPosition;
                StartCoroutine(LerpPosition(SelectedPiece,startPos,endPos,0.075f));
                if(SelectedPiece.name == _ArrayPiece[nearestIndex].name )
                {
                    if(SelectedPiece.GetComponent<PieceScript>()._maxCount ==0 && !SelectedPiece.GetComponent<PieceScript>()._isRightPosition)
                    {
                        SelectedPiece.GetComponent<PieceScript>()._isRightPosition=true;
                        SelectedPiece.GetComponent<PieceScript>()._maxCount ++;
                        if(_hasWon < 48)
                        {
                            _hasWon += 1;
                        }
                        CheckWon();
                    }
                }
                else
                {
                    if(SelectedPiece.GetComponent<PieceScript>()._maxCount==1)
                    {
                        if(_hasWon > 0)
                        {
                            _hasWon -= 1;
                        }

                        SelectedPiece.GetComponent<PieceScript>()._isRightPosition = false;
                        SelectedPiece.GetComponent<PieceScript>()._maxCount --;
                        
                    }
                }
                SelectedPiece.GetComponent<PieceScript>()._isFramePuzzleArea=true;
            }
            else
            {
                Vector3 startPos = SelectedPiece.transform.parent.position;
                Vector3 endPos   = _ArrayPiece[nearestIndex].GetComponent<PieceScript>().RightPosition;
                float deltaX     = endPos.x - SelectedPiece.transform.position.x;
                float deltaY     = endPos.y - SelectedPiece.transform.position.y;
                float deltaZ     = endPos.z - SelectedPiece.transform.position.z;
                Vector3 end      = new Vector3(startPos.x + deltaX, startPos.y+ deltaY,startPos.z+deltaZ);
                StartCoroutine(LerpPosition(SelectedPiece.transform.parent.gameObject,startPos,end,0.075f));
                if(SelectedPiece.name == _ArrayPiece[nearestIndex].name )
                {
                    for(int n=0; n<_BlockPieces.Count ;n++)
                    {
                        if(_BlockPieces[n].GetComponent<PieceScript>()._maxCount ==0 && !_BlockPieces[n].GetComponent<PieceScript>()._isRightPosition)
                        {
                            _BlockPieces[n].GetComponent<PieceScript>()._isRightPosition=true;
                            _BlockPieces[n].GetComponent<PieceScript>()._maxCount ++;
                            _BlockPieces[n].GetComponent<PieceScript>()._isConnected=true;
                            if(_hasWon < 48)
                            {
                                _hasWon += 1;
                            }
                            CheckWon();
                        }
                    }
                }
                else
                {
                    for(int m=0; m<_BlockPieces.Count ;m++)
                    {
                        if(_BlockPieces[m].GetComponent<PieceScript>()._maxCount==1)
                        {
                            if(_hasWon > 0)
                            {
                                _hasWon -= 1;
                            }

                            _BlockPieces[m].GetComponent<PieceScript>()._isRightPosition = false;
                            _BlockPieces[m].GetComponent<PieceScript>()._maxCount --;
                            _BlockPieces[m].GetComponent<PieceScript>()._isConnected=true;
                        }
                    }
                }
            }
        }
        
    }
}


