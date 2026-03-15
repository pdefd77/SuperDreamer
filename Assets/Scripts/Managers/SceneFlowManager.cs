using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowManager
{
    public void GotoScene(string sceneName) // string으로 씬 전환하기
    {
        SceneManager.LoadScene(sceneName);
    }
}
