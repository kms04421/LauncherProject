using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("base ������Ʈ")]
    [SerializeField] private GameObject baseObj; // �⺻ ������Ʈ ������

    [Header("�� ����")]
    public float waveAmplitude = 20; // ����
    public float maxHeight = 10; // ���� �ִ����
    private const int scale_X = 100;// X������
    private const int scale_Z = 100;// Y������

    private List<GameObject> baseObjList = new List<GameObject>(); // ���̽� ����Ʈ
    void Start()
    {
        BuildMap();
    }

    // �ʻ��� �Լ�
    private void BuildMap()
    {
        for (int x = 0; x < scale_X; x++)
        {
            for (int z = 0; z < scale_Z; z++)
            {
                GameObject baseObjInstance = Instantiate(baseObj, new Vector3(x, 0, z), Quaternion.identity);
                baseObjInstance.transform.SetParent(transform);
                baseObjList.Add(baseObjInstance);

            }
        }

        for (int i = 0; i < baseObjList.Count; i++) // ���� �� PerlinNoise���� ����
        {
            float xCoord = (baseObjList[i].transform.position.x) / waveAmplitude;
            float zCoord = (baseObjList[i].transform.position.z) / waveAmplitude;
            int y = (int)(Mathf.PerlinNoise(xCoord, zCoord) * maxHeight);

            baseObjList[i].transform.position = new Vector3(baseObjList[i].transform.position.x, y, baseObjList[i].transform.position.z);
        }

        int baseObjHeight = 0; // baseObjList[i]���� �����

        for (int i = 0; i < baseObjList.Count; i++) // ������ ������Ʈ �ϴ� ä���
        {
            baseObjHeight = (int)baseObjList[i].transform.position.y;         
            if (0 < baseObjHeight)
            {
                for (int y = 0; y < baseObjHeight; y++)
                {
                    GameObject baseObjInstance = Instantiate(baseObj, new Vector3(baseObjList[i].transform.position.x, y, baseObjList[i].transform.position.z), Quaternion.identity);
                    baseObjInstance.transform.SetParent(transform);

                    if(y+2 < baseObjHeight) // ������ �Ⱥ��̴� ������Ʈ ��Ȱ��ȭ
                    {
                        baseObjInstance.SetActive(false);
                    }
                }
            }
        }

    }
}
