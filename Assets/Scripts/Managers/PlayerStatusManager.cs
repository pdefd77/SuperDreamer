using UnityEngine;

public enum WeaponType { sword, scythe, shield }

public class PlayerStatusManager
{
    public Character character;

    public int Gold
    {
        get
        {
            return gold;
        }
        set
        {
            gold = value;
        }
    }
    private int gold; // 인게임 재화
    public float Damage
    {
        get
        {
            return damage * (1f + weaponLevel[(int)nowWeaponType] * 0.1f); // 무기레벨에 따라 데미지 증가
        }
        set
        {
            damage = value;
        }
    }
    private float damage = 10f; // 캐릭터 데미지
    public int HitCount
    {
        get
        {
            return hitCount;
        }
        set
        {
            hitCount = value;
            if (nowWeaponType == WeaponType.scythe && character.IsSkillAvailable == 0 && hitCount >= 40)
            {
                character.ReadySkill();
            }
            else if (nowWeaponType == WeaponType.shield && character.IsSkillAvailable == 0 && hitCount >= 20)
            {
                character.ReadySkill();
            }
        }
    }
    private int hitCount; // 스킬 발동을 위한 타격 횟수 카운터
    public WeaponType NowWeaponType
    {
        get
        {
            return nowWeaponType;
        }
        set
        {
            nowWeaponType = value;
        }
    }
    private WeaponType nowWeaponType; // 현재 무기
    public int[] WeaponLevel => weaponLevel;
    private int[] weaponLevel; // 무기 레벨

    public void Init()
    {
        weaponLevel = new int[3] { 0, 0, 0 };
        nowWeaponType = WeaponType.sword;
    }
}
