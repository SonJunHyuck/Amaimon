using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.WSA;

public class GameDataManager : MonoBehaviour
{
    public Dictionary<int, FieldData> fieldDataDic;
    public Dictionary<int, MonsterData> monsterDataDic;
    public Dictionary<int, MonsterAreaData> monsterAreaDataDic;

    private void Awake()
    {
        LoadData();
    }

    private void LoadData()
    {
        StringBuilder path = new StringBuilder();

        //.. Field
        FieldData[] fieldDataArray;
        MonsterData[] monsterDataArray;
        MonsterAreaData[] monsterAreaDataArray;

        try
        {
            fieldDataArray = Resources.LoadAll<FieldData>("ScriptableDatas/Field");
            if (fieldDataArray == null)
                throw new System.Exception("Fail Load Field Data");

            monsterDataArray = Resources.LoadAll<MonsterData>("ScriptableDatas/Monster");
            if (monsterDataArray == null)
                throw new System.Exception("Fail Load Monster Data");

            monsterAreaDataArray = Resources.LoadAll<MonsterAreaData>("ScriptableDatas/MonsterArea");
            if (monsterAreaDataArray == null)
                throw new System.Exception("Fail Load MonsterArea Data");
        }
        catch (System.Exception err)
        {
            Debug.Log(err.Message);
            return;
        }

        //.. Field
        fieldDataDic = new Dictionary<int, FieldData>();
        for (int i = 0; i < fieldDataArray.Length; i++)
        {
            FieldData fieldData = fieldDataArray[i];
            fieldDataDic.Add(fieldData.fieldId, fieldData);
        }

        //.. Monster
        monsterDataDic = new Dictionary<int, MonsterData>();
        for (int i = 0; i < monsterDataArray.Length; i++)
        {
            MonsterData monsterData = monsterDataArray[i];
            monsterDataDic.Add(monsterData.monsterId, monsterData);
        }

        //.. Monster Area
        monsterAreaDataDic = new Dictionary<int, MonsterAreaData>();
        for (int i = 0; i < monsterAreaDataArray.Length; i++)
        {
            MonsterAreaData monsterAreaData = monsterAreaDataArray[i];
            monsterAreaDataDic.Add(monsterAreaData.monsterAreaId, monsterAreaData);
        }
    }
}
