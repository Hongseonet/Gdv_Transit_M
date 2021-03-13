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

    Text _target;


    void Start()
    {
        _calendarInstance = this;
        Vector3 startPos = refItem.transform.localPosition;
        _dateItems.Clear();
        _dateItems.Add(refItem);

        int itemWidth = Screen.width / 7;
        for (int i = 1; i < _totalDateNum; i++)
        {
            GameObject item = GameObject.Instantiate(refItem) as GameObject;
            item.name = "Day_" + i.ToString("D2");
            item.transform.SetParent(refItem.transform.parent);
            item.transform.localScale = Vector3.one;
            item.transform.localRotation = Quaternion.identity;

            //post process by scr width
            //item.transform.localPosition = new Vector3((i % 7) * 155  + startPos.x, startPos.y - (i / 7) * 80, startPos.z);
            item.transform.localPosition = new Vector3((i % 7) * itemWidth + startPos.x, startPos.y - (i / 7) * 100, startPos.z);

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
        CheckDataOnDay(); //get data on current month

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

                    label.text = (date + 1).ToString();
                    date++;
                }
            }
        }
        _yearNumText.text = _dateTime.Year.ToString();
        _monthNumText.text = _dateTime.Month.ToString("D2");
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

    public void ShowCalendar(Text target)
    {
        _calendarPanel.SetActive(true);
        _target = target;
        //_calendarPanel.transform.position = new Vector3(965, 475, 0);//Input.mousePosition-new Vector3(0,120,0);
    }

    
    //get date string selected date
    public void OnDateItemClick(GameObject day)
    {
        string strDay = day.transform.GetComponentInChildren<Text>().text;
        //_target.text = _yearNumText.text + "-" + _monthNumText.text + "-" + int.Parse(day).ToString("D2");
        //Common.GetInstance.PrintLog('w', "OnDateItemClick", _yearNumText.text + "-" + _monthNumText.text + "-" + int.Parse(strDay).ToString("D2"));
        this.GetComponent<Editor>().DayEvent(day.name, _dateTime.Year.ToString() + "-" + _dateTime.Month.ToString("D2") + "-" + int.Parse(strDay).ToString("D2"));

        //_calendarPanel.SetActive(false);
    }

    public void CheckDataOnDay() //scan history on cur month
    {
        //scan data at this month
        List<string> rtnData = SqliteMgr.GetInstance.ReadData("select date, data from inout where date like '" + _dateTime.Year.ToString() + "-" + _dateTime.Month.ToString("D2") + "%' order by date asc", 2);
        int monthSum = 0; //total month out

        //for (int i = 0; i < rtnData.Count; i++)
        for (int i = 0; i < rtnData.Count; i++)
        {
            int daySum = 0; //total out only

            //Debug.LogWarning("dd : " + rtnData[i]); //eg 2021-02-10\식비#-1000,커피#-1200000
            string[] dateData = rtnData[i].Split('\\'); //classfy between date and data
            string[] items = dateData[1].Split(','); //classfy all items

            Transform target = refItem.transform.parent.Find("Day_" + rtnData[i].Split('-')[2].Substring(0, 2));
            if (target != null)
            {
                for (int c = 0; c < items.Length; c++)
                {
                    //Debug.LogWarning("ddf : " + items[c]);
                    string[] item = items[c].Split('#');

                    if (int.Parse(item[1].Replace(",","")) < 0)
                        daySum += int.Parse(item[1]);

                    target.GetComponent<DayData>().SetOutTotal(daySum);
                }
                monthSum += daySum;
            }
        }
        txtMonthOut.text = string.Format("{0:N0}", monthSum);
        rtnData.Clear();
    }
}