using UnityEngine;
using UnityEngine.UI;

public class AddItem : MonoBehaviour
{
    [SerializeField] Button btnAdd;
    Transform parentTrs;

    
    private void OnEnable()
    {
        if (parentTrs == null)
            parentTrs = this.transform.parent;

        btnAdd.onClick.AddListener(BtnEvent); //add button only
    }

    private void OnDestroy()
    {
        btnAdd.onClick.RemoveAllListeners();
    }

    void BtnEvent()
    {
        int childCnt = parentTrs.childCount - 1;

        //add item
        GameObject items = Instantiate(parentTrs.GetChild(0).gameObject);
        items.name = "item_" + childCnt;
        items.SetActive(true);
        items.transform.SetParent(parentTrs);
        items.transform.SetSiblingIndex(parentTrs.childCount - 2); //set hierarchy order
        items.transform.localScale = Vector3.one;
    }
}