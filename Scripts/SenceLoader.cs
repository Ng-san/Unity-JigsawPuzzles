using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SenceLoader : MonoBehaviour
{
    public int sceneID;
    public GameObject loadingScene;
    public Slider Loadingbar;

    void Start()
    {
        StartCoroutine(AsyncLoad());

    }

    IEnumerator AsyncLoad()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);
        loadingScene.SetActive(true);
        while(!operation.isDone)
        {
            Loadingbar.value = operation.progress;
            yield return null;
        }
    }
}
