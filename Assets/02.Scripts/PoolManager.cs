using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    public GameObject[] prefabs;

    private List<GameObject>[] pools;

    private void Awake()
    {
        instance = this;

        // .. TODO : CSV로 몬스터 이름 다 받아오고, 여기서 프리펩 세팅 (List가 배열이 아니라 Dictionary로 이루어져야함)
        pools = new List<GameObject>[prefabs.Length];

        for(int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<GameObject>();
        }
    }

    public GameObject Get(int index)
    {
        GameObject select = null;

        // .. 비활성화 게임오브젝트에 접근 시도 in selected pool
        foreach(GameObject item in pools[index])
        {
            if(!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                return select;
            }
        }

        // .. if not found
        if(select == null)
        {
            select = Instantiate(prefabs[index], transform);
            pools[index].Add(select);
        }

        return select;
    }    
}
