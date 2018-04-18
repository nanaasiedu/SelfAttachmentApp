using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity;

public class StartSceneManager : Singleton<StartSceneManager>
{
    void openTherapyRoomScene() {
        Application.LoadLevel("TherapyRoom");
    }

}
