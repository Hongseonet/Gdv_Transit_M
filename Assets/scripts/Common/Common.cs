using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections;

public class Common : SingletonMgr<Common>
{
    public void CheckPath()
    {
        if (ConstValue.ISDEV)
        {
            Debug.Log("dataPath : " + Application.dataPath);
            Debug.Log("persistentDataPath : " + Application.persistentDataPath);
            Debug.Log("streamingAssetsPath : " + Application.streamingAssetsPath);
        }
    }

    public void SetImage(string spriteName, Image target)
    {
        target.sprite = Resources.Load(spriteName, typeof(Sprite)) as Sprite;
    }
    public void SetImage(string spriteName, RawImage target)
    {

    }

    public void PrintLog(char type, string grp, string msg)
    {
        if (ConstValue.ISDEV)
        {
            switch (type)
            {
                case 'w':
                    Debug.LogWarning (grp + " : " + msg);
                    break;
                case 'e':
                    Debug.LogError (grp + " : " + msg);
                    break;
                default:
                    Debug.Log(grp + " : " + msg);
                    break;
            }
        }
    }

    public void SetColor(Image img, Color color)
    {
        if (color.r > 1f && color.b > 1f && color.b > 1f)
            img.color = new Color(color.r / 255, color.g / 255, color.b / 255, color.a / 255);
        else
            img.color = color;
    }
    public void SetColor(RawImage img, Color color)
    {
        if (color.r > 1f && color.b > 1f && color.b > 1f)
            img.color = new Color(color.r / 255, color.g / 255, color.b / 255, color.a / 255);
        else
            img.color = color;
    }
    public void SetColor(Text txt, Color color)
    {
        if (color.r > 1f && color.b > 1f && color.b > 1f)
            txt.color = new Color(color.r / 255, color.g / 255, color.b / 255, color.a / 255);
        else
            txt.color = color;
    }
    public void SetColor(Button btn, Color color)
    {
        if (color.r > 1f && color.b > 1f && color.b > 1f)
            btn.gameObject.GetComponent<Image>().color = new Color(color.r / 255, color.g / 255, color.b / 255, color.a / 255);
        else
            btn.gameObject.GetComponent<Image>().color = color;
    }

    public void AppQuit()
    {
        Application.Quit();
    }

    public IEnumerator NextScene(string sceneName)
    {
        GC.Collect();
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadSceneAsync(sceneName);
    }
}