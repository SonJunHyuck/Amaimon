using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Object/Monster")]
public class MonsterData : ScriptableObject
{
    public enum MonsterType
    {
        Herbivore,
        Predator
    }

    public int monsterId;
    public string monsterName;
    public MonsterType monsterType;
}
