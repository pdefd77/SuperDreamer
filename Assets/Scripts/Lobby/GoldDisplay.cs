using TMPro;
using UnityEngine;

public class GoldDisplay : MonoBehaviour
{
    TextMeshProUGUI goldTxt;

    private void Awake()
    {
        goldTxt = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        goldTxt.text = "°ñµå : " + Managers.PlayerStatus.Gold.ToString();
    }
}
