using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineHelper
{   
    private static Dictionary<float, WaitForSeconds> _WaitForSeconds = new Dictionary<float, WaitForSeconds>();
    
    public static WaitForSeconds WaitForSeconds(float seconds) //입력한 코루틴이 존재하면 존재하는 코루틴을 반환 없으면 새로생성
    {
        if (_WaitForSeconds.TryGetValue(seconds, out var waitForSeconds))
        {
            return waitForSeconds;
        }
        else
        {
            waitForSeconds = new WaitForSeconds(seconds);

            _WaitForSeconds.Add(seconds, waitForSeconds);

            return waitForSeconds;
        }
    }
}
