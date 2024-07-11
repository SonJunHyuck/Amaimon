using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Object/Field")]
public class FieldData : ScriptableObject
{
    public enum FieldType
    {
        Town,
        Wild   
    }

    [Header("Field")]
    public int fieldId;
    public string fieldName;
    FieldType fieldType;

    [Header("NPC")]
    public int[] NPC;

    [Header("Monster Spawn")]
    public List<int> monsterAreaList;
}
