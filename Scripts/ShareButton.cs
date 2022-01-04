using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ShareButton : MonoBehaviour
{
    private string ShareMessage;

    public void ClickShareButton()
    {
        ShareMessage = "Come and join playing Jigsaw Puzzle with me";

        StartCoroutine(TakeScreeenShareAndShare());
    }
    private IEnumerator TakeScreeenShareAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ScreenShare = new Texture2D(Screen.width,Screen.height,TextureFormat.RGB24, false);
        ScreenShare.ReadPixels(new Rect(0,0,Screen.width, Screen.height), 0 ,0);
        ScreenShare.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "share img.png");
        File.WriteAllBytes(filePath, ScreenShare.EncodeToPNG());

        Destroy(ScreenShare);

        new NativeShare().AddFile(filePath).SetSubject("Jigsaw Puzzle").SetText(ShareMessage).Share();
    }
}
