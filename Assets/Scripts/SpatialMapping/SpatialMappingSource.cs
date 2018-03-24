using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpatialMappingSource : MonoBehaviour

{
    public struct SurfaceObject
    {
        public int ID;
        public int UpdateID;
        public GameObject Object;
        public MeshRenderer Renderer;
        public MeshFilter Filter;
    }

    public List<SurfaceObject> SurfaceObjects { get; private set; }

    protected Type[] componentsRequiredForSurfaceMesh =
        {
            typeof(MeshFilter),
            typeof(MeshRenderer),
            typeof(MeshCollider)
        };

    protected virtual Material RenderMaterial { get { return SpatialMappingManager.Instance.SurfaceMaterial; } }

    protected virtual void Awake()
    {
        SurfaceObjects = new List<SurfaceObject>();
    }

    virtual public List<MeshRenderer> GetMeshRenderers()
    {
        List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

        for (int index = 0; index < SurfaceObjects.Count; index++)
        {
            if (SurfaceObjects[index].Renderer != null)
            {
                meshRenderers.Add(SurfaceObjects[index].Renderer);
            }
        }

        return meshRenderers;
    }

    protected GameObject AddSurfaceObject(Mesh mesh, string objectName, Transform parentObject, int meshID = 0)
    {
        SurfaceObject surfaceObject = new SurfaceObject();
        surfaceObject.ID = meshID;
        surfaceObject.UpdateID = 0;

        surfaceObject.Object = new GameObject(objectName, componentsRequiredForSurfaceMesh);
        surfaceObject.Object.transform.SetParent(parentObject);
        surfaceObject.Object.layer = SpatialMappingManager.Instance.PhysicsLayer;

        surfaceObject.Filter = surfaceObject.Object.GetComponent<MeshFilter>();
        surfaceObject.Filter.sharedMesh = mesh;

        surfaceObject.Renderer = surfaceObject.Object.GetComponent<MeshRenderer>();
        surfaceObject.Renderer.sharedMaterial = RenderMaterial;

        SurfaceObjects.Add(surfaceObject);

        return surfaceObject.Object;
    }

    protected void UpdateSurfaceObject(GameObject gameObject, int meshID)
    {
        // If it's in the list, update it
        for (int i = 0; i < SurfaceObjects.Count; ++i)
        {
            if (SurfaceObjects[i].Object == gameObject)
            {
                SurfaceObject thisSurfaceObject = SurfaceObjects[i];
                thisSurfaceObject.ID = meshID;
                thisSurfaceObject.UpdateID++;
                SurfaceObjects[i] = thisSurfaceObject;
                return;
            }
        }

        // Not in the list, add it
        SurfaceObject surfaceObject = new SurfaceObject();
        surfaceObject.ID = meshID;
        surfaceObject.UpdateID = 0;

        surfaceObject.Object = gameObject;
        surfaceObject.Filter = surfaceObject.Object.GetComponent<MeshFilter>();
        surfaceObject.Renderer = surfaceObject.Object.GetComponent<MeshRenderer>();

        SurfaceObjects.Add(surfaceObject);
    }

    protected void RemoveSurfaceObject(GameObject surface, bool removeAndDestroy = true)
    {
        // Remove it from our list
        for (int index = 0; index < SurfaceObjects.Count; index++)
        {
            if (SurfaceObjects[index].Object == surface)
            {
                if (removeAndDestroy)
                {
                    Destroy(SurfaceObjects[index].Object);
                }
                SurfaceObjects.RemoveAt(index);
                break;
            }
        }
    }

    protected void Cleanup()
    {
        for (int index = 0; index < SurfaceObjects.Count; index++)
        {
            Destroy(SurfaceObjects[index].Object);
        }
        SurfaceObjects.Clear();
    }

    virtual public List<MeshFilter> GetMeshFilters()
    {
        List<MeshFilter> meshFilters = new List<MeshFilter>();

        for (int index = 0; index < SurfaceObjects.Count; index++)
        {
            if (SurfaceObjects[index].Filter != null &&
                SurfaceObjects[index].Filter.sharedMesh != null &&
                SurfaceObjects[index].Filter.sharedMesh.vertexCount > 2)
            {
                meshFilters.Add(SurfaceObjects[index].Filter);
            }
        }

        return meshFilters;
    }


}

