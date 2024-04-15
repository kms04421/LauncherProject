using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Inventory 
{
    public event Action<ItemBase> OnItemAdded; // 아이템이 추가될 때 발생하는 이벤트

    public void AddItem(ItemBase item)
    {
        // 아이템 추가 후 이벤트 호출
        OnItemAdded?.Invoke(item);
    }
}
