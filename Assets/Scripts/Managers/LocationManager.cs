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
    public HeadsUpDirectionIndicator directionIndicator;
    public KeywordManager keywordManager;

    void Start () {
        SurfaceMeshesToPlanes.Instance.MakePlanesComplete += SurfaceMeshesToPlanes_MakePlanesComplete;
        child.SetActive(false);
        scanAlertMessage.SetActive(true);
        ChildLocationSet = false;
        directionIndicator.HideIndicator();
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

        horizontalPlanes = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Floor);
        Debug.Log("number of floors: " + horizontalPlanes.Count);
        verticalPlanes = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Wall);

        GameObject floorPlane = findFloorPlane(horizontalPlanes);

        if (floorPlane != null)
        {
            //RemoveVertices(SurfaceMeshesToPlanes.Instance.ActivePlanes);
            bool success = placeChildOnFloor(floorPlane);

            if (!success)
            {
                setAlertText("Currently scanning room: Please make sure you have a clear open space around you");
                restartScan();
            }
            else {
                activateProtocol();
            }
        }
        else
        {
            Debug.Log("Failed to find surfaces - restarting search");
            restartScan();
        }
    }

    private void activateProtocol() {
        scanAlertMessage.SetActive(false);
        directionIndicator.ShowIndicator();
        child.SetActive(true);
        keywordManager.startKeywordRecognizer();
        protocolManager.SendMessage("StartProtocol");
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

        Vector3 headPosition = headTransform.position;
        //Vector3 headForward = Quaternion.Euler(-headTransform.rotation.x, 0, -headTransform.rotation.z) * headTransform.forward;
        int x = 0;
        headTransform.rotation = new Quaternion(0, headTransform.rotation.y, 0, 1);
        Vector3 headForward = headTransform.forward;

        for (float bearing = 0.0f; bearing < 360.0f; bearing += ScenesData.childStartPositionCheckAngle) {
            Debug.Log("Checking Bearing of: " + bearing);
            if (bearing != 0.0f) headForward = Quaternion.Euler(0, ScenesData.childStartPositionCheckAngle, 0) * headForward;

            Vector3 childPositionXZ = headPosition + ScenesData.childStartDistance * headForward;

            
            if (Physics.Raycast(headPosition, headForward, ScenesData.childStartDistance + ScenesData.childMinDistanceToWall))
            {
                Debug.Log("Raycast hits wall/object");
                debug_DrawLine(headPosition, headForward, ScenesData.childStartDistance + ScenesData.childMinDistanceToWall, false);
                continue;
            }
            debug_DrawLine(headPosition, headForward, ScenesData.childStartDistance + ScenesData.childMinDistanceToWall, true);

            RaycastHit hitInfo;
            bool hit = Physics.Raycast(childPositionXZ, Vector3.down, out hitInfo);

            Vector3 directionVector = hitInfo.normal * 1;
            Debug.DrawRay(hitInfo.point, directionVector, Color.blue, 100, true);
            if (!(hit && childPositionRayHitTest(hitInfo.point, floorYPosition))) {
                debug_DrawLine(childPositionXZ, Vector3.down, headPosition.y - hitInfo.point.y, false);
                continue;
            }
            debug_DrawLine(childPositionXZ, Vector3.down, headPosition.y - hitInfo.point.y, true);

            setChildPositionAtPoint(hitInfo.point, surfacePlane);
            Debug.Log("SUCCESS -- CHILD POSITIONED at bearing: " + bearing);
            return true;
        }

        Debug.Log("FLOOR POSITIONING FAILED");
        return false;

    }

    private void debug_DrawLine(Vector3 position, Vector3 direction, float distance, bool success) {
        Vector3 directionVector = direction * distance;
        Debug.DrawRay(position, directionVector, success ? Color.green : Color.red, 100, true);
    }

    private bool childPositionRayHitTest(Vector3 hitPosition, float floorPositionY) {
        bool isCloseToFloor = hitPosition.y - floorPositionY < ScenesData.floorHitAllowance;
        if (!isCloseToFloor) { Debug.Log("HIT too far from floor"); return false; }

        hitPosition.y += ScenesData.childMidHeight;
        gameObject.transform.position = hitPosition;

        return rayCastBomb(hitPosition);
    }

    private bool colliderHitsMeshes(List<Mesh> meshes) {
        Collider collider = GetComponent<Collider>();
        
        foreach (Mesh mesh in meshes) {
            if (collider.bounds.Intersects(mesh.bounds) && BoundsIsEncapsulated(collider.bounds, mesh.bounds))
            { Debug.Log("MESH COLLISION DETECTED"); return true; }
        }

        Debug.Log("PASSED MESH TEST");
        return false;
    }

    private bool rayCastBomb(Vector3 floorPosition) {
        Vector3 currDir = Vector3.forward;
        Vector3 scanPosition = floorPosition + Vector3.up * ScenesData.offGroundBombStartHeight;

        for (float bearing = 0.0f; bearing < 360.0f; bearing += ScenesData.bombScanCheckAngle)
        {
            if (bearing != 0.0f) currDir = Quaternion.Euler(0, ScenesData.bombScanCheckAngle, 0) * currDir;

            for (int scanIteration = 0; scanIteration < ScenesData.numberOfBombScans; scanIteration++) {
                float currDistance = ScenesData.bombScanStartDistance + scanIteration * ScenesData.bombScanSeperation;
                Vector3 currPosition = scanPosition + currDir * currDistance;
                float scanDistance = ScenesData.childHeight - ScenesData.offGroundBombStartHeight;

                Vector3 directionVector = Vector3.up * scanDistance;
                Debug.DrawRay(currPosition, directionVector, Color.yellow, 100, true);

                if (Physics.Raycast(currPosition, Vector3.up, scanDistance)) {
                    Debug.Log("BOMB SCAN FAILED");
                    return false;
                }
            }
        }

        return true;
    }

    bool BoundsIsEncapsulated(Bounds Encapsulator, Bounds Encapsulating)
    {
        return Encapsulator.Contains(Encapsulating.min) && Encapsulator.Contains(Encapsulating.max);
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
