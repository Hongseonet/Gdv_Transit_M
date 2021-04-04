using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Download : MonoBehaviour
{
    public Slider sld;

    //https://xr-a.oss-cn-beijing.aliyuncs.com/Video/20201106_06_Cam1_5692466.mp4
    readonly string urlHead = "https://xr-a.oss-cn-beijing.aliyuncs.com/Video/";
    string localPath;

    List<string> files;


    private void Awake()
    {
        BetterStreamingAssets.Initialize();

        localPath = string.Format("{0}/GymnasticVideo", Application.persistentDataPath);

        //get gymnastic file list
        files = new List<string>();

#if UNITY_EDITOR
        StreamReader sReader = new StreamReader(Path.Combine(Application.streamingAssetsPath, "GymnasticInfo.csv"));
#elif UNITY_ANDROID
        StreamReader sReader = BetterStreamingAssets.OpenText("GymnasticInfo.csv");
#endif
        string readData;

        if (sReader == null)
            Debug.LogWarning("sReader null");
        else
        {
            while ((readData = sReader.ReadLine()) != null)
            {
                //Debug.LogWarning("data : " + sReader.ReadLine());
                try
                {
                    files.Add(readData.Split(',')[0]);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("not pose data in " + e.Message);
                }
            }
            sReader.Close();
            sReader = null;
        }
        //end

        //check directory
        if (!Directory.Exists(localPath))
            Directory.CreateDirectory(localPath);

        //check exist files & file download contain under coroutine
        StartCoroutine(StartDownLoad());
    }

    /// <summary>
    /// download file by URI list
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartDownLoad()
    {
        bool[] existFile = new bool[files.Count];
        bool isSkip = true;

        //get exist files and add skip
        for (int i = 0; i < existFile.Length; i++)
        {
            if (File.Exists(Path.Combine(localPath, files[i] + ".mp4")))
                existFile[i] = true;
            else
                isSkip = false;
        }

        if (!isSkip)
        {
            sld.value = 0;

            List<UnityWebRequest> reqList = new List<UnityWebRequest>();
            List<bool> doneList = new List<bool>();

            for (int i = 0; i < existFile.Length; i++)
            {
                if (!existFile[i])
                {
                    reqList.Add(new UnityWebRequest(urlHead + files[i] + ".mp4"));
                    doneList.Add(false);
                }
            }

            for (int i = 0; i < reqList.Count; i++)
            {
                reqList[i].downloadHandler = new DownloadHandlerBuffer();
                reqList[i].SendWebRequest();
            }

            bool checkExit = false;
            while (!checkExit)
            {
                float maxPer = 0;

                checkExit = true;

                for (int i = 0; i < reqList.Count; i++)
                {
                    if (!reqList[i].isDone)
                    {
                        checkExit = false;
                        maxPer += reqList[i].downloadProgress / (float)reqList.Count;
                    }
                    else
                    {
                        maxPer += 1f / (float)reqList.Count;
                        if (!doneList[i])
                        {
                            byte[] datas = reqList[i].downloadHandler.data;
                            File.WriteAllBytes(Path.Combine(localPath, files[i] + ".mp4"), datas);
                            doneList[i] = true;
                        }
                    }
                }

                sld.value = maxPer;
                yield return null;
            }

            for (int i = 0; i < reqList.Count; i++)
            {
                if (!doneList[i])
                {
                    byte[] datas = reqList[i].downloadHandler.data;
                    File.WriteAllBytes(Path.Combine(localPath, files[i] + ".mp4"), datas);
                    doneList[i] = true;
                }
            }
        }
        yield return new WaitForSeconds(0.5f);

        this.gameObject.SetActive(false);
    }
}