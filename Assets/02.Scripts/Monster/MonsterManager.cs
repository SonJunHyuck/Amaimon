using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.AI.Navigation;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class MonsterManager : MonoBehaviour
{
    public Transform monsterArea;
    public Transform[] monsterSpawnPools;  // Monster Group

    //public List<MonsterGroup> monsterGroupList;
    public Vector3[] monsterZone;
    public NavMeshSurface[] navMeshSurfaces;

    public BossMapInfo[] bossMap;
    public Transform[] bossParentTrans;
    public Vector3[] bossZone;

    BossMapInfo currentBoss;

    public UIManager uiM;

    RewardPackage bossPackage;

    [Header("▶ Enemy Info")]
    public GameObject mEnemyInfo;
    public Transform mEnemyHPParent;

    void Start()
    {
        //monsterGroupList = new List<MonsterGroup>() ;
        monsterZone = new Vector3[monsterSpawnPools.Length];

        for(int fieldIdx = 0; fieldIdx < 2; fieldIdx++)
        {
            FieldData fieldData = GameManager.instance.GetFieldData(fieldIdx);

            foreach (int areaId in fieldData.monsterAreaList)
            {
                MonsterAreaData areaData = GameManager.instance.GetMonsterAreaData(areaId);
                Transform area = monsterArea.GetChild(areaId);
                area.GetComponentInChildren<SpawnArea>().InitMonsterSpanwer(areaId);
                
                monsterZone[areaData.monsterAreaId] = monsterSpawnPools[areaData.monsterAreaId].GetChild(0).localScale;
            }
        }

        bossZone = new Vector3[bossParentTrans.Length];
        for (int i = 0; i < bossParentTrans.Length; i++)
        {
            bossZone[i] = bossParentTrans[i].GetChild(0).localScale;
        }
    }

    void Update()
    {
        // Boss Reward
        if(currentBoss != null && currentBoss.bossEntity.bDead && !currentBoss.completed)
        {
            bossPackage = currentBoss.GetRewardPakage();
            currentBoss.EndBattle();
            currentBoss = null;
            uiM.DeactiveBossState();
            uiM.ActiveRewardUI(bossPackage);
        }

        UpdateMonsterHPBar();
    }

    void UpdateMonsterHPBar()
    {
        //for (int i = 0; i < monsterGroupList.Count; i++)
        //{
        //    for (int j = 0; j < monsterGroupList[i].monsterCount; j++)
        //    {
        //        MonsterGroup monsterGroup = monsterGroupList[i];
                
        //        // 몬스텨 존재
        //        if (monsterGroup.monsterDatas[j].transform != null)
        //        {
        //            // 맞았을 때
        //            if (monsterGroup.monsterDatas[j].entity.bIsHit)
        //            {
        //                // 이미지가 있다면 업데이트, 없으면 새로 만들어주기
        //                if(monsterGroup.monsterDatas[j].hpBarImage != null)
        //                {
        //                    monsterGroup.monsterDatas[j].hpBar.position = Camera.main.WorldToScreenPoint(monsterGroup.monsterDatas[j].transform.position) + Vector3.up * 100f;
        //                    monsterGroup.UpdateHPImage(j);
        //                }
        //                else
        //                {
        //                    GameObject hpBar = Instantiate(mEnemyInfo, Vector3.zero, Quaternion.identity);
        //                    hpBar.transform.parent = mEnemyHPParent;
        //                    monsterGroup.AddHPImage(j, hpBar);
        //                }
        //            }
        //            else
        //            {
        //                if (monsterGroup.monsterDatas[j].hpBarImage != null)
        //                {
        //                    // 없애기
        //                    Destroy(monsterGroup.monsterDatas[j].hpBar.gameObject);
        //                    monsterGroup.RemoveHPImage(j); ;
        //                }
        //            }
        //        }
        //        else if(monsterGroup.monsterDatas[j].transform == null)
        //        {
        //            if(monsterGroup.monsterDatas[j].hpBarImage != null)
        //            {
        //                // 몬스터 없으면 이미지도 같이 없애기
        //                Destroy(monsterGroup.monsterDatas[j].hpBar.gameObject);
        //            }               
        //        }
        //    }
        //}
    }

    public void EnterBossZone(int idx)
    {
        currentBoss = bossMap[idx];
    }
}

[System.Serializable]
public class MonsterGroup
{
    public struct MonsterData
    {
        public Transform transform;
        public Transform hpBar;
        public Image hpBarImage;
        public Vector3 position
        {
            get
            {
                return transform.localPosition;
            }
        }
        public LivingEntity entity
        {
            get
            {
                return transform.GetComponent<LivingEntity>();
            }
        }
    }

    public int prefabId;
    public int monsterCount;
    public MonsterData[] monsterDatas;
    public bool isBoss;

    public MonsterGroup(Transform[] mon, bool bIsBoss = false)
    {
        monsterCount = mon.Length;
        monsterDatas = new MonsterData[monsterCount];

        for (int i = 0; i < monsterCount; i++)
        {
            monsterDatas[i].transform = mon[i];
            //monsterDatas[i].position = mon[i].localPosition;
            //monsterDatas[i].entity = mon[i].GetComponent<LivingEntity>();
            monsterDatas[i].hpBar = null;
            monsterDatas[i].hpBarImage = null;
        }

        this.isBoss = bIsBoss;
    }

    public void SetOriginPosition(int idx, GameObject prefab)
    {
        prefab.transform.localPosition = monsterDatas[idx].position;
    }

    public void UpdateHPImage(int idx)
    {
        monsterDatas[idx].hpBarImage.fillAmount = monsterDatas[idx].entity.mHealth / (float)monsterDatas[idx].entity.startingHealth;
    }

    public void AddHPImage(int idx, GameObject hpBar)
    {
        monsterDatas[idx].hpBar = hpBar.transform;
        monsterDatas[idx].hpBar.GetChild(1).GetComponent<TextMeshProUGUI>().text = monsterDatas[idx].entity.mName;
        monsterDatas[idx].hpBarImage = hpBar.transform.GetChild(0).GetChild(0).GetComponent<Image>() ;

    }

    public void RemoveHPImage(int idx)
    {
        monsterDatas[idx].hpBar = null;
        monsterDatas[idx].hpBarImage = null;
    }
}
