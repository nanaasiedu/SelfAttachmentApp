using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Unity;
using UnityEngine.UI;

public class LocationManager : Singleton<LocationManager>
{

    public bool limitScanningByTime = true;
    private bool meshesProcessed = false;

    public bool ChildLocationSet { get; private set; }
    public GameObject protocolManager;
    public GameObject child;
    public Transform headTransform;
    public GameObject scanAlertMessage;
    public Vector3 ChildLocation { get; private set; }

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
            bool success = placeChildOnFloor(floorPlane);

            if (!success)
            {
                setAlertText("Currently scanning room: Please make sure you have a clear open space around you");
                restartScan();
            }
            else {
                child.SetActive(true);
                protocolManager.SendMessage("StartProtocol");
            }
        }
        else
        {
            Debug.Log("Failed to find surfaces - restarting search");
            restartScan();
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

    private bool placeChildOnFloor(GameObject floorPlane) {
        SurfacePlane surfacePlane = floorPlane.GetComponent<SurfacePlane>();
        float floorYPosition = floorPlane.transform.position.y;
        headTransform.Rotate(-headTransform.localRotation.x, 0, 0);

        for (float bearing = 0.0f; bearing < 360.0f; bearing += ScenesData.childStartPositionCheckAngle) {
            if (bearing != 0.0f) headTransform.Rotate(0, ScenesData.childStartPositionCheckAngle, 0);
            Vector3 childPositionXZ = headTransform.position + ScenesData.childStartDistance * headTransform.forward;

            RaycastHit hitInfo;
            bool hit = Physics.Raycast(childPositionXZ, Vector3.down, out hitInfo);
            if (!(hit && childPositionRayHitTest(hitInfo.point, floorYPosition))) continue;

            setChildPositionAtPoint(hitInfo.point, surfacePlane);
            Debug.Log("SUCCESS -- CHILD POSITIONED at bearing: " + bearing);
            return true;
        }

        Debug.Log("FLOOR POSITIONING FAILED");
        return false;

    }

    private bool childPositionRayHitTest(Vector3 hitPosition, float floorPositionY) {
        return hitPosition.y - floorPositionY < ScenesData.floorHitAllowance;
    }

    private void setChildPositionAtPoint(Vector3 childFloorPosition, SurfacePlane surfacePlane) {
        Vector3 childHeightOffset = Vector3.up * ScenesData.childHeightOffset;
        Vector3 planeThickness = (surfacePlane.PlaneThickness * surfacePlane.SurfaceNormal);
        ChildLocation = childFloorPosition + planeThickness + childHeightOffset;
        ChildLocationSet = true;
        child.GetComponent<AreaManager>().ResetPosition();
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

    private void setAlertText(string text) {
        Text textComponent = scanAlertMessage.GetComponentInChildren<Text>();
        textComponent.text = text;
    }

    private void restartScan() {
        SpatialMappingManager.Instance.StartObserver();
        meshesProcessed = false;
    }
}
