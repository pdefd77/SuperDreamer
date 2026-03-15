using System.Collections.Generic;
using UnityEngine;

public class StageManager
{
    public StageAdmin stageAdmin;

    public int Stage
    {
        get
        {
            return stage;
        }
        set
        {
            stage = value;
        }
    }
    private int stage = 0; // 스테이지 번호

    public int EnemyCount
    {
        get
        {
            return enemyCount;
        }
        set
        {
            enemyCount = value;
        }
    }
    private int enemyCount; // 클리어 판별 용 남은 적 수
    public Queue<GameObject> EnemyQueue => enemyQueue;
    private Queue<GameObject> enemyQueue = new(); // 남은 적 큐

    public void StartStage() // 스테이지 시작
    {
        if (stage == 10) // 최종 스테이지까지 깨면 로비로
        {
            Debug.Log("Clear");
            Managers.SceneFlow.GotoScene("Lobby");
            return;
        }

        stageAdmin.StartStage(stage); // 인덱스0 시작이라 실행한 뒤 stage 값 추가
        stage++;

        Managers.PlayerStatus.character?.StartStageDirect();
    }

    public void EndStage() // 현재 스테이지 클리어
    {
        Managers.PlayerStatus.character.EndStageDirect();
    }

    public GameObject EnemySpawn(EnemyType enemyType)
    {
        return Managers.Asset.Enemies[(int)enemyType];
    }

    public void EnemyDie(int gold)
    {
        enemyCount--;
        Managers.PlayerStatus.Gold += gold;
        enemyQueue.Dequeue();
        if (enemyCount <= 0)
        {
            EndStage();
        }
    }
}
