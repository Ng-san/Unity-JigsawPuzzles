using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Scroll_Menu : MonoBehaviour
{
    public GameObject MenuBox;
    public GameObject PhotoComplete;
    public GameObject ImageShare;
    public void SetPuzzlesPhoto()
    {
        if(Admob.instance.fulltime >= Admob.instance.TimeShowAdmob)
        {
            // Debug.Log(Admob.instance.TimeShowAdmob);
            Admob.instance.fulltime = 0;
            Admob.instance.RequestInterstitial();
        }
        ControllerPuzzle.instance._isBlocked = false;
        foreach(GameObject ob in ControllerPuzzle.instance.ListLevel)
        { 
            if(ControllerPuzzle.instance.ListLevel.IndexOf(ob)== ControllerPuzzle.instance.LevelUnlocked-1)
            {
                for(int i=0; i<48; i++)
                {
                    GameObject.Find("Piece (" + i + ")").transform.Find("puzzle").GetComponent<SpriteRenderer>().sprite = ob.transform.Find("Image").gameObject.GetComponent<Image>().sprite;
                }
                MenuBox.SetActive(false);
                PhotoComplete.GetComponent<Image>().sprite = ob.transform.Find("Image").gameObject.GetComponent<Image>().sprite;
                ImageShare.GetComponent<Image>().sprite = ob.transform.Find("Image").gameObject.GetComponent<Image>().sprite;
                ControllerPuzzle.instance.CurrentLevel   = ControllerPuzzle.instance.LevelUnlocked;
                PlayerPrefs.SetInt("CurrentLevel",ControllerPuzzle.instance.CurrentLevel);
                ControllerUI.instance.LevelBox.SetActive(true);
                ControllerUI.instance.BackButton.SetActive(true);
                ControllerUI.instance.PlayAgainButton.SetActive(true);
                ControllerUI.instance.LevelText.text = "Level "+ControllerPuzzle.instance.LevelUnlocked;
                ControllerPuzzle.instance.Url            = ob.GetComponent<LevelScript>()._url;
                ControllerPuzzle.instance._isloadData    = true;
                // ControllerPuzzle.instance.SetParent();
            }
        }
    }
    public void NextPhotoButton()
    {
        if(Admob.instance.fulltime >= Admob.instance.TimeShowAdmob)
        {
            // Debug.Log(Admob.instance.TimeShowAdmob);
            Admob.instance.fulltime = 0;
            Admob.instance.RequestInterstitial();
        }
        ControllerPuzzle.instance.FramePuzzle.SetActive(true);
        ControllerPuzzle.instance.BoxPieces.SetActive(true);
        for(int i=0; i <ControllerPuzzle.instance._ArrayPiece.Length; i++)
        {
            ControllerPuzzle.instance._ArrayPiece[i].gameObject.SetActive(true);
        }
        ControllerUI.instance.LevelCompleteBox.SetActive(false);
        ControllerUI.instance.LevelText.text = "Level "+ (ControllerPuzzle.instance.CurrentLevel +1);
        ControllerPuzzle.instance._isBlocked     = false;
        foreach(GameObject ob in ControllerPuzzle.instance.ListLevel)
        { 
            if(ControllerPuzzle.instance.ListLevel.IndexOf(ob)== ControllerPuzzle.instance.CurrentLevel)
            {
                for(int i=0; i<48; i++)
                {
                    GameObject.Find("Piece (" + i + ")").transform.Find("puzzle").GetComponent<SpriteRenderer>().sprite = ob.transform.Find("Image").gameObject.GetComponent<Image>().sprite;
                }
                MenuBox.SetActive(false);

                PhotoComplete.GetComponent<Image>().sprite = ob.transform.Find("Image").gameObject.GetComponent<Image>().sprite;
                ImageShare.GetComponent<Image>().sprite = ob.transform.Find("Image").gameObject.GetComponent<Image>().sprite;
                ControllerPuzzle.instance.Url            = ob.GetComponent<LevelScript>()._url;
            }
        }
        ControllerPuzzle.instance.CurrentLevel++; 
        PlayerPrefs.SetInt("CurrentLevel",ControllerPuzzle.instance.CurrentLevel);
        ControllerPuzzle.instance._isloadData    = true;
        // ControllerPuzzle.instance.SetParent();
    }
}
