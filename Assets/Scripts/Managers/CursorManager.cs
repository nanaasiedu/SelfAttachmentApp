using UnityEngine;
using HoloToolkit.Unity;


public class CursorManager : Singleton<CursorManager>
{
    [Tooltip("Drag the Cursor object to show when it hits a hologram.")]
    public GameObject CursorOnHolograms;

    [Tooltip("Drag the Cursor object to show when it does not hit a hologram.")]
    public GameObject CursorOffHolograms;

    public GameObject CursorFingerDetected;

    public GameObject CursorFingerDown;

    void Awake()
    {
        if (CursorOnHolograms == null || CursorOffHolograms == null)
        {
            return;
        }

        clearCursors();
    }

    void Update()
    {
        if (SatGazeManager.Instance == null || CursorOnHolograms == null || CursorOffHolograms == null)
        {
            return;
        }

        if (CursorFingerDetected != null && CursorFingerDetected != null && HandsManager.Instance.HandDetected)
        {
            if (HandsManager.Instance.FingerDown)
            {
                CursorOnHolograms.SetActive(false);
                CursorOffHolograms.SetActive(false);
                CursorFingerDetected.SetActive(false);
                CursorFingerDown.SetActive(true);
            }
            else
            {
                CursorOnHolograms.SetActive(false);
                CursorOffHolograms.SetActive(false);
                CursorFingerDown.SetActive(false);
                CursorFingerDetected.SetActive(true);
            }
        }
        else if (SatGazeManager.Instance.Hit)
        {
            CursorOffHolograms.SetActive(false);
            if (CursorFingerDetected != null) CursorFingerDetected.SetActive(false);
            if (CursorFingerDown != null) CursorFingerDown.SetActive(false);
            CursorOnHolograms.SetActive(true);
        }
        else
        {
            CursorOnHolograms.SetActive(false);
            if (CursorFingerDetected != null) CursorFingerDetected.SetActive(false);
            if (CursorFingerDown != null) CursorFingerDown.SetActive(false);
            CursorOffHolograms.SetActive(true);
        }

        gameObject.transform.position = SatGazeManager.Instance.Position;
        gameObject.transform.forward = SatGazeManager.Instance.Normal;
    }

    private void clearCursors() {
        CursorOnHolograms.SetActive(false);
        CursorOffHolograms.SetActive(false);
        if (CursorFingerDetected != null ) CursorFingerDetected.SetActive(false);
        if (CursorFingerDown != null)  CursorFingerDown.SetActive(false);
    }
}
