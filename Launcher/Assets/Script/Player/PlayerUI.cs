using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using UnityEngine.UI;
using System;
public class PlayerUI : MonoBehaviour
{
    int playerItemNumber = 1;
    private SpriteAtlas spriteAtlas;

    public Transform uiObj;

    public ItemSlot[] itemSlot;

    public Inventory inventory;
    void Start()
    {
        init();
        inventoryAdd();


    }

    private void init()
    {
        inventory = new Inventory();

        itemSlot = new ItemSlot[7];

        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i] = new ItemSlot(); // �� ��Ҹ� ���ο� ��ü�� �ʱ�ȭ ����� ���ٽ� NullReferenceException�ȳ���
        }

        SetDefaults();
    }
   
    private void inventoryAdd()
    {
        inventory.OnItemAdded += ItemSlotAdd;
    }

    public void ItemSlotAdd(ItemBase item)
    {  
        // ������ ���Կ� ������ ���
        Sprite sprite = SpriteReturn("BlockIcons_" + item.ItemCode);

        if (sprite != null)
        {         
            if(SlotDuplicatecheck(item.ItemCode) < 0)
            {               
                for (int i = 0; i < 7; i++)
                {
                    if (!itemSlot[i].image.gameObject.activeSelf)
                    {
                        itemSlot[i].itemCode = item.ItemCode;
                        itemSlot[i].image.sprite = sprite;
                        itemSlot[i].image.gameObject.SetActive(true);
                       
                        break;
                    }
                }
            }
            else
            {
            
            }
              
        }
    }
    public int SlotDuplicatecheck(byte itemCode) // ���� �ߺ� �˻� 
    {
       
        for(int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].itemCode == (itemCode)) return i; 
        }
        return -1;
    }
    public void ItemRemove()
    {

    
    }
    private void SetDefaults()
    {
   
      spriteAtlas = Resources.Load<SpriteAtlas>("ItemSprite");
      ItemSet();
    }
    private void ItemSet() // �������� �迭�� ���
    {
        int i = 0;
        foreach (Transform child in uiObj)
        {
            Image imageComponent = child.GetChild(2).GetComponentInChildren<Image>();
            if(imageComponent != null && i < 7)
            {
                itemSlot[i].image = imageComponent;
                itemSlot[i].highlightObj = child.GetChild(1).gameObject;
                itemSlot[i].image.gameObject.SetActive(false);
                i++;

            }
            
           
        }

    }
    public void NumberInput(int number) // ���̶���Ʈ ȿ���ֱ�
    {
        itemSlot[playerItemNumber].highlightObj.SetActive(false);
        playerItemNumber = number;
        itemSlot[playerItemNumber].highlightObj.SetActive(true);
        
    }

    public Sprite SpriteReturn(string spriteName) // ��������Ʈ ��Ʋ�� ���� ��ȯ
    {
        return spriteAtlas.GetSprite(spriteName);
    }

    

}
