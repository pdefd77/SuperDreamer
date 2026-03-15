using System.Collections;
using UnityEngine;

public class Enemy5 : Enemy1
{
    private bool isBerserk; // 폭주 상태 체크

    protected override void Init()
    {
        base.Init();

        isBerserk = false;
        maxHp = 2500f + Managers.Stage.Stage * 10f;
        hp = maxHp;

        StartCoroutine(BerserkIE());
        StartCoroutine(ColorCheckIE());
    }

    protected override void Die()
    {
        isDie = true;
        Managers.Stage.EnemyDie(20);
        Destroy(gameObject);
    }

    private IEnumerator BerserkIE() // 10스테이지 보스. 일정 시간마다 폭주 상태가 되어 방어하기 전까지 이동 속도 증가 
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            float nowTime = 0f, maxTime = 1f;
            while (nowTime <= maxTime)
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Color.red, nowTime / maxTime);
                nowTime += Time.deltaTime;
                yield return null;
            }

            isBerserk = true;
            rigidBody2D.linearVelocity = Vector2.left * 4f;
        }
    }

    private IEnumerator ColorCheckIE()
    {
        while (true)
        {
            if (isBerserk && rigidBody2D.linearVelocity.x > -3f)
            {
                isBerserk = false;
                GetComponent<SpriteRenderer>().color = Color.green;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
