using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DownloadButton : MonoBehaviour
{
    public string url;
    // public GameObject TextDownloaded;
    [SerializeField] private Text textResult;
    public GameObject newPannel;
    private bool _isOpen;
    public void ClickDownloadButton()
    {
        StartCoroutine( TakeScreenshotAndSave() );
    }

    private IEnumerator TakeScreenshotAndSave()
    {
        yield return new WaitForEndOfFrame();
        url = ControllerPuzzle.instance.Url;
        WWW www = new WWW(url);
        float elapsedtime=0f;
        textResult.text = "Downloading...";
        OpenNotification();
        while(!www.isDone)
        {
            elapsedtime+=Time.deltaTime;
            if(elapsedtime >=2f && www.progress <= 0.5)
                break;
            yield return null;
        }
        // StartCoroutine(_downloadImage(www));
        if (string.IsNullOrEmpty(www.error))
        {
            UnityEngine.Debug.Log("Success");
            www.texture.Apply();
            Debug.Log( "Permission result: " + NativeGallery.SaveImageToGallery( www.texture, "JisawPuzzle", "My img {0}.png" ) );
            // textResult.gameObject.SetActive(true);
            textResult.text = "Download success";
            yield return new WaitForSeconds(1.5f);
            OpenNotification();
        }
        else
        {
            UnityEngine.Debug.Log("Error: " + www.error);
            // textResult.gameObject.SetActive(true);
            textResult.text = "Your internet is not working";
            yield return new WaitForSeconds(2f);
            OpenNotification();

        }
    }
    public void OpenNotification()
    {
        if(newPannel != null)
        {
            Animator ani = newPannel.GetComponent<Animator>();
            if(ani != null)
            {
                _isOpen = ani.GetBool("On");

                ani.SetBool("On", !_isOpen);
            }
        }
    }
}
