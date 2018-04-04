using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

/// <summary>
/// GazeManager determines the location of the user's gaze, hit position and normals.
/// </summary>
public class SatGazeManager : Singleton<SatGazeManager>
{
    [Tooltip("Maximum gaze distance for calculating a hit.")]
    public float MaxGazeDistance = 5.0f;

    [Tooltip("Select the layers raycast should target.")]
    public LayerMask RaycastLayerMask = Physics.DefaultRaycastLayers;

    /// <summary>
    /// Physics.Raycast result is true if it hits a Hologram.
    /// </summary>
    public bool Hit { get; private set; }

    /// <summary>
    /// HitInfo property gives access
    /// to RaycastHit public members.
    /// </summary>
    public RaycastHit HitInfo { get; private set; }

    /// <summary>
    /// Position of the user's gaze.
    /// </summary>
    public Vector3 Position { get; private set; }

    /// <summary>
    /// RaycastHit Normal direction.
    /// </summary>
    public Vector3 Normal { get; private set; }

    private GazeStabilizer gazeStabilizer;
    private Vector3 gazeOrigin;
    private Vector3 gazeDirection;

    void Awake()
    {
        gazeStabilizer = GetComponent<GazeStabilizer>();
    }

    private void Update()
    {
        gazeOrigin = Camera.main.transform.position;
        gazeDirection = Camera.main.transform.forward;

        gazeStabilizer.UpdateStability(gazeOrigin, Camera.main.transform.rotation);
        gazeOrigin = gazeStabilizer.StablePosition;

        UpdateRaycast();
    }

    /// <summary>
    /// Calculates the Raycast hit position and normal.
    /// </summary>
    private void UpdateRaycast()
    {
        RaycastHit hitInfo;

        Hit = Physics.Raycast(gazeOrigin,
                       gazeDirection,
                       out hitInfo,
                       MaxGazeDistance,
                       RaycastLayerMask);

        HitInfo = hitInfo;

        if (Hit)
        {
            Position = hitInfo.point;
            Normal = hitInfo.normal;
        }
        else
        {
            Position = gazeOrigin + (gazeDirection * MaxGazeDistance);
            Normal = gazeDirection;
        }

    }
}

