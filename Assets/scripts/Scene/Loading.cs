using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    [SerializeField] Slider loadingBar;

    // Start is called before the first frame update
    void Start()
    {
        loadingBar.value = 0f;

        StartCoroutine(MoveScene());
    }

    IEnumerator MoveScene()
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync("Editor");

        while (!ao.isDone)
        {
            yield return null;
            loadingBar.value = ao.progress;
        }

        ao.allowSceneActivation = false;
    }
}