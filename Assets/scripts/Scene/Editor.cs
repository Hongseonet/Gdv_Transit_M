using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Editor : MonoBehaviour
{
    [Tooltip("Common")]
    [SerializeField] bool isDev;
    [SerializeField] bool isClearDB;
    [SerializeField] Transform transitEditor;
    [SerializeField] CanvasScaler canvasScaler;

    [Tooltip("Transit")]
    [SerializeField] Transform btnRoot, scrView;

    bool isDataUpdate; //is updated on view
    string selectedDay; //giaoisd

    float scrRatio;
    //work resolution : 1080*1920 HD

    private void Awake()
    {
        ConstValue.ISDEV = isDev;

        if (isDev)
        {
            PlayerPrefs.DeleteAll();
            GameObject.Find("/Reporter").SetActive(true);
        }
        else
            GameObject.Find("/Reporter").SetActive(false);

        if (isClearDB)
            ConstValue.CLEARDB = true;

        scrRatio = Screen.width / 1080f;
        //canvasScaler.scaleFactor = scrRatio;
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

        GetComponent<CalendarController>().Init();
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
        Common.GetInstance.PrintLog('w', "BtnEVent", btn.name);

        switch (btn.name.Split('_')[1].ToLower())
        {
            case "write":
                //Debug.LogWarning("not defined yet");
                break;
            case "share": //apply data to db
                List<string> listItems = new List<string>();

                //find scrview
                for (int i=1; i<scrView.childCount-1; i++)
                    listItems.Add(scrView.GetChild(i).GetComponent<Item>().GetData().Replace(",","")); //remove specific word ','

                StringBuilder strBld = new StringBuilder();

                for (int i = 0; i < listItems.Count; i++)
                {
                    if (i == listItems.Count - 1)
                        strBld.Append(listItems[i]);
                    else
                        strBld.Append(listItems[i] + ',');
                }
                listItems.Clear(); //clear unsing

                Common.GetInstance.PrintLog('d', "final data", strBld.ToString());

                //check is insert or update?
                if(string.IsNullOrEmpty(strBld.ToString()))
                {
                    Debug.LogWarning("dd : " + selectedDay);
                    SqliteMgr.GetInstance.RemoveRows("delete from inout where date = '" + selectedDay + "';");
                    //delete from inout where date = '2021-12-01'
                }
                else
                {
                    if (isDataUpdate)
                        SqliteMgr.GetInstance.UpdateData("update inout set data = '" + strBld + "' where date = '" + selectedDay + "';");
                    else
                        SqliteMgr.GetInstance.InsertData("insert into inout values('" + selectedDay + "','" + strBld + "');");
                }
                
                //refresh data
                this.GetComponent<CalendarController>().CheckDataOnDay();
                break;
            case "close":
                //close detail list
                for(int i=1; i<scrView.childCount-1; i++)
                    Destroy(scrView.GetChild(i).gameObject);

                //clear daily in/out value
                //## hard coding
                this.GetComponent<Editor>().transitEditor.GetChild(0).GetComponent<TotalValue>().ClearValue();
                break;
        }
    }

    public void DayEvent(string item, string date) //click event each day
    {
        selectedDay = date;// date.Split('-'); //2021-03-24

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

            if (string.IsNullOrEmpty(existData)) //routine out when data null
                return;

            isDataUpdate = true; //for db update or insert
            Common.GetInstance.PrintLog('w', "exist data", existData); //return 1 line only

            //json parsing
            string[] strSplit = existData.Split(','); //split string comma like a CSV
            for (int i=0; i< strSplit.Length; i++)
            {
                //Debug.LogWarning("dd : " + jData[(i + 1).ToString()].ToString());

                //cpy gameobject
                GameObject items = Instantiate(Resources.Load("Prefebs/Item"), scrView) as GameObject;
                items.name = "item_" + i;
                items.SetActive(true);
                items.transform.SetSiblingIndex(scrView.childCount - 2); //set hierarchy order
                items.transform.localScale = Vector3.one;

                //insert data to items
                string[] tmpStr = new string[2];
                tmpStr = strSplit[i].ToString().Split('#');
                transitEditor.GetComponentInChildren<TotalValue>().TotalValues(int.Parse(tmpStr[1])); //extract only value

                items.GetComponent<Item>().InsertData(tmpStr[0], tmpStr[1]); //insert data each items
            }
        }
    }
}