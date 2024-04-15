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
            itemSlot[i] = new ItemSlot(); // 각 요소를 새로운 객체로 초기화 해줘야 접근시 NullReferenceException안나옴
        }

        SetDefaults();
    }
   
    private void inventoryAdd()
    {
        inventory.OnItemAdded += ItemSlotAdd;
    }

    public void ItemSlotAdd(ItemBase item)
    {  
        // 아이템 슬롯에 옵저버 등록
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
    public int SlotDuplicatecheck(byte itemCode) // 슬롯 중복 검사 
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
    private void ItemSet() // 슬롯정보 배열에 담기
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
    public void NumberInput(int number) // 하이라이트 효과주기
    {
        itemSlot[playerItemNumber].highlightObj.SetActive(false);
        playerItemNumber = number;
        itemSlot[playerItemNumber].highlightObj.SetActive(true);
        
    }

    public Sprite SpriteReturn(string spriteName) // 스프라이트 아틀라스 정보 반환
    {
        return spriteAtlas.GetSprite(spriteName);
    }

    

}
