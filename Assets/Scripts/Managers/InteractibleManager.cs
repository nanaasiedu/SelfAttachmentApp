using HoloToolkit.Unity;
using UnityEngine;

/// <summary>
/// InteractibleManager keeps tracks of which GameObject
/// is currently in focus.
/// </summary>
public class InteractibleManager : Singleton<InteractibleManager>
{
    public GameObject FocusedGameObject { get; private set; }

    private GameObject oldFocusedGameObject = null;

    void Start()
    {
        FocusedGameObject = null;
    }

    void Update()
    {

        oldFocusedGameObject = FocusedGameObject;

        if (SatGazeManager.Instance.Hit) {
            RaycastHit hitInfo = SatGazeManager.Instance.HitInfo;
            FocusedGameObject = hitInfo.collider != null ? hitInfo.collider.gameObject : null;
        } else {
            FocusedGameObject = null;
        }

        if (FocusedGameObject != oldFocusedGameObject)
        {
            ResetFocusedInteractible();

            if (FocusedGameObject != null)
            {
                if (FocusedGameObject.GetComponent<Interactible>() != null)
                {
                    // 2.c: Send a GazeEntered message to the FocusedGameObject.
                    FocusedGameObject.SendMessage("GazeEntered");
                }
            }
        }
    }

    private void ResetFocusedInteractible()
    {
        if (oldFocusedGameObject != null)
        {
            if (oldFocusedGameObject.GetComponent<Interactible>() != null)
            {
                // 2.c: Send a GazeExited message to the oldFocusedGameObject.
                oldFocusedGameObject.SendMessage("GazeExited");
            }
        }
    }
}