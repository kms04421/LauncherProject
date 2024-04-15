using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class ItemSlot : ItemBase
{
    public byte itemCode;

    public int itemQuantity;
    public byte ItemCode { 
        get { return itemCode; } 
        set { itemCode = value; } }
    public int ItemQuantity { get; set; }

    public int itemNumber;
    public bool isItemUsable;
    public GameObject highlightObj;
    public Image image;
    public int GetCode()
    {
        return itemCode;
    }
}
