using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("base 오브젝트")]
    [SerializeField] private GameObject baseObj; // 기본 오브젝트 프리팹

    [Header("맵 정보")]
    public float waveAmplitude = 20; // 파장
    public float maxHeight = 10; // 파장 최대높이
    private const int scale_X = 100;// X사이즈
    private const int scale_Z = 100;// Y사이즈

    private List<GameObject> baseObjList = new List<GameObject>(); // 베이스 리스트
    void Start()
    {
        BuildMap();
    }

    // 맵생성 함수
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

        for (int i = 0; i < baseObjList.Count; i++) // 랜덤 맵 PerlinNoise으로 생성
        {
            float xCoord = (baseObjList[i].transform.position.x) / waveAmplitude;
            float zCoord = (baseObjList[i].transform.position.z) / waveAmplitude;
            int y = (int)(Mathf.PerlinNoise(xCoord, zCoord) * maxHeight);

            baseObjList[i].transform.position = new Vector3(baseObjList[i].transform.position.x, y, baseObjList[i].transform.position.z);
        }

        int baseObjHeight = 0; // baseObjList[i]높이 저장용

        for (int i = 0; i < baseObjList.Count; i++) // 생성한 오브젝트 하단 채우기
        {
            baseObjHeight = (int)baseObjList[i].transform.position.y;         
            if (0 < baseObjHeight)
            {
                for (int y = 0; y < baseObjHeight; y++)
                {
                    GameObject baseObjInstance = Instantiate(baseObj, new Vector3(baseObjList[i].transform.position.x, y, baseObjList[i].transform.position.z), Quaternion.identity);
                    baseObjInstance.transform.SetParent(transform);

                    if(y+2 < baseObjHeight) // 지하의 안보이는 오브젝트 비활성화
                    {
                        baseObjInstance.SetActive(false);
                    }
                }
            }
        }

    }
}
