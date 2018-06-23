
using UnityEngine;


using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Unity;

public class TherapyRoomSceneManager : MonoBehaviour
{
    public GameObject[] objectsToDestroy;
    public GameObject[] objectsToKeep;

    public GameObject protocolManager;
    public GameObject child;
    public HeadsUpDirectionIndicator directionIndicator;
    public GameObject scanAlertMessage;
    public KeywordManager keywordManager;
    public Transform headTransform;

    public void Start()
    {
        keepObjects();

        SpatialMappingManager.Instance.DrawVisualMeshes = ScenesData.showMeshes;
        LocationManager.Instance.debugOn = ScenesData.debugMode;

        ScenesData.protocolManager = protocolManager;
        ScenesData.scanAlertMessage = scanAlertMessage;
        ScenesData.child = child;
        ScenesData.keywordManager = keywordManager;
        ScenesData.directionIndicator = directionIndicator;
        ScenesData.headTransform = headTransform;

        if (ScenesData.autoRoomScan) LocationManager.Instance.StartScan();
    }

    public void OpenStartPageScene()
    {
        cleanUp();
        Application.LoadLevel("StartPage");
    }

    private void cleanUp() {
        foreach (Object objectToDestroy in objectsToDestroy) {
            Destroy(objectToDestroy);
        }

        ScenesData.autoRoomScan = false;
        ScenesData.showMeshes = false;
        ScenesData.debugMode = false;

        SpatialMappingManager.Instance.DrawVisualMeshes = false;

        keywordManager.endKeywordRecognizer();
    }

    private void keepObjects() {
        foreach (Object objectToKeep in objectsToKeep)
        {
            DontDestroyOnLoad(objectToKeep);
        }
    }

}
