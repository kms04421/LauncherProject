using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineHelper
{   
    private static Dictionary<float, WaitForSeconds> _WaitForSeconds = new Dictionary<float, WaitForSeconds>();
    
    public static WaitForSeconds WaitForSeconds(float seconds) //�Է��� �ڷ�ƾ�� �����ϸ� �����ϴ� �ڷ�ƾ�� ��ȯ ������ ���λ���
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
