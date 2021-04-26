using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] Button btnRemove;
    [SerializeField] Button btnDeposit;
    [SerializeField] InputField inpScript, inpValue;

    bool isDeposit;


    private void OnEnable()
    {
        btnRemove.onClick.AddListener(() => BtnEvent(btnRemove));
        btnDeposit.onClick.AddListener(() => BtnEvent(btnDeposit));
    }

    private void OnDestroy()
    {
        btnRemove.onClick.RemoveAllListeners();
    }

    public void InsertData(string script, string value) //fill down the exist data
    {
        int valueCvt;

        if (int.Parse(value) < 0)
            valueCvt = int.Parse(value.Substring(1, value.Length - 1));
        else
            valueCvt = int.Parse(value);

        inpScript.text = script;
        inpValue.text = string.Format("{0:N0}", valueCvt);

        //this.transform.parent.getchi
        //base.TotalValue += int.Parse(inpValue.text);
    }

    public string GetData()
    {
        char addWord; //for is deposit
        if (!isDeposit)
            addWord = '-';
        else
            addWord = '+';

        return inpScript.text + '#' + addWord + inpValue.text;
    }

    void BtnEvent(Button btn)
    {
        switch (btn.name.Split('_')[1])
        {
            case "Remove":
                //check saved data, didn't yet 
                Destroy(this.gameObject);
                break;
            case "Deposit":
                isDeposit = !isDeposit;

                string spriteImage;
                if (isDeposit)
                    spriteImage = "Images/btnPlus";
                else
                    spriteImage = "Images/btnMinus";

                Common.GetInstance.SetImage(spriteImage, btn.GetComponent<Image>());
                
                break;
        }
    }
}