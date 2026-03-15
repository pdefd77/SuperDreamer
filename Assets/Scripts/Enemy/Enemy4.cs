using UnityEngine;

public class Enemy4 : Enemy1
{
    protected override void Init()
    {
        base.Init();

        maxHp = 1000f + Managers.Stage.Stage * 10f; // 5스테이지 보스: 체력이 많고 이동속도 감소
        hp = maxHp;
    }

    protected override void Die()
    {
        isDie = true;
        Managers.Stage.EnemyDie(10);
        Destroy(gameObject);
    }
}
