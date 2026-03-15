using UnityEngine;

public class Managers : MonoBehaviour
{
    // Singleton
    public static Managers Instance => instance;
    private static Managers instance;
    #region Managers
    public static PlayerStatusManager PlayerStatus => instance.playerStatus;
    private PlayerStatusManager playerStatus = new(); // 플레이어 스탯 관리
    public static StageManager Stage => instance.stage;
    private StageManager stage = new(); // 전투 관리
    public static AssetManager Asset => instance.asset;
    private AssetManager asset = new(); // 에셋 관리
    public static SceneFlowManager SceneFlow => instance.sceneFlow;
    private SceneFlowManager sceneFlow = new(); // 에셋 관리
    #endregion

    private bool isOnly = false; // 첫 오브젝트인지 체크 (OnDestroy 에 사용하는 에디터용 코드)

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        isOnly = true;
        instance = this;
        DontDestroyOnLoad(gameObject);

        SetInit();
    }

    private void SetInit()
    {
        //Application.targetFrameRate = 60;

        asset.Init();
        playerStatus.Init();
    }

    private void OnDestroy() // 초기화
    {
        if (!isOnly) return;
    }
}
