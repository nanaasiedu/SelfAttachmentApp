using UnityEngine;


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
        if (GazeManager.Instance == null || CursorOnHolograms == null || CursorOffHolograms == null)
        {
            return;
        }

        if (GazeManager.Instance.Hit)
        {
            CursorOnHolograms.SetActive(true);
            CursorOffHolograms.SetActive(false);
        }
        else
        {
            CursorOffHolograms.SetActive(true);
            CursorOnHolograms.SetActive(false);
        }

        gameObject.transform.position = GazeManager.Instance.Position;
        gameObject.transform.forward = GazeManager.Instance.Normal;
    }
}
