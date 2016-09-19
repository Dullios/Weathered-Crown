using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tiled2Unity;

[CustomTiledImporter]
public class ImportParticleColliders : ICustomTiledImporter {
    
    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> customProperties)
    {}

    public void CustomizePrefab(GameObject prefab)
    {
        var collisionObjects = FindChildrenByName(prefab.transform, "Collision").ConvertAll<GameObject>(t => t.gameObject);

        int i = 0;
        foreach (GameObject collisionObject in collisionObjects)
        {
            var polyCol = collisionObject.GetComponent<PolygonCollider2D>();
            if (polyCol == null)
                continue;

            GameObject particleCollision = new GameObject("ParticleCollision");
            particleCollision.transform.SetParent(collisionObject.transform.parent);

            var meshFilter = particleCollision.AddComponent<MeshFilter>();
            Mesh m = PolyColToMesh(polyCol);

            if (!AssetDatabase.IsValidFolder("Assets/Tiled2Unity/Meshes/Collisions"))
                AssetDatabase.CreateFolder("Assets/Tiled2Unity/Meshes", "Collisions");

            string filePath = "Assets/Tiled2Unity/Meshes/Collisions/" + prefab.name + "_" + i++ + ".asset";
            if (AssetDatabase.LoadAssetAtPath(filePath, typeof(System.Object)) != null)
                AssetDatabase.DeleteAsset(filePath);
            AssetDatabase.CreateAsset(m, filePath);

            meshFilter.sharedMesh = m;

            particleCollision.AddComponent<MeshCollider>();
        }
    }

    List<Transform> FindChildrenByName(Transform parent, string name)
    {
        List<Transform> children = new List<Transform>();

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == name)
            {
                children.Add(child);
            }

            if (child.childCount > 0)
                children.AddRange(FindChildrenByName(child, name));
        }

        return children;
    }

    Mesh PolyColToMesh(PolygonCollider2D polyCol, float zDepth = 1)
    {
        CombineInstance[] subMeshes = new CombineInstance[polyCol.pathCount];

        for (int i = 0; i < polyCol.pathCount; i++)
        {
            var verticies2D = polyCol.GetPath(i);
            int n = verticies2D.Length;

            // Get triangle indices for the front face
            Triangulator tr = new Triangulator(verticies2D);
            int[] indicies2D = tr.Triangulate();
            List<int> indicies = new List<int>(indicies2D);

            // Create all 3D verts
            Vector3[] verticies = new Vector3[2 * verticies2D.Length];
            for (int j = 0; j < verticies2D.Length; j++)
            {
                // Front face
                verticies[j] = new Vector3(verticies2D[j].x, verticies2D[j].y, 0);

                // Back face
                verticies[j + verticies2D.Length] = new Vector3(verticies2D[j].x, verticies2D[j].y, zDepth);
            }

            // Create triangles for back face
            for (int j = 0; j < indicies2D.Length; j += 3)
            {
                int a = indicies2D[j] + n;
                int b = indicies2D[j + 1] + n;
                int c = indicies2D[j + 2] + n;

                indicies.AddRange(new int[] { c, b, a });
            }

            // Create boundary edge list
            List<Vector2> boundaryEdges = new List<Vector2>();
            // First create all edges
            for (int j = 0; j < indicies2D.Length; j += 3)
            {
                int a = indicies2D[j];
                int b = indicies2D[j + 1];
                int c = indicies2D[j + 2];

                boundaryEdges.Add(new Vector2(a, b));
                boundaryEdges.Add(new Vector2(b, c));
                boundaryEdges.Add(new Vector2(c, a));
            }

            // Remove similar edges
            for (int j = 0; j < boundaryEdges.Count; j++)
            {
                for (int k = j + 1; k < boundaryEdges.Count; k++)
                {
                    if ((boundaryEdges[j].x == boundaryEdges[k].y && boundaryEdges[j].y == boundaryEdges[k].x) ||
                        false)//(boundaryEdges[j].x == boundaryEdges[k].x && boundaryEdges[j].y == boundaryEdges[k].y))
                    {
                        boundaryEdges.RemoveAt(k);
                        boundaryEdges.RemoveAt(j);

                        j--;

                        break;
                    }
                }
            }

            // Create triangles for sides
            for (int j = 0; j < boundaryEdges.Count; j++)
            {
                indicies.AddRange(new int[] { (int)boundaryEdges[j].y + n, (int)boundaryEdges[j].y, (int)boundaryEdges[j].x });
                indicies.AddRange(new int[] { (int)boundaryEdges[j].x, (int)boundaryEdges[j].x + n, (int)boundaryEdges[j].y + n});
            }

            // Create the mesh
            Mesh temp = new Mesh();
            temp.vertices = verticies;
            temp.triangles = indicies.ToArray();
            temp.RecalculateNormals();
            temp.RecalculateBounds();

            subMeshes[i].mesh = temp;
        }

        // Combine all meshes and return
        Mesh mesh = new Mesh();
        mesh.CombineMeshes(subMeshes, true, false);
        mesh.Optimize();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}
