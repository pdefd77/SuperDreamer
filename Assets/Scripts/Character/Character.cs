using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    private InputSystem_Actions inputActions;

    private InputAction attackInputAction;
    private InputAction guardInputAction;
    private InputAction moveInputAction;
    private InputAction skillAction;

    private Rigidbody2D rigidBody2D;

    public Collider2D attackCollider2D; // 공격범위
    public Collider2D guardCollider2D; // 방어범위
    private ContactFilter2D enemyFilter;

    private Vector2 startPos = new(-6.5f, 0f); // 스테이지 시작할 때 캐릭터 위치
    private Vector2 endPos = new(12f, 0f); // 스테이지 끝날 때 캐릭터 위치 (연출 상 안보여야 하므로 화면 바깥)

    public int IsSkillAvailable => isSkillAvailable;
    private int isSkillAvailable;  // 0 스킬 없음, 1 스킬 준비, 2 스킬 사용 중
    private bool isUnstoppable; // 방패 스킬 사용 여부
    private int isGuard;  // 0 가드 안함, 1 가드 대기, 2 가드 중
    private IEnumerator guardIE;
    private IEnumerator holdAttackIE; // 꾹 누를시 자동 공격

    public void Init()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();

        // 조작 관련
        inputActions = new();

        attackInputAction = inputActions.Player.Attack;
        guardInputAction = inputActions.Player.Guard;
        moveInputAction = inputActions.Player.Move;
        skillAction = inputActions.Player.Skill;

        Subscribe(attackInputAction, OnAttack);
        Subscribe(guardInputAction, OnGuard);
        Subscribe(moveInputAction, OnMove, true);
        if (Managers.PlayerStatus.NowWeaponType == WeaponType.scythe) Subscribe(skillAction, OnSkillScythe);
        else if (Managers.PlayerStatus.NowWeaponType == WeaponType.shield) Subscribe(skillAction, OnSkillShield);

        // 공격 관련
        enemyFilter = new();
        enemyFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
        enemyFilter.useLayerMask = true;

        isSkillAvailable = 0;
        isUnstoppable = false;
        isGuard = 0;
    }

    private void Subscribe(InputAction inputAction, Action<InputAction.CallbackContext> action, bool isOnlyOnce= false)
    {
        inputAction.started += action;
        if (isOnlyOnce) return;
        inputAction.performed += action;
        inputAction.canceled += action;
    }

    private void Describe(InputAction inputAction, Action<InputAction.CallbackContext> action, bool isOnlyOnce = false)
    {
        inputAction.started -= action;
        if (isOnlyOnce) return;
        inputAction.performed += action;
        inputAction.canceled += action;
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (attackCollider2D == null) return;
        if (isGuard != 0) return; // 가드 중에는 공격 불가

        if (context.phase == InputActionPhase.Performed)
        {
            if (holdAttackIE == null) StartCoroutine(holdAttackIE = HoldIE());
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            StopCoroutine(holdAttackIE);
            holdAttackIE = null;
        }
    }

    public IEnumerator HoldIE() // 꾹 누를시 느린 속도로 자동 공격
    {
        AttackEnemy();
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            AttackEnemy();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnSkillScythe(InputAction.CallbackContext context)
    {
        if (isSkillAvailable == 1)
        {
            isSkillAvailable = 2;

            GetComponent<SpriteRenderer>().color = Color.white;
            StartCoroutine(OnSkillScytheIE());
        }
    }

    private IEnumerator OnSkillScytheIE() // 5초간 무기 범위 증가
    {
        attackCollider2D.transform.localScale = new(2f, 0.2f, 1f);
        yield return new WaitForSeconds(5f);
        attackCollider2D.transform.localScale = new(0.4f, 0.2f, 1f);

        Managers.PlayerStatus.HitCount = 0;
        isSkillAvailable = 0;
    }

    private void OnSkillShield(InputAction.CallbackContext context)
    {
        if (isSkillAvailable == 1)
        {
            isSkillAvailable = 2;

            GetComponent<SpriteRenderer>().color = Color.white;
            isUnstoppable = true;
        }
    }

    private void AttackEnemy() // 적 공격
    {
        List<Collider2D> attackedEnemies = new();

        attackCollider2D.Overlap(enemyFilter, attackedEnemies);

        if (attackedEnemies.Count > 0)
        {
            Managers.PlayerStatus.HitCount++;
            foreach (Collider2D enemy in attackedEnemies)
            {
                if(Managers.PlayerStatus.NowWeaponType==WeaponType.sword&& Managers.PlayerStatus.HitCount >= 10)
                {
                    Managers.PlayerStatus.HitCount = 0;
                    if (enemy != null) enemy.GetComponent<Enemy1>().TakeDamage(Managers.PlayerStatus.Damage * 2f); // 검 크리티컬
                }
                else
                {
                    if (enemy != null) enemy.GetComponent<Enemy1>().TakeDamage(Managers.PlayerStatus.Damage);
                }
            }
        }
    }

    private void OnGuard(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (isGuard == 0)
            {
                StartCoroutine(guardIE = OnGuardIE());
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            if (isGuard == 1)
            {
                StopCoroutine(guardIE);
                isGuard = 0;
            }
        }
    }

    private IEnumerator OnGuardIE() // 가드 버튼을 누르는 동안 가드. 내부 쿨타임 존재
    {
        if (attackCollider2D == null) yield break;

        isGuard = 1;

        yield return null;

        List<Collider2D> attackedEnemies = new();

        while (attackedEnemies.Count == 0)
        {
            attackCollider2D.Overlap(enemyFilter, attackedEnemies);
            yield return null;
        }

        isGuard = 2;

        Enemy1 nowEnemy = attackedEnemies[0].GetComponent<Enemy1>();
        int power = nowEnemy.KnockBackPower;
        if (nowEnemy.enemyType == EnemyType.enemy3) nowEnemy.LostIron(); // enemy3타입이면 약화

        foreach (GameObject enemy in Managers.Stage.EnemyQueue)
        {
            if (enemy != null) enemy.GetComponent<Enemy1>().KnockBack(power);
        }

        Vector2 repulsionSpeed = Vector2.left * 8f;
        rigidBody2D.linearVelocity = repulsionSpeed;
        while (repulsionSpeed.x <= -2)
        {
            if (isUnstoppable) // 방패 스킬 사용시 넉백 면역
            {
                isUnstoppable = false;
                Managers.PlayerStatus.HitCount = 0;
                isSkillAvailable = 0;
                break;
            }
            repulsionSpeed += Vector2.right * 6f * Time.deltaTime;
            if (transform.position.x <= -8.5f)
            {
                rigidBody2D.linearVelocity = Vector2.zero; // 너무 왼쪽인 경우 더 뒤로가지 않음
            }
            else
            {
                rigidBody2D.linearVelocity = repulsionSpeed;
            }
            yield return null;
        }

        rigidBody2D.linearVelocity = Vector2.zero;
        yield return null;

        isGuard = 0;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (isGuard != 0) return; // 가드 중에는 이동 불가

        rigidBody2D.linearVelocity = Vector2.right * (Managers.Stage.Stage == 5 ? 2.5f : 5f); // 5스테이지(보스)에서는 이동속도 감소
    }

    public void StartStageDirect()
    {
        StartCoroutine(StartStageDirectIE());
    }

    private IEnumerator StartStageDirectIE() // 스테이지 시작 연출
    {
        transform.position = startPos + Vector2.left * 5f;

        float nowTime = 0f, maxTime = 0.5f;
        while (nowTime <= maxTime)
        {
            transform.position = Vector2.Lerp(transform.position, startPos, 0.1f);

            nowTime += Time.deltaTime;
            yield return null;
        }

        transform.position = startPos;

        yield return null;

        rigidBody2D.simulated = true;
        inputActions.Enable();
        StartCoroutine(GameOverCheck());
    }

    public void EndStageDirect()
    {
        StartCoroutine(EndStageDirectIE());
    }

    private IEnumerator EndStageDirectIE() // 스테이지 종료 연출
    {
        rigidBody2D.simulated = false;
        inputActions.Disable();

        while (transform.position.x < endPos.x)
        {
            transform.position += Vector3.right * 0.15f;
            yield return null;
        }

        Managers.Stage.StartStage(); // 다음 스테이지
    }

    public void ReadySkill()
    {
        GetComponent<SpriteRenderer>().color = Color.yellow;
        isSkillAvailable = 1;
    }

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        try { Managers.PlayerStatus.character = this; }
        catch { }
    }

    private IEnumerator GameOverCheck()
    {
        while (true)
        {
            if (transform.position.x <= -9f)
            {
                Managers.SceneFlow.GotoScene("Lobby"); // 너무 밀려난 경우게임 오버
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnDestroy()
    {
        inputActions.Disable();

        Describe(attackInputAction, OnAttack);
        Describe(guardInputAction, OnGuard);
        Describe(moveInputAction, OnMove, true);
        if (Managers.PlayerStatus.NowWeaponType == WeaponType.scythe) Describe(skillAction, OnSkillScythe);
        else if (Managers.PlayerStatus.NowWeaponType == WeaponType.shield) Describe(skillAction, OnSkillShield);
    }
}
