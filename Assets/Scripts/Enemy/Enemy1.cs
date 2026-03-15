using System.Collections;
using UnityEngine;

public enum EnemyType { enemy1, enemy2, enemy3, enemy4Boss, enemy5Boss }

public class Enemy1 : MonoBehaviour
{
    public EnemyType enemyType;

    protected Rigidbody2D rigidBody2D;

    protected float maxHp;
    protected float hp;
    protected float reductionRate; // 데미지 감소율
    public int KnockBackPower => knockBackPower;
    protected int knockBackPower;
    protected bool isDie;
    protected bool isKnockBack;

    protected virtual void Init()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();

        isDie = false;
        reductionRate = 1f;
        knockBackPower = 30;
        maxHp = 100f + Managers.Stage.Stage * 10f;
        hp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        if (isDie) return;

        hp -= damage * reductionRate;
        Color color = GetComponent<SpriteRenderer>().color;
        color.a = (hp / maxHp) * 0.9f + 0.1f;
        GetComponent<SpriteRenderer>().color = color;
        if (hp <= 0) Die();
    }

    public void KnockBack(int power)
    {
        if (!isKnockBack) StartCoroutine(KnockBackIE(power));
    }

    private IEnumerator KnockBackIE(int power) // 넉백당하면 뒤로 후퇴했다가 다시 전진
    {
        isKnockBack = true;
        rigidBody2D.linearVelocity = Vector2.right * power / 10f;

        while (rigidBody2D.linearVelocity.x >= 0)
        {
            rigidBody2D.linearVelocity += Vector2.left * 0.05f;
            yield return null;
        }

        rigidBody2D.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.2f);

        while (rigidBody2D.linearVelocity.x >= -1f)
        {
            rigidBody2D.linearVelocity += Vector2.left * 0.25f;
            yield return null;
        }

        rigidBody2D.linearVelocity = Vector2.left * 2.5f;
        isKnockBack = false;
    }

    protected virtual void Die()
    {
        isDie = true;
        Managers.Stage.EnemyDie(2);
        Destroy(gameObject);
    }

    public void LostIron() // enemy3 타입이 가드당했을 경우 약화
    {
        enemyType = EnemyType.enemy1;
        reductionRate = 1f;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        rigidBody2D.linearVelocity = Vector2.left * 2.5f;
    }
}
