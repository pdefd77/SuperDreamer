using System.Collections;
using UnityEngine;

// 스테이지 관리
public class StageAdmin : MonoBehaviour
{
    private Vector2 startPos = new(10f, 0f); // 처음 적이 소환되는 위치

    private void Awake()
    {
        try { Managers.Stage.stageAdmin = this; }
        catch { }
    }

    void Start()
    {
        Managers.Stage.Stage = 0;
        Managers.Stage.StartStage();
    }

    public void StartStage(int stage)
    {
        StartCoroutine(SpawnEnemies(Managers.Asset.Stages[stage]));
    }

    private IEnumerator SpawnEnemies(StageSO nowStage) // 현재 스테이지 적 소환
    {
        Managers.Stage.EnemyCount = 0;
        foreach (EnemyWave enemyWave in nowStage.EnemyWaves)
        {
            Managers.Stage.EnemyCount += enemyWave.count; // 현재 스테이지 적 수 설정
        }

        yield return new WaitForSeconds(0.5f);
        Vector2 nowPos = startPos;

        foreach (EnemyWave enemyWave in nowStage.EnemyWaves)
        {
            for(int i = 0; i < enemyWave.count; i++)
            {
                Managers.Stage.EnemyQueue.Enqueue(Instantiate(Managers.Stage.EnemySpawn(enemyWave.enemyType), nowPos, Quaternion.identity));
                nowPos += Vector2.right * 0.5f;
            }
        }

        yield break;
    }
}
