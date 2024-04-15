using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Inventory 
{
    public event Action<ItemBase> OnItemAdded; // �������� �߰��� �� �߻��ϴ� �̺�Ʈ

    public void AddItem(ItemBase item)
    {
        // ������ �߰� �� �̺�Ʈ ȣ��
        OnItemAdded?.Invoke(item);
    }
}
