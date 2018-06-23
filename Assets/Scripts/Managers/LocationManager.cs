using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Unity;
using UnityEngine.UI;

public class LocationManager : Singleton<LocationManager>
{
    public Material floorFailMaterial;
    public Material wallFailMaterial;
    public Material bombFailMaterial;
    public bool debugOn;

    public bool limitScanningByTime = true;
    private bool meshesProcessed = false;

    public bool ChildLocationSet { get; private set; }

    public Vector3 ChildLocation { get; private set; }
    public Vector3 CarpetLocation { get { return ChildLocation + Vector3.up * ScenesData.carpetHeightOffset; } }

    private Stack<Vector3> objectFloorPositions;
    System.Random rnd = new System.Random();
    public Vector3 generateFloorPosition { get {
            return ChildLocation + ((rnd.Next(0, 2) == 0 ? 1 : -1) * rnd.Next(3, 41) * 0.1f) * Vector3.left + ((rnd.Next(0, 2) == 0 ? 1 : -1) * rnd.Next(3, 41) * 0.1f) * Vector3.forward;
        } }

    public Vector3 ChandelierPosition {
        get {
            RaycastHit hitInfo;
            bool hit = Physics.Raycast(ChildLocation + Vector3.up, Vector3.up, out hitInfo);
            if (hit) Debug.Log("Placing Chandelier on ceiling");
            if (!hit) Debug.Log("Celieng not found");
            float ceilingHeight = (hit ? hitInfo.point.y - ChildLocation.y : ScenesData.defaultCeilingHeight);

            return ChildLocation + (ceilingHeight - ScenesData.chandelierHeight) * Vector3.up;
        }
    }

    public Vector3 PlantPotPosition {
        get {
            Vector3 currDir = Vector3.forward;
            Vector3 headPosition = ScenesData.headTransform.position;

            for (float bearing = 0.0f; bearing < 360.0f; bearing += ScenesData.childStartPositionCheckAngle)
            {
                if (bearing != 0.0f) currDir = Quaternion.Euler(0, ScenesData.childStartPositionCheckAngle, 0) * currDir;

                for (float distance = ScenesData.potStartDistance; distance < ScenesData.potEndDistance; distance += ScenesData.potTestSeperation) {
                    RaycastHit hitInfo;
                    bool hit = Physics.Raycast(headPosition + currDir * distance, Vector3.down, out hitInfo);

                    if (hit && hitInfo.point.y > floorY + ScenesData.minTableY) return hitInfo.point + Vector3.up * ScenesData.potTableOffset;
                }

            }

            return defaultPlantPotPosition;
        }
    }

    private Vector3 defaultPlantPotPosition {
        get
        {
            return ChildLocation + Vector3.right;
        }
    }

    private float startWallScanBearing = 0f;

    private bool secondPortrait = true;

    public Vector3 wallPortraitPosition {
        get
        {
            secondPortrait = !secondPortrait;
            Vector3 currDir = Vector3.forward;
            Vector3 headPosition = ScenesData.headTransform.position;

                for (float bearing = startWallScanBearing; bearing < 360.0f; bearing += ScenesData.childStartPositionCheckAngle)
                {
                    if (bearing != 0.0f) currDir = Quaternion.Euler(0, ScenesData.childStartPositionCheckAngle, 0) * currDir;

                    RaycastHit hitInfo;
                    bool hit = Physics.Raycast(headPosition, currDir, out hitInfo);

                    if (hit)
                    {
                        Debug.Log("Portrait found wall");
                        wallPortraitNormal = -hitInfo.normal;

                        startWallScanBearing = bearing + 40f;
                        return hitInfo.point;
                    }

                }

            

            Debug.Log("No wall detected");
            wallPortraitNormal = (!secondPortrait ? defaultPortraitNormal : defaultPortraitNormal2);
            return (!secondPortrait ? defaultPortraitWallPosition : defaultPortraitWallPosition2); ;
        }
    }

    public Vector3 defaultPortraitWallPosition {
        get {
            ScenesData.headTransform.rotation = new Quaternion(0, ScenesData.headTransform.rotation.y, 0, 1);
            Vector3 headForward = ScenesData.headTransform.forward;

            headForward = Quaternion.Euler(0, 35, 0) * headForward;

            return ChildLocation + 3f * headForward;
        }
    }

    public Vector3 defaultPortraitWallPosition2
    {
        get
        {
            ScenesData.headTransform.rotation = new Quaternion(0, ScenesData.headTransform.rotation.y, 0, 1);
            Vector3 headForward = ScenesData.headTransform.forward;

            headForward = Quaternion.Euler(0, -35, 0) * headForward;

            return ChildLocation + 3f * headForward;
        }
    }

    public Vector3 wallPortraitNormal;

    private Vector3 defaultPortraitNormal {
        get {
            return (Quaternion.Euler(0, 35, 0) * ScenesData.headTransform.forward);
        }
    }

    private Vector3 defaultPortraitNormal2
    {
        get
        {
            return (Quaternion.Euler(0, -35, 0) * ScenesData.headTransform.forward);
        }
    }

    private float floorY = 0;

    void Start () {
        StartScan();
    }

    public void StartScan()
    {
        if (!meshesProcessed) SurfaceMeshesToPlanes.Instance.MakePlanesComplete += SurfaceMeshesToPlanes_MakePlanesComplete;
        ScenesData.child.SetActive(false);
        ScenesData.scanAlertMessage.SetActive(true);
        ChildLocationSet = false;
        ScenesData.directionIndicator.HideIndicator();

        meshesProcessed = false;

        SpatialMappingManager.Instance.StartObserver();
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
            setAlertText("Trying to place child...");
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
            setAlertText("Currently scanning room: Be sure to scan the floor!");
            restartScan();
        }
    }

    private void activateProtocol() {
        ScenesData.scanAlertMessage.SetActive(false);
        ScenesData.directionIndicator.ShowIndicator();
        ScenesData.child.SetActive(true);
        ScenesData.keywordManager.startKeywordRecognizer();
        ScenesData.protocolManager.SendMessage("StartProtocol");
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
        floorY = floorYPosition;

        Vector3 headPosition = ScenesData.headTransform.position;
        //Vector3 headForward = Quaternion.Euler(-headTransform.rotation.x, 0, -headTransform.rotation.z) * headTransform.forward;
        ScenesData.headTransform.rotation = new Quaternion(0, ScenesData.headTransform.rotation.y, 0, 1);
        Vector3 headForward = ScenesData.headTransform.forward;

        for (float bearing = 0.0f; bearing < 360.0f; bearing += ScenesData.childStartPositionCheckAngle) {
            Debug.Log("Checking Bearing of: " + bearing);
            if (bearing != 0.0f) headForward = Quaternion.Euler(0, ScenesData.childStartPositionCheckAngle, 0) * headForward;

            Vector3 childPositionXZ = headPosition + ScenesData.childStartDistance * headForward;

            RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, headForward, out hitInfo, ScenesData.childStartDistance + ScenesData.childMinDistanceToWall))
            {
                if (debugOn) debug_sphere(hitInfo.point, wallFailMaterial); reveal_collider(hitInfo.collider); ;
                Debug.Log("Raycast hits wall/object");
                debug_DrawLine(headPosition, headForward, ScenesData.childStartDistance + ScenesData.childMinDistanceToWall, false);
                continue;
            }
            debug_DrawLine(headPosition, headForward, ScenesData.childStartDistance + ScenesData.childMinDistanceToWall, true);

            bool hit = Physics.Raycast(childPositionXZ, Vector3.down, out hitInfo);

            Vector3 directionVector = hitInfo.normal;
            Debug.DrawRay(hitInfo.point, directionVector, Color.blue, 100, true);
            if (!(hit && childPositionRayHitTest(hitInfo.point, floorYPosition))) {
                if (debugOn) reveal_collider(hitInfo.collider);
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
        if (debugOn) debug_sphere(hitPosition, floorFailMaterial);
        bool isCloseToFloor = hitPosition.y - floorPositionY < ScenesData.floorHitAllowance;
        if (!isCloseToFloor) { Debug.Log("HIT too far from floor / floor : " + floorPositionY + " hit: " + hitPosition.y ); return false; }

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
        Vector3 scanPosition = floorPosition + Vector3.up * (ScenesData.offGroundBombStartHeight + ScenesData.childHeightOffset);

        for (float bearing = 0.0f; bearing < 360.0f; bearing += ScenesData.bombScanCheckAngle)
        {
            if (bearing != 0.0f) currDir = Quaternion.Euler(0, ScenesData.bombScanCheckAngle, 0) * currDir;

            for (int scanIteration = 0; scanIteration < ScenesData.numberOfBombScans; scanIteration++) {
                float currDistance = ScenesData.bombScanStartDistance + scanIteration * ScenesData.bombScanSeperation;
                Vector3 currPosition = scanPosition + currDir * currDistance;
                float scanDistance = ScenesData.childHeight - ScenesData.offGroundBombStartHeight;

                Vector3 directionVector = Vector3.up * scanDistance;
                Debug.DrawRay(currPosition, directionVector, Color.yellow, 100, true);

                RaycastHit hitInfo;
                if (Physics.Raycast(currPosition, Vector3.up, out hitInfo, scanDistance)) {
                    if (debugOn) debug_sphere(hitInfo.point, bombFailMaterial); reveal_collider(hitInfo.collider); ;
                    Debug.Log("BOMB SCAN FAILED at bearing : " + bearing);
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
        ScenesData.child.GetComponent<AreaManager>().ResetPosition();
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
        Text textComponent = ScenesData.scanAlertMessage.GetComponentInChildren<Text>();
        textComponent.text = text;
    }

    private void restartScan() {
        SpatialMappingManager.Instance.StartObserver();
        meshesProcessed = false;
    }

    private void debug_sphere(Vector3 location, Material material)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = location;
        sphere.GetComponent<Renderer>().material = material;
        sphere.transform.localScale = 0.03f * new Vector3(1, 1, 1);
        Destroy(sphere.GetComponent<Collider>());
    }

    private void reveal_collider(Collider collider) {
        //collider.gameObject.GetComponent<Renderer>().material = bombFailMaterial;
    }
}
