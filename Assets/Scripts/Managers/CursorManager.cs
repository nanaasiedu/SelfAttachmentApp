using UnityEngine;
using HoloToolkit.Unity;


public class CursorManager : Singleton<CursorManager>
{
    [Tooltip("Drag the Cursor object to show when it hits a hologram.")]
    public GameObject CursorOnHolograms;

    [Tooltip("Drag the Cursor object to show when it does not hit a hologram.")]
    public GameObject CursorOffHolograms;

    void Awake()
    {
        if (CursorOnHolograms == null || CursorOffHolograms == null)
        {
            return;
        }

        CursorOnHolograms.SetActive(false);
        CursorOffHolograms.SetActive(false);
    }

    void Update()
    {
        if (SatGazeManager.Instance == null || CursorOnHolograms == null || CursorOffHolograms == null)
        {
            return;
        }

        if (SatGazeManager.Instance.Hit)
        {
            CursorOnHolograms.SetActive(true);
            CursorOffHolograms.SetActive(false);
        }
        else
        {
            CursorOffHolograms.SetActive(true);
            CursorOnHolograms.SetActive(false);
        }

        gameObject.transform.position = SatGazeManager.Instance.Position;
        gameObject.transform.forward = SatGazeManager.Instance.Normal;
    }
}
