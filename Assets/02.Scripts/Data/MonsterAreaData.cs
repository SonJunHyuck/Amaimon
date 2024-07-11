using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Object/MonsterArea")]
public class MonsterAreaData : ScriptableObject
{
    [Header("MonsterArea")]
    public int monsterAreaId;

    [Header("Spawn")]
    public int maxMonsterCount;
    public float spawnInterval;
    public List<int> monsterIdList;
}
