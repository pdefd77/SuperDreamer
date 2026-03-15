using System;
using UnityEngine;

public class AssetManager
{
    public GameObject[] Enemies => enemies;
    private GameObject[] enemies; // 적 모음
    public StageSO[] Stages => stages;
    private StageSO[] stages; // 적 모음

    public void Init()
    {
        enemies = Resources.LoadAll<GameObject>("Enemies");

        stages = Resources.LoadAll<StageSO>("Stages");
        Array.Sort(stages, (a, b) => a.stage.CompareTo(b.stage)); // 스테이지 순서대로 정렬
    }
}
