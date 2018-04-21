using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity;

public class StartSceneManager : MonoBehaviour
{
    void openTherapyRoomScene() {
        ScenesData.currentProtocol = ScenesData.ProtocolType.NONE;
        Application.LoadLevel("TherapyRoom");
    }

    void openSatIntroScene() {
        Application.LoadLevel("SatIntro");
    }

}
