using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity;

public class StartSceneManager : MonoBehaviour
{
    void openTherapyRoomScene() {
        loadTherapyRoomScene(ScenesData.ProtocolType.NONE);
    }

    void startStage2()
    {
        loadTherapyRoomScene(ScenesData.ProtocolType.STAGE_2);
    }

    void startStage3() {
        loadTherapyRoomScene(ScenesData.ProtocolType.STAGE_3);
    }

    void startStage4() {
        loadTherapyRoomScene(ScenesData.ProtocolType.STAGE_4);
    }

    void openSatIntroScene() {
        loadTherapyRoomScene(ScenesData.ProtocolType.STAGE_1);
    }

    private void loadTherapyRoomScene(ScenesData.ProtocolType protocolType) {
        ScenesData.currentProtocol = protocolType;
        Application.LoadLevel("TherapyRoom");
    }

}
