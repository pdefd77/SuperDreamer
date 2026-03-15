using UnityEngine;

public class Enemy2 : Enemy1
{
    protected override void Init()
    {
        base.Init();

        knockBackPower = 12;
        maxHp = 150f + Managers.Stage.Stage * 15f; // enemy2는 넉백이 감소하고 체력이 많음
        hp = maxHp;
    }

    protected override void Die()
    {
        isDie = true;
        Managers.Stage.EnemyDie(3);
        Destroy(gameObject);
    }
}
