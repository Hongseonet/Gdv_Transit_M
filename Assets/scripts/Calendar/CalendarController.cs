using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarController : MonoBehaviour
{
    public GameObject _calendarPanel;
    public Text _yearNumText;
    public Text _monthNumText;

    [SerializeField] Button btnYearPrev, btnMonthPRev, btnYearNext, btnMonthNext;
    [SerializeField] Text txtMonthOut;

    public GameObject refItem;

    public List<GameObject> _dateItems = new List<GameObject>();
    const int _totalDateNum = 42;

    private DateTime _dateTime;
    public static CalendarController _calendarInstance;

    GameObject pastObj; //for effect of activate selected day
    
    
    public void Init()
    {
        _calendarInstance = this;
        Vector3 startPos = refItem.transform.localPosition;
        _dateItems.Clear();
        _dateItems.Add(refItem);

        //int itemWidth = Screen.width / 7;
        int itemWidth = 1080 / 7;

        //generate day items
        for (int i = 1; i < _totalDateNum; i++)
        {
            GameObject item = GameObject.Instantiate(refItem) as GameObject;
            item.name = "Dummy_" + i.ToString("D2"); //set temporary name
            item.transform.SetParent(refItem.transform.parent);
            item.transform.localScale = Vector3.one;
            item.transform.localRotation = Quaternion.identity;

            //post process by scr width
            //item.transform.localPosition = new Vector3((i % 7) * 155  + startPos.x, startPos.y - (i / 7) * 80, startPos.z);
            item.transform.localPosition = new Vector3((i % 7) * itemWidth + startPos.x, startPos.y - (i / 7) * 100, startPos.z);
            item.GetComponent<RectTransform>().sizeDelta = new Vector2(itemWidth, 100f);

            _dateItems.Add(item);
        }

        _dateTime = DateTime.Now;

        CreateCalendar();
    }

    private void OnEnable()
    {
        btnYearNext.onClick.AddListener(() => CalndarCtrlEVent(btnYearNext));
        btnYearPrev.onClick.AddListener(() => CalndarCtrlEVent(btnYearPrev));
        btnMonthNext.onClick.AddListener(() => CalndarCtrlEVent(btnMonthNext));
        btnMonthPRev.onClick.AddListener(() => CalndarCtrlEVent(btnMonthPRev));
    }

    private void OnDisable()
    {
        btnYearNext.onClick.RemoveAllListeners();
        btnYearPrev.onClick.RemoveAllListeners();
        btnMonthNext.onClick.RemoveAllListeners();
        btnMonthPRev.onClick.RemoveAllListeners();
    }

    void CreateCalendar()
    {
        DateTime firstDay = _dateTime.AddDays(-(_dateTime.Day - 1));
        int index = GetDays(firstDay.DayOfWeek);

        int date = 0;
        for (int i = 0; i < _totalDateNum; i++)
        {
            Text label = _dateItems[i].GetComponentInChildren<Text>();
            _dateItems[i].SetActive(false);

            if (i >= index)
            {
                DateTime thatDay = firstDay.AddDays(date);
                if (thatDay.Month == firstDay.Month)
                {
                    _dateItems[i].SetActive(true);
                    _dateItems[i].name = "Day_" + (date + 1).ToString("D2");

                    label.text = (date + 1).ToString();
                    date++;
                }
            }
        }
        _yearNumText.text = _dateTime.Year.ToString();
        _monthNumText.text = _dateTime.Month.ToString("D2");

        CheckDataOnDay(); //get data on current month
    }

    public void ClearCalendar()
    {
        //clear date table
        Transform dayRoot = refItem.transform.parent;
        for(int i=0; i<dayRoot.childCount; i++)
            dayRoot.GetChild(i).GetComponent<DayData>().SetOutTotal(0);
    }

    int GetDays(DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Sunday: return 0; //sunday first
            case DayOfWeek.Monday: return 1;
            case DayOfWeek.Tuesday: return 2;
            case DayOfWeek.Wednesday: return 3;
            case DayOfWeek.Thursday: return 4;
            case DayOfWeek.Friday: return 5;
            case DayOfWeek.Saturday: return 6;
        }
        return -1;
    }

    void CalndarCtrlEVent(Button btn)
    {
        switch (btn.name.Split('_')[1])
        {
            case "YearPrev":
                YearPrev();
                break;
            case "YearNext":
                YearNext();
                break;
            case "MonthPrev":
                MonthPrev();
                break;
            case "MonthNext":
                MonthNext();
                break;
        }
        CreateCalendar();
    }

    public void YearPrev()
    {
        _dateTime = _dateTime.AddYears(-1);
        //CreateCalendar();
    }

    public void YearNext()
    {
        _dateTime = _dateTime.AddYears(1);
        //CreateCalendar();
    }

    public void MonthPrev()
    {
        _dateTime = _dateTime.AddMonths(-1);
        //CreateCalendar();
    }

    public void MonthNext()
    {
        _dateTime = _dateTime.AddMonths(1);
        //CreateCalendar();
    }

    //get date string selected date
    public void OnDateItemClick(GameObject day)
    {
        if (pastObj == null)
            pastObj = day;
        else if(pastObj != day)
        {
            Common.GetInstance.SetColor(pastObj.GetComponent<Image>(), Color.white);
            pastObj = day;
        }
            
        Common.GetInstance.SetColor(day.GetComponent<Image>(), new Color(255, 0, 0, 255));

        string strDay = day.transform.GetComponentInChildren<Text>().text;
        //_target.text = _yearNumText.text + "-" + _monthNumText.text + "-" + int.Parse(day).ToString("D2");
        this.GetComponent<Editor>().DayEvent(day.name, _dateTime.Year.ToString() + "-" + _dateTime.Month.ToString("D2") + "-" + int.Parse(strDay).ToString("D2"));
    }

    public void CheckDataOnDay() //scan history on current month
    {
        ClearCalendar(); //clear past data

        //scan data at this month
        List<string> rtnData = SqliteMgr.GetInstance.ReadData("select date, data from inout where date like '" + _dateTime.Year.ToString() + "-" + _dateTime.Month.ToString("D2") + "%' order by date asc", 2);
        int monthSum = 0; //total month out

        for (int i = 0; i < rtnData.Count; i++)
        {
            int daySum = 0; //daily out total

            //Debug.LogWarning("dd : " + rtnData[i]); //eg 2021-02-10\식비#-1000,커피#-1200000
            string[] dateData = rtnData[i].Split('\\'); //classfy between date and data
            string[] items = dateData[1].Split(','); //classfy all items

            //Debug.Log("dd : " + rtnData[i].Split('-')[2].Substring(0, 2));
            Transform target = refItem.transform.parent.Find("Day_" + rtnData[i].Split('-')[2].Substring(0, 2));
            //Debug.Log("dd : " + target.name);

            if (target != null)
            {
                Debug.Log("exist data on day : " + target.name);
                for (int c = 0; c < items.Length; c++)
                {
                    //Debug.LogWarning("ddf : " + items[c]);
                    string[] item = items[c].Split('#');

                    try
                    {
                        if (int.Parse(item[1].Replace(",", "")) < 0)
                            daySum += int.Parse(item[1]);
                    }catch(Exception e)
                    {
                        Debug.LogWarning("conert err : " + e.Message);
                    }
                }
                target.GetComponent<DayData>().SetOutTotal(daySum);
                monthSum += daySum;
            }
        }
        txtMonthOut.text = string.Format("{0:N0}", monthSum);
        rtnData.Clear();
    }
}