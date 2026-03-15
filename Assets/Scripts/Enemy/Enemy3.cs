using UnityEngine;

public class Enemy3 : Enemy1
{
    protected override void Init()
    {
        base.Init();

        reductionRate = 0.25f; // enemy3은 넉백 전까지 데미지 감소 효과를 얻음
    }

    protected override void Die()
    {
        isDie = true;
        Managers.Stage.EnemyDie(4);
        Destroy(gameObject);
    }
}