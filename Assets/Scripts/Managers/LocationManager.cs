using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity.SpatialMapping;

public class LocationManager : MonoBehaviour {

    public bool limitScanningByTime = true;
    private bool meshesProcessed = false;

    public bool ChildLocationSet { get; private set; }
    public GameObject protocolManager;
    public GameObject child;
    public Transform headTransform;
    public GameObject scanAlertMessage;

    void Start () {
        SurfaceMeshesToPlanes.Instance.MakePlanesComplete += SurfaceMeshesToPlanes_MakePlanesComplete;
        child.SetActive(false);
        scanAlertMessage.SetActive(true);
        ChildLocationSet = false;
    }
	
	void Update () {
        if (!meshesProcessed && limitScanningByTime)
        {
            float elapsedTime = Time.time - SpatialMappingManager.Instance.StartTime;
            if (!limitScanningByTime || (elapsedTime >= ScenesData.scanningDuration))
            {
                if (SpatialMappingManager.Instance.IsObserverRunning())
                {
                    SpatialMappingManager.Instance.StopObserver();
                }

                CreatePlanes();
                meshesProcessed = true;
            }
        }
    }

    private void SurfaceMeshesToPlanes_MakePlanesComplete(object source, System.EventArgs args)
    {
        List<GameObject> horizontalPlanes = new List<GameObject>();
        List<GameObject> verticalPlanes = new List<GameObject>();

        horizontalPlanes = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Table | PlaneTypes.Floor);
        verticalPlanes = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Wall);

        GameObject floorPlane = findFloorPlane(horizontalPlanes);

        if (floorPlane != null)
        {
            scanAlertMessage.SetActive(false);
            RemoveVertices(SurfaceMeshesToPlanes.Instance.ActivePlanes);
            placeChildOnFloor(floorPlane);
        }
        else
        {
            Debug.Log("Failed to find surfaces - restarting search");
            SpatialMappingManager.Instance.StartObserver();
            meshesProcessed = false;
        }
    }

    private GameObject findFloorPlane(List<GameObject> horizontalPlanes) {
        foreach (GameObject horizontalPlane in horizontalPlanes) {
            SurfacePlane surfacePlane = horizontalPlane.GetComponent<SurfacePlane>();
            if (surfacePlane == null) continue;
            if (surfacePlane.PlaneType == PlaneTypes.Floor) return horizontalPlane;
        }

        return null;
    }

    private void placeChildOnFloor(GameObject floorPlane) {
        SurfacePlane surfacePlane = floorPlane.GetComponent<SurfacePlane>();

        headTransform.Rotate(-headTransform.localRotation.x, 0, 0);
        Vector3 childPositionXZ = headTransform.position + ScenesData.childStartDistance * headTransform.forward;

        RaycastHit hitInfo;
        bool hit = Physics.Raycast(childPositionXZ, Vector3.down, out hitInfo);

        if (hit)
        {
            Debug.Log("FLOOR RAYCAST DETECTED check : " + (hitInfo.collider == floorPlane.GetComponent<Collider>()));
            Vector3 childFloorPosition = hitInfo.point;

            Vector3 childHeightOffset = Vector3.up * ScenesData.childHeightOffset;
            Vector3 planeThickness = (surfacePlane.PlaneThickness * surfacePlane.SurfaceNormal);
            Vector3 childPosition = childFloorPosition + planeThickness + childHeightOffset;
            child.transform.position = childPosition;
            child.SetActive(true);
        }
        else {
            Debug.Log("RAYCAST FAILED");
        }

    }

    private void CreatePlanes()
    {
        SurfaceMeshesToPlanes surfaceToPlanes = SurfaceMeshesToPlanes.Instance;
        if (surfaceToPlanes != null && surfaceToPlanes.enabled)
        {
            surfaceToPlanes.MakePlanes();
        }
    }

    private void RemoveVertices(IEnumerable<GameObject> boundingObjects)
    {
        RemoveSurfaceVertices removeVerts = RemoveSurfaceVertices.Instance;
        if (removeVerts != null && removeVerts.enabled)
        {
            removeVerts.RemoveSurfaceVerticesWithinBounds(boundingObjects);
        }
    }

    private void OnDestroy()
    {
        if (SurfaceMeshesToPlanes.Instance != null)
        {
            SurfaceMeshesToPlanes.Instance.MakePlanesComplete -= SurfaceMeshesToPlanes_MakePlanesComplete;
        }
    }
}
