using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    private int areaId;
    private int[] monsterIdArray;  // 초기화 후에는 고정 -> Array

    // SpawnData
    private int maxMonsterCount;
    private int currentMonsterCount;

    private MonsterAreaData areaData;

    // .. Doing Coroutine
    private bool isSpawning;

    private void Awake()
    {
        isSpawning = false;

        // 랜덤시드 초기화
        int randomSeed = (int)(Random.Range(0, 1000));
        Random.InitState(randomSeed);
    }

    public void InitMonsterSpanwer(int inAreaId)
    {
        areaId = inAreaId;
        areaData = GameManager.instance.GetMonsterAreaData(inAreaId);

        // Area에 몬스터의 총 수가 정해져있고, 종류 상관없이 랜덤으로 생성
        monsterIdArray = new int[areaData.monsterIdList.Count];
        maxMonsterCount = areaData.maxMonsterCount;
        int idx = 0;
        foreach (int monsterId in areaData.monsterIdList)
        {
            monsterIdArray[idx++] = monsterId;
        }

        // 스폰 시작
        StartCoroutine(DoSpawn());
    }


    private IEnumerator DoSpawn()
    {
        if(isSpawning)
        {
            yield break;
        }

        isSpawning = true;

        WaitForSeconds waitForSeconds = new WaitForSeconds(areaData.spawnInterval);

        // 현재 Area의 몬스터 수를 체크하고, 몬스터가 없으면 Spawn
        while (currentMonsterCount < maxMonsterCount)
        {
            yield return waitForSeconds;

            int spawnidx = Random.Range(0, monsterIdArray.Length);
            Spawn(monsterIdArray[spawnidx]);
        }

        isSpawning = false;
    }


    private void Spawn(int inMonsterId)
    {
        GameObject spawnedMonster = PoolManager.instance.Get(inMonsterId);

        // 위치 결정
        Vector3 spawnPos = GetValidPosition();
        spawnedMonster.transform.position = spawnPos;

        // 부모 설정
        spawnedMonster.transform.SetParent(transform);

        // 초기화
        spawnedMonster.GetComponent<LivingEntity>().Init();

        // DieEvent
        spawnedMonster.GetComponent<LivingEntity>().OnDie += MonsterDie;

        currentMonsterCount++;
    }

    private void MonsterDie(Transform monster)
    {
        currentMonsterCount--;
        monster.GetComponent<LivingEntity>().OnDie -= MonsterDie;

        StartCoroutine(DoSpawn());
    }


    private Vector3 GetValidPosition()
    {
        // 생성 구 선택
        Transform area = transform.GetChild(0);
        int areaIdx = Random.Range(0, area.childCount);
        Transform sphere = area.GetChild(areaIdx);

        // 구의 반지름을 기준으로 랜덤하게 위치 설정
        float radius = sphere.localScale.x * 0.5f;  // y, z 동일
        Vector3 random = new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius));

        return sphere.position + random;
    }
}
