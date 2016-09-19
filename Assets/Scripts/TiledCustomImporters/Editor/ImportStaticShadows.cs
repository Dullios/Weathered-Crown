using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tiled2Unity;

[CustomTiledImporter]
public class ImportStaticShadows : ICustomTiledImporter {

    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> customProperties)
    {
        if (customProperties.ContainsKey("shadow"))
        {
            gameObject.AddComponent<ShadowDesignation>();
        }
    }

    public void CustomizePrefab(GameObject prefab)
    {
        // Destroy all old shadows
        foreach (var oldShadow in prefab.GetComponentsInChildren<ShadowController>())
        {
            GameObject.DestroyImmediate(oldShadow.gameObject);
        }

        // Generate new shadows
        var shadowDesignations = prefab.GetComponentsInChildren<ShadowDesignation>();
        foreach (var genShadow in shadowDesignations)
        {
            //var meshFilters = genShadow.transform.GetChild(0).GetComponentsInChildren<MeshFilter>();

            //if (mf.GetComponent<Collider>() || mf.sharedMesh == null || mf.transform == mf.transform.root)
            //    continue;

            GenerateSunShadows(genShadow.gameObject);
            GenerateOtherShadows(genShadow.gameObject);

            GameObject.DestroyImmediate(genShadow);
        }
        
    }

    void GenerateOtherShadows(GameObject obj)
    {
        // Get Map Size
        var tiledMap = obj.transform.root.GetComponent<Tiled2Unity.TiledMap>();
        if (tiledMap == null)
            return;
        Rect mapSize = new Rect(0, 0, tiledMap.GetMapWidthInPixelsScaled(), -tiledMap.GetMapWidthInPixelsScaled());

        // Create Shadow
        Mesh shadow = new Mesh();

        int[] indicies = {
                    0, 1, 2,
                    1, 3, 2
                };

        Vector3[] v = {
                    new Vector2(mapSize.xMin, mapSize.yMin),   // Top Left
                    new Vector2(mapSize.xMax, mapSize.yMin),   // Top Right
                    new Vector2(mapSize.xMin, mapSize.yMax),   // Bot Left
                    new Vector2(mapSize.xMax, mapSize.yMax)    // Bot Right
                };

        shadow.vertices = v;
        shadow.triangles = indicies;

        shadow.RecalculateNormals();
        shadow.RecalculateBounds();

        // Save Mesh
        if (!AssetDatabase.IsValidFolder("Assets/Tiled2Unity/Meshes/Shadows"))
            AssetDatabase.CreateFolder("Assets/Tiled2Unity/Meshes", "Shadows");

        string filePath = "Assets/Tiled2Unity/Meshes/Shadows/" + obj.transform.root.name + "_" + obj.gameObject.name + "_other_shadows" + ".asset";
        if (AssetDatabase.LoadAssetAtPath(filePath, typeof(System.Object)) != null)
            AssetDatabase.DeleteAsset(filePath);
        AssetDatabase.CreateAsset(shadow, filePath);

        // Create GameObject to render the shadows
        GameObject snowShadows = new GameObject("SnowShadows");

        // Add Shadow Controller Script
        var shadowController = snowShadows.AddComponent<ShadowController>();
        shadowController.weather = WeatherManager.WeatherType.snow;

        // Inherit values from the original object
        Transform child = obj.transform.GetChild(0);
        snowShadows.transform.SetParent(obj.gameObject.transform, true);
        snowShadows.transform.position = child.transform.position;
        snowShadows.transform.rotation = child.transform.rotation;
        snowShadows.transform.localScale = child.transform.localScale;

        // Create and configure components
        var filter = snowShadows.AddComponent<MeshFilter>();
        filter.sharedMesh = shadow;

        var renderer = snowShadows.AddComponent<MeshRenderer>();
        Material mat = AssetDatabase.LoadAssetAtPath("Assets/Materials/snowShadow.mat", typeof(Material)) as Material;
        renderer.materials = new Material[] { mat };
        renderer.sortingLayerName = "Shadows";
        renderer.sortingOrder = 0;

        // Create GameObject to render the shadows
        GameObject rainShadows = new GameObject("RainShadows");

        // Add Shadow Controller Script
        shadowController = rainShadows.AddComponent<ShadowController>();
        shadowController.weather = WeatherManager.WeatherType.rain;

        // Inherit values from the original object
        rainShadows.transform.SetParent(obj.gameObject.transform, true);
        rainShadows.transform.position = child.transform.position;
        rainShadows.transform.rotation = child.transform.rotation;
        rainShadows.transform.localScale = child.transform.localScale;

        // Create and configure components
        filter = rainShadows.AddComponent<MeshFilter>();
        filter.sharedMesh = shadow;

        renderer = rainShadows.AddComponent<MeshRenderer>();
        mat = AssetDatabase.LoadAssetAtPath("Assets/Materials/rainShadow.mat", typeof(Material)) as Material;
        renderer.materials = new Material[] { mat };
        renderer.sortingLayerName = "Shadows";
        renderer.sortingOrder = 0;

    }

    void GenerateSunShadows(GameObject obj)
    {
        // Get Map Size
        var tiledMap = obj.transform.root.GetComponent<Tiled2Unity.TiledMap>();
        if (tiledMap == null)
            return;

        float tileSize = tiledMap.TileWidth * tiledMap.ExportScale;
        float mapWidth = tiledMap.GetMapWidthInPixelsScaled();
        float mapHeight = -tiledMap.GetMapHeightInPixelsScaled();

        // Must look at all meshes in children, as the mesh may have been split
        MeshFilter[] meshFilters = obj.transform.GetChild(0).GetComponentsInChildren<MeshFilter>();

        // Grabs all the verts from each mesh and combines it into an array
        Vector2[] verts = meshFilters.SelectMany<MeshFilter, Vector2>(
            m => System.Array.ConvertAll<Vector3, Vector2>(m.sharedMesh.vertices.Where((x, i) => i % 4 == 3).ToArray<Vector3>(),
                v => (Vector2)v)).ToArray<Vector2>();

        //Vector2[] verts = System.Array.ConvertAll<Vector3, Vector2>(meshFilters[0].sharedMesh.vertices, v => (Vector2)v);

        List<Mesh> subShadowMeshes = new List<Mesh>();

        Vector2 perspectiveDifferenceX = new Vector2((34f / 64) * tileSize, 0);
        Vector2 perspectiveDifferenceY = new Vector2(0, (27f / 64) * tileSize);

        for (float x = 0; x < mapWidth; x += tileSize)
        {
            bool shadowTopSet = false;
            bool shadowRightSet = false;

            List<Vector2> shadowVerts = new List<Vector2>();

            for (float y = 0; y > mapHeight; y -= tileSize)
            {
                Vector2 currentTile = new Vector2(x, y);
                Vector2 lowerTile = new Vector2(x, y - tileSize);

                if (!shadowTopSet)
                {
                    if (QuadExists(verts, currentTile) && !QuadExists(verts, lowerTile))
                    {
                        Vector2 leftTile = new Vector2(x - tileSize, y);
                        if (!QuadExists(verts, leftTile))
                        {
                            shadowVerts.Add(lowerTile + perspectiveDifferenceY);
                            shadowVerts.Add(lowerTile + perspectiveDifferenceX);
                        }
                        else
                            shadowVerts.Add(lowerTile);

                        Vector2 lowerRightTile = new Vector2(x + tileSize, y - tileSize);
                        shadowVerts.Add(lowerRightTile);

                        shadowTopSet = true;
                    }
                }
                else
                {
                    Vector2 rightTile = new Vector2(x + tileSize, y);
                    Vector2 lowerRightTile = new Vector2(x + tileSize, y - tileSize);

                    if (QuadExists(verts, currentTile))
                    {
                        if (shadowRightSet)
                        {
                            shadowVerts.Add(rightTile - perspectiveDifferenceY + perspectiveDifferenceX);
                        }
                        else
                        {
                            shadowVerts.Add(rightTile - perspectiveDifferenceY);
                        }

                        Vector2 leftTile = new Vector2(x - tileSize, y);
                        Vector2 upperLeftTile = new Vector2(x - tileSize, y + tileSize);
                        bool shadowToLeft = false;

                        // Check for connecting shadow on the left
                        for (int i = 0; i < subShadowMeshes.Count; i++)
                        {
                            Vector3[] v = subShadowMeshes[i].vertices;
                            if (v[0].x != x - tileSize) continue;
                            if (v[v.Length - 1].y == y || v[v.Length - 1].y == y - perspectiveDifferenceY.y)
                            {
                                shadowToLeft = true;
                                break;
                            }
                        }

                        if (!QuadExists(verts, leftTile) || (!QuadExists(verts, upperLeftTile) && !shadowToLeft))
                        {
                            shadowVerts.Add(currentTile - perspectiveDifferenceY + perspectiveDifferenceX);
                            shadowVerts.Add(currentTile);
                        }
                        else
                        {
                            shadowVerts.Add(currentTile - perspectiveDifferenceY);
                        }
                            
                        // Create and store Mesh
                        subShadowMeshes.Add(CreateMesh(shadowVerts.ToArray()));

                        shadowTopSet = false;
                        shadowRightSet = false;
                        y += tileSize;
                    }
                    else if (QuadExists(verts, rightTile))
                    {
                        if (!shadowRightSet)
                        {
                            if (QuadExists(verts, new Vector2(x + tileSize, y + tileSize)))
                            {
                                shadowVerts.Add(rightTile + perspectiveDifferenceX);
                            }
                            else
                            {
                                shadowVerts.Add(rightTile + perspectiveDifferenceX - perspectiveDifferenceY);
                            }
                        }

                        shadowVerts.Add(lowerRightTile + perspectiveDifferenceX);

                        shadowRightSet = true;
                    }
                    else
                    {
                        if (shadowRightSet)
                        {
                            shadowVerts.Add(rightTile + perspectiveDifferenceY);
                        }
                        shadowVerts.Add(lowerRightTile);

                        shadowRightSet = false;
                    }
                }
            }
        }

        if (subShadowMeshes.Count == 0)
            return;

        Mesh masterShadow = new Mesh();
        CombineInstance[] subShadowCombine = subShadowMeshes.ConvertAll<CombineInstance>(m =>
            {
                CombineInstance c = new CombineInstance();
                c.mesh = m;
                return c;
            }).ToArray();
        masterShadow.CombineMeshes(subShadowCombine, true, false);

        masterShadow.RecalculateBounds();
        // Define normals
        Vector3[] norms = new Vector3[masterShadow.vertexCount];
        for (int i = 0; i < norms.Length; i++ )
        {
            norms[i] = Vector3.forward;
        }

        // Save Mesh
        if (!AssetDatabase.IsValidFolder("Assets/Tiled2Unity/Meshes/Shadows"))
            AssetDatabase.CreateFolder("Assets/Tiled2Unity/Meshes", "Shadows");

        string filePath = "Assets/Tiled2Unity/Meshes/Shadows/" + obj.transform.root.name + "_" + obj.gameObject.name + "_sun_shadows" + ".asset";
        if (AssetDatabase.LoadAssetAtPath(filePath, typeof(System.Object)) != null)
            AssetDatabase.DeleteAsset(filePath);
        AssetDatabase.CreateAsset(masterShadow, filePath);

        // Create GameObject to render the shadows
        GameObject sunShadows = new GameObject("SunShadows");

        // Add Shadow Controller Script
        var shadowController = sunShadows.AddComponent<ShadowController>();
        shadowController.weather = WeatherManager.WeatherType.sun;

        // Inherit values from the original object
        Transform child = obj.transform.GetChild(0);
        sunShadows.transform.SetParent(obj.gameObject.transform, true);
        sunShadows.transform.position = child.transform.position;
        sunShadows.transform.rotation = child.transform.rotation;
        sunShadows.transform.localScale = child.transform.localScale;

        // Create and configure components
        var filter = sunShadows.AddComponent<MeshFilter>();
        filter.sharedMesh = masterShadow;

        var renderer = sunShadows.AddComponent<MeshRenderer>();
        Material mat = AssetDatabase.LoadAssetAtPath("Assets/Materials/sunShadow.mat", typeof(Material)) as Material;
        renderer.materials = new Material[] { mat };
        renderer.sortingLayerName = "Shadows";
        renderer.sortingOrder = 0;
    }

    Mesh CreateMesh(Vector2[] polygon)
    {
        // Triangulate
        Triangulator tr = new Triangulator(polygon);
        int[] indicies = tr.Triangulate();

        // Create mesh
        Mesh mesh = new Mesh();
        mesh.vertices = System.Array.ConvertAll<Vector2, Vector3>(polygon, v => (Vector3)v);
        mesh.triangles = indicies;

        return mesh;
    }

    bool QuadExists(Vector2[] verticies, Vector2 topLeft)
    {
        //for (int i = 3; i < verticies.Length; i += 4)
        for (int i = 0; i < verticies.Length; i++)
        {
            if (verticies[i] == topLeft)
                return true;
        }
        return false;
    }
}
