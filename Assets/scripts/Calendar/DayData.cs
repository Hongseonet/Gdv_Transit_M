using UnityEngine;
using UnityEngine.UI;

public class DayData : MonoBehaviour
{
    [SerializeField] Text txtTotal;
    [SerializeField] Image imgCheck;


    public void SetOutTotal(int dayTotal)
    {
        if (dayTotal < 0)
            txtTotal.text = string.Format("{0:N0}", dayTotal);
        else
            txtTotal.text = "";
    }
}