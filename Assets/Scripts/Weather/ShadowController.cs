using UnityEngine;
using System.Collections;

public class ShadowController : MonoBehaviour {

    // Variables to control rendering of shadows
    const int globalSortingOrder = 0;
    int globalOccSortingOrder { get { return globalSortingOrder - 1; } }

    int instanceSortingOrder { get { return globalOccSortingOrder - 1; } }

    int instanceOccSortingOrder { get { return instanceSortingOrder - 1; } }

    const int globalZ = -5;
    int instanceZ { get { return globalZ + 1; } }

    public WeatherManager.WeatherType weather;

    Rect mapSize;

    MeshFilter occlusionMeshFilter;
    Renderer occlusionRenderer;

    Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();

        var tiledMap = transform.root.GetComponent<Tiled2Unity.TiledMap>();
        if (tiledMap == null)
            return;
        mapSize = new Rect(0, 0, tiledMap.GetMapWidthInPixelsScaled(), -tiledMap.GetMapWidthInPixelsScaled());

        GameObject occluder = GameObject.CreatePrimitive(PrimitiveType.Quad);
        occluder.name = "InstanceWeatherOcclusion";
        occluder.transform.SetParent(this.transform, false);

        occlusionMeshFilter = occluder.GetComponent<MeshFilter>();
        occlusionRenderer = occluder.GetComponent<MeshRenderer>();

        occlusionRenderer.material = new Material(Shader.Find("Transparent/Shadow"));
        occlusionRenderer.material.SetColor("_Color", new Color(0, 0, 0, 0));
        occlusionRenderer.sortingLayerName = renderer.sortingLayerName;
        occlusionRenderer.sortingOrder = renderer.sortingOrder - 1;

        occluder.SetActive(false);

        StartCoroutine(WaitForInstanceCast());
    }

    void Update()
    {
        //if (renderer.enabled)
        //{
        //    if (WeatherManager.Instance.type != weather &&
        //       !WeatherManager.Instance.instanceIsRunning) // && WeatherManager.Instance.instanceTypeNum != (int)weather))
        //    {
        //        renderer.enabled = false;
        //    }
        //}
        //else
        //{
        //    if (WeatherManager.Instance.type == weather)
        //        renderer.enabled = true;
        //}
    }

    IEnumerator WaitForInstanceCast()
    {
        while (!WeatherManager.Instance.instanceIsRunning)
        {
            if (WeatherManager.Instance.type == weather && !renderer.enabled)
                renderer.enabled = true;
            else if (WeatherManager.Instance.type != weather && renderer.enabled)
                renderer.enabled = false;

            yield return null;
        }

        // Instance weather has been cast
        if (WeatherManager.Instance.type == weather)
        {
            SetOccluder(true, false);
        }
        if (WeatherManager.Instance.instanceTypeNum == (int)weather)
        {
            SetOccluder(true, true);
        }

        StartCoroutine(WaitForInstanceEnd());
    }

    IEnumerator WaitForInstanceEnd()
    {
        var iwt = WeatherManager.Instance.instanceTypeNum;

        while (WeatherManager.Instance.instanceIsRunning)
        {
            yield return null;
        }

        SetOccluder(false);

        StartCoroutine(WaitForInstanceCast());
    }

    void SetOccluder(bool enable, bool instanceWeather = false)
    {
        // Disable
        if (!enable)
        {
            occlusionMeshFilter.gameObject.SetActive(false);
        }
        // Enable
        else
        {
            renderer.enabled = true;
            Rect instanceSize = WeatherManager.Instance.instanceRectArea;

            Mesh mesh = occlusionMeshFilter.sharedMesh = new Mesh();

            if (!instanceWeather)
            {
                renderer.transform.localPosition = new Vector3(0, 0, globalZ);

                renderer.sortingOrder = globalSortingOrder;
                occlusionRenderer.sortingOrder = globalOccSortingOrder;

                int[] indicies = {
                    0, 1, 2,
                    1, 3, 2
                };

                Vector3[] v = {
                    new Vector2(instanceSize.xMin, mapSize.yMin),   // Top Left
                    new Vector2(instanceSize.xMax, mapSize.yMin),   // Top Right
                    new Vector2(instanceSize.xMin, mapSize.yMax),   // Bot Left
                    new Vector2(instanceSize.xMax, mapSize.yMax)    // Bot Right
                };

                mesh.vertices = v;
                mesh.triangles = indicies;

                mesh.RecalculateBounds();
                mesh.RecalculateNormals();

                occlusionMeshFilter.sharedMesh = mesh;
            }
            else
            {
                // Instance
                renderer.transform.localPosition = new Vector3(0, 0, instanceZ);

                renderer.sortingOrder = instanceSortingOrder;
                occlusionRenderer.sortingOrder = instanceOccSortingOrder;

                Mesh m1 = new Mesh();
                Mesh m2 = new Mesh();

                int[] indicies = {
                    0, 1, 2,
                    1, 3, 2
                };

                Vector3[] v1 = {
                    new Vector2(mapSize.xMin, mapSize.yMin),        // Top Left
                    new Vector2(instanceSize.xMin, mapSize.yMin),   // Top Right
                    new Vector2(mapSize.xMin, mapSize.yMax),        // Bot Left
                    new Vector2(instanceSize.xMin, mapSize.yMax)    // Bot Right
                };

                Vector3[] v2 = {
                    new Vector2(instanceSize.xMax, mapSize.yMin),   // Top Left
                    new Vector2(mapSize.xMax, mapSize.yMin),        // Top Right
                    new Vector2(instanceSize.xMax, mapSize.yMax),   // Bot Left
                    new Vector2(mapSize.xMax, mapSize.yMax)         // Bot Right
                };

                m1.vertices = v1;
                m1.triangles = indicies;

                m2.vertices = v2;
                m2.triangles = indicies;

                CombineInstance[] ci = new CombineInstance[2];

                ci[0] = new CombineInstance();
                ci[0].mesh = m1;
                ci[1] = new CombineInstance();
                ci[1].mesh = m2;

                mesh.CombineMeshes(ci, true, false);

                mesh.RecalculateBounds();
                mesh.RecalculateNormals();

                occlusionMeshFilter.sharedMesh = mesh;
            }

            occlusionMeshFilter.gameObject.SetActive(true);
        }
    }
}
