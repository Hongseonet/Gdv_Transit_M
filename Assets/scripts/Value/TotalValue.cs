using UnityEngine;
using UnityEngine.UI;

public class TotalValue : MonoBehaviour
{
    [SerializeField] Text txtTotalIn, txtTotalOut;
    [SerializeField] Text txtMonthOut;

   public void TotalValues(int values)
    {
        int tmpValue;

        if(values == 0)
        {
            txtTotalIn.text = "0";
            txtTotalOut.text = "0";

            return;
        }
        else if (values > 0)
        {
            tmpValue = int.Parse(txtTotalIn.text.Replace(",","")) + values;
            txtTotalIn.text = string.Format("{0:N0}", tmpValue);
        }
        else
        {
            tmpValue = int.Parse(txtTotalOut.text.Replace(",", "")) + values;
            txtTotalOut.text = string.Format("{0:N0}", tmpValue);
        }

        //
        //Debug.LogWarning("aaa " + string.Format("{0:N0}", int.Parse(txtTotalIn.text)));
        //Debug.LogWarning("bbb " + string.Format("{0:N0}", int.Parse(txtTotalOut.text)));
    }
}