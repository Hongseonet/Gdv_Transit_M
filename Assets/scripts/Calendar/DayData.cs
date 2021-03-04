using UnityEngine;
using UnityEngine.UI;

public class DayData : MonoBehaviour
{
    [SerializeField] Text txtTotal;
    [SerializeField] Image imgCheck;


    public void SetOutTotal(int dayTotal)
    {
        txtTotal.text = string.Format("{0:N0}", dayTotal);
    }

    public void ExistInOut(bool isActive)
    {
        imgCheck.enabled = isActive;
    }
}