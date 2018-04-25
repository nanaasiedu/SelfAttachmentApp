using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity;

public class StartSceneManager : MonoBehaviour
{
    void openTherapyRoomScene() {
        loadTherapyRoomScene(ScenesData.ProtocolType.NONE);
    }

    void performNeckMassageProtocol()
    {
        loadTherapyRoomScene(ScenesData.ProtocolType.MASSAGE);
    }

    void openSatIntroScene() {
        Application.LoadLevel("SatIntro");
    }

    private void loadTherapyRoomScene(ScenesData.ProtocolType protocolType) {
        ScenesData.currentProtocol = protocolType;
        Application.LoadLevel("TherapyRoom");
    }

}
