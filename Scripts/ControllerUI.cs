using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Rendering;

public class ControllerUI : MonoBehaviour
{
    private static ControllerUI _instance;
    public static ControllerUI instance
    {
        get{
            if(_instance == null){
                _instance = FindObjectOfType<ControllerUI>();
            }
            return _instance;
        }
    }
    public GameObject ScrollView;
    public GameObject panel;

    //Menu
    public  GameObject MenuBox; 

    //LevelComplete
    public  GameObject LevelCompleteBox;

    //Banner
    public  GameObject BackButton;
    public  GameObject LevelBox;
    //public  GameObject SettingBox;
    public  GameObject BoxAll;
    public  GameObject PlayAgainButton;
    private bool isOpen;

    //SOUND SETTING
    public  GameObject Soundbutton;
    public  AudioSource backGroundSound;
    public  AudioSource VictorySound;
    private bool _isMuted;
    private bool _isSaveIndex;
    //public bool _isSettingBoxOn;
    public  Text LevelText;
    public  GameObject ShareBox;

    // public GameObject scrollbar;
    public RectTransform rectTransform;
    public RectTransform canvas;

    //----------------------------------BANNER--------------------------------//
    public void LoadSound()
    {
        _isMuted = PlayerPrefs.GetInt("Muted") == 1;
        backGroundSound.Play() ;
        if(_isMuted)
        {
            backGroundSound.Stop() ;
            Soundbutton.GetComponent<Image>().color = Color.grey;
        }
        else
        {
            backGroundSound.Play() ;
            Soundbutton.GetComponent<Image>().color = Color.white;
        }

    }
    public void OpenSetting()
    {
        if(panel != null)
        {
            Animator animator = panel.GetComponent<Animator>();
            if(animator != null)
            {
                isOpen = animator.GetBool("Show");

                animator.SetBool("Show", !isOpen);
            }
        }
    }

    public void SettingSound()
    { 
        _isMuted = !_isMuted;
        backGroundSound.Play();
        PlayerPrefs.SetInt("Muted",_isMuted ? 1 : 0);
        
        if(_isMuted)
        {
            backGroundSound.Stop() ;
            Soundbutton.GetComponent<Image>().color = Color.grey;
        }
        else
        {
            backGroundSound.Play() ;
            Soundbutton.GetComponent<Image>().color = Color.white;
        }
        
    }

    public void BacktoMenu()
    {
        PlayerPrefs.SetInt("SaveCurrentIndex",1);
        if(Admob.instance.fulltime >= Admob.instance.TimeShowAdmob)
        {
            // Debug.Log(Admob.instance.TimeShowAdmob);
            Admob.instance.fulltime = 0;
            Admob.instance.RequestInterstitial();
        }
        BackButton.SetActive(false);
        PlayAgainButton.SetActive(false);
        LevelBox.SetActive(false);
        MenuBox.SetActive(true);
        LevelCompleteBox.SetActive(false);
        ControllerPuzzle.instance.FramePuzzle.SetActive(true);
        ControllerPuzzle.instance.BoxPieces.SetActive(true);
        for(int i=0; i <ControllerPuzzle.instance._ArrayPiece.Length; i++)
        {
            ControllerPuzzle.instance._ArrayPiece[i].gameObject.SetActive(true);
        }
        ControllerPuzzle.instance._isBlocked = true;
        // float convert = (float)ControllerPuzzle.instance.ListLevel.Count;
        // scrollbar.GetComponent<Scrollbar>().value =  (ControllerPuzzle.instance.CurrentLevel-1)*((1f/(convert-1f)));
    }
    public void Restart()
    {
        ControllerPuzzle.instance._isBlocked = true;
        LevelCompleteBox.SetActive(false);
        ControllerPuzzle.instance.FramePuzzle.SetActive(true);
        ControllerPuzzle.instance.BoxPieces.SetActive(true);
        for(int i=0; i <ControllerPuzzle.instance._ArrayPiece.Length; i++)
        {
            ControllerPuzzle.instance._ArrayPiece[i].gameObject.SetActive(true);
        }
        if(ControllerPuzzle.instance._isBlocked)
        {
            ControllerPuzzle.instance._hasWon =0;
        
            while(ControllerPuzzle.instance._ListPieceInBoxArea.Count != 0)
            {
                for(int m=0; m<ControllerPuzzle.instance._ListPieceInBoxArea.Count; m++)
                {
                    ControllerPuzzle.instance._ListPieceInBoxArea.Remove(ControllerPuzzle.instance._ListPieceInBoxArea[0]);
                }
            }
            for(int i=0; i<ControllerPuzzle.instance._ArrayPiece.Length;i++)
            {
                GameObject obj = ControllerPuzzle.instance._ArrayPiece[i];
                int RandArray  = Random.Range(0,i);
                ControllerPuzzle.instance._ArrayPiece[i] = ControllerPuzzle.instance._ArrayPiece[RandArray];
                ControllerPuzzle.instance._ArrayPiece[RandArray] = obj;  
            }
            
            float xdiff = ControllerPuzzle.instance.BoxPieces.transform.position.x - ControllerPuzzle.instance.getScreenWidth()/3f;
            for(int j=0; j< ControllerPuzzle.instance._ArrayPiece.Length;j++)
            {
                ControllerPuzzle.instance._ArrayPiece[j].GetComponent<PieceScript>()._isBoxPiecesArea   = true;
                ControllerPuzzle.instance._ArrayPiece[j].GetComponent<PieceScript>()._isRightPosition   = false;
                ControllerPuzzle.instance._ArrayPiece[j].GetComponent<PieceScript>()._isFramePuzzleArea = false;
                ControllerPuzzle.instance._ArrayPiece[j].GetComponent<PieceScript>()._isConnected       = false;
                ControllerPuzzle.instance._ArrayPiece[j].GetComponent<PieceScript>()._maxCount          = 0;
                ControllerPuzzle.instance._ArrayPiece[j].GetComponent<PieceScript>().id                 = j;
                ControllerPuzzle.instance._ArrayPiece[j].GetComponent<SortingGroup>().sortingOrder      = 1;
                ControllerPuzzle.instance._ArrayPiece[j].transform.parent                               = null;
                ControllerPuzzle.instance._ArrayPiece[j].transform.parent                               = GameObject.Find("Container").transform;

                ControllerPuzzle.instance._ListPieceInBoxArea.Add(ControllerPuzzle.instance._ArrayPiece[j]);
                ControllerPuzzle.instance._ArrayPiece[j].transform.position = new Vector2(xdiff, ControllerPuzzle.instance.BoxPieces.transform.position.y);
                xdiff += 3f;
            }
            ControllerPuzzle.instance.BoxPieces.GetComponent<SortingGroup>().sortingOrder = 0;
        }
        ControllerPuzzle.instance.SaveDataPieces();
        ControllerPuzzle.instance.SaveDataJson();
        ControllerPuzzle.instance._isBlocked = false;
    }
    //--------------------------------------------LEVEL COMPLETE---------------------------------//
    public void ShareBoxButton()
    {
        ShareBox.SetActive(true);
        LevelCompleteBox.SetActive(false);
    }
    public void CancelShareboxButton()
    {
        ShareBox.SetActive(false);
        LevelCompleteBox.SetActive(true);
    }
}
