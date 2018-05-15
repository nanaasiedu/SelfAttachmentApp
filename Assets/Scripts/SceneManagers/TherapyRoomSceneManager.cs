using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity;

public class TherapyRoomSceneManager : MonoBehaviour
{
    public GameObject[] objectsToDestroy;

    public void OpenStartPageScene()
    {
        Application.LoadLevel("StartPage");
        cleanUp();
    }

    private void cleanUp() {
        foreach (Object objectToDestroy in objectsToDestroy) {
            Destroy(objectToDestroy);
        }
    }

}
