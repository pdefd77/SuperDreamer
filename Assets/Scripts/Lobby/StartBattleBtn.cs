using UnityEngine;
using UnityEngine.UI;

public class StartFieldBtn : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(GotoField);
    }

    private void GotoField()
    {
        Managers.SceneFlow.GotoScene("Field");
    }
}
