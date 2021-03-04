using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Editor : MonoBehaviour
{
    [Tooltip("Common")]
    [SerializeField] bool isDev;
    [SerializeField] Transform transitEditor;

    [Tooltip("Transit")]
    [SerializeField] Transform btnRoot, scrView;

    bool isUpdated; //is updated on view


    private void Awake()
    {
        ConstValue.ISDEV = isDev;

        if (isDev)
            PlayerPrefs.DeleteAll();
    }

    void Start()
    {
        //load google data : admin hongseonmail@gmail.com
        //

        for (int i=0; i<btnRoot.childCount; i++)
        {
            Button btn = btnRoot.GetChild(i).GetComponent<Button>();
            btn.onClick.AddListener(() => BtnEVent(btn));
        }

        SqliteMgr.GetInstance.Init();
    }

    void Update() //for debug key
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            CloseApp();
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            
        }
    }

    private void CloseApp()
    {
        for (int i = 0; i < btnRoot.childCount; i++)
            btnRoot.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();

        Application.Quit();
    }

    void BtnEVent(Button btn)
    {
        Common.GetInstance.PrintLog('w', "BtnEVent", "btn.name");

        switch (btn.name.Split('_')[1].ToLower())
        {
            case "save":
                List<string> listA = new List<string>();
                listA.Add("급여#3800000");
                listA.Add("보너스#1200000");
                listA.Add("커피#-38000");
                listA.Add("상시#1200000");

                StringBuilder strBld = new StringBuilder();

                for (int i=0; i<listA.Count; i++)
                {
                    strBld.Append(listA[i] + ',');

                    if (i == listA.Count - 1)
                        strBld.Append(listA[i]);
                }
                listA.Clear(); //clear unsing

                Debug.LogWarning("are : " + strBld.ToString());

                //check is insert or update?
                //##
                //SqliteMgr.GetInstance.InsertData("");
                break;
            case "share": //upload dbfile to cloud
                
                break;
            case "close":
                //close an edit page
                transitEditor.gameObject.SetActive(false);

                //remove items on list
                for(int i=1; i<scrView.childCount; i++) //
                    Destroy(scrView.GetChild(i));
                
                break;
        }
    }

    public void DayEvent(string item, string date)
    {
        if (item.Contains("Day_"))
        {
            //clear prev history
            transitEditor.GetComponentInChildren<TotalValue>().TotalValues(0);
            for(int i=1; i< scrView.childCount-1; i++) //remove all without first and lastest
                Destroy(scrView.GetChild(i).gameObject);
            //end

            transitEditor.gameObject.SetActive(true);

            //read data on current date
            string existData = SqliteMgr.GetInstance.ReadData("select data from inout where date == '" + date + "';");

            if (string.IsNullOrEmpty(existData))
                return;
            Common.GetInstance.PrintLog('w', "exist data", existData); //return 1 line only

            //json parsing
            string[] strSplit = existData.Split(','); //split string comma like a CSV
            for (int i=0; i< strSplit.Length; i++)
            {
                //Debug.LogWarning("dd : " + jData[(i + 1).ToString()].ToString());

                //cpy gameobject
                GameObject items = Instantiate(scrView.GetChild(0).gameObject);
                items.name = "item_" + i;
                items.SetActive(true);
                items.transform.SetParent(scrView);
                items.transform.SetSiblingIndex(scrView.childCount - 2); //set hierarchy order

                //insert data to items
                string[] tmpStr = new string[2];
                tmpStr = strSplit[i].ToString().Split('#');
                transitEditor.GetComponentInChildren<TotalValue>().TotalValues(int.Parse(tmpStr[1])); //extract only value

                items.GetComponent<Item>().InsertData(tmpStr[0], tmpStr[1]); //insert data each items
            }
        }
    }
}