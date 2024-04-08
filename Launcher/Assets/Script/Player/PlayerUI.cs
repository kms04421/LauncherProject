using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerUI : MonoBehaviour
{
    int playerItemNumber = 1;
  
    public void NumberInput(InputAction.CallbackContext context)
    {       
            playerItemNumber = int.Parse(context.control.name);
          
    }
}
