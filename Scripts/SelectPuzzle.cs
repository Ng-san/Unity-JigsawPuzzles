using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SelectPuzzle : MonoBehaviour
{
    public GameObject MenuBox;
    public GameObject PhotoComplete;
    public GameObject ImageShare;
    public void SetPuzzlesPhoto()
    {
        if(Admob.instance.fulltime >= Admob.instance.TimeShowAdmob)
        {
            Admob.instance.fulltime = 0;
            Admob.instance.RequestInterstitial();
        }
        if(gameObject.GetComponent<LevelScript>().unlocked)
        {
            ControllerPuzzle.instance._isBlocked = false;
            for(int i=0; i<48; i++)
            {
                GameObject.Find("Piece (" + i + ")").transform.Find("puzzle").GetComponent<SpriteRenderer>().sprite = gameObject.transform.Find("Image").gameObject.GetComponent<Image>().sprite;
            }
            ControllerPuzzle.instance.CurrentLevel   =  gameObject.GetComponent<LevelScript>()._level;
            MenuBox.SetActive(false);
            ControllerUI.instance.LevelBox.SetActive(true);
            ControllerUI.instance.BackButton.SetActive(true);
            ControllerUI.instance.PlayAgainButton.SetActive(true);

            PhotoComplete.GetComponent<Image>().sprite = gameObject.transform.Find("Image").gameObject.GetComponent<Image>().sprite;
            ImageShare.GetComponent<Image>().sprite = gameObject.transform.Find("Image").gameObject.GetComponent<Image>().sprite;
            ControllerUI.instance.LevelText.text = "Level "+gameObject.GetComponent<LevelScript>()._level;
            ControllerPuzzle.instance.Url            = gameObject.GetComponent<LevelScript>()._url;
            PlayerPrefs.SetInt("CurrentLevel",ControllerPuzzle.instance.CurrentLevel);
            ControllerPuzzle.instance._isloadData    = true;
            // ControllerPuzzle.instance.SetParent();
       }
        
    }
}
