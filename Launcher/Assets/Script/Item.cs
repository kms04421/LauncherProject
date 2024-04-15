using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ItemBase
{
    public byte ItemCode { get; set; }
    public int ItemQuantity { get; set; }

    public byte ItemByteconversion(byte byteNum)
    {
        ItemCode = byteNum;
        return ItemCode;
    }

}
