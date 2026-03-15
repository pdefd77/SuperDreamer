using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartSummonBtn : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldTxt;
    [SerializeField] private TextMeshProUGUI[] weaponLevelTxt;

    private void Start()
    {
        goldTxt.text = "°ñµå : " + Managers.PlayerStatus.Gold.ToString();
        for (int i = 0; i < weaponLevelTxt.Length; i++) weaponLevelTxt[i].text = "+" + Managers.PlayerStatus.WeaponLevel[i].ToString();
        GetComponent<Button>().onClick.AddListener(Summon);
    }

    private void Summon()
    {
        if (Managers.PlayerStatus.Gold >= 10)
        {
            Managers.PlayerStatus.Gold -= 10;
            goldTxt.text = "°ñµå : " + Managers.PlayerStatus.Gold.ToString();

            int summonWeapon = Random.Range(0, Managers.PlayerStatus.WeaponLevel.Length);
            Managers.PlayerStatus.WeaponLevel[summonWeapon]++;
            weaponLevelTxt[summonWeapon].text = "+" + Managers.PlayerStatus.WeaponLevel[summonWeapon].ToString();
        }
    }
}
