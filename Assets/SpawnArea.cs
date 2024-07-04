using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    public Vector3 GetValidPosition()
    {
        // 생성 구 선택
        int areaIdx = Random.Range(0, transform.childCount);
        Transform area = transform.GetChild(areaIdx);

        // 구의 반지름을 기준으로 랜덤하게 위치 설정
        float radius = area.localScale.x * 0.5f;  // y, z 동일
        int randomSeed = (int)(Random.Range(0, 1000));
        Random.InitState(randomSeed);
        Vector3 random = new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius));

        Debug.Log(radius);

        return area.position + random;
    }
}
