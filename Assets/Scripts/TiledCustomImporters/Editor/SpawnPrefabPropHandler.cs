using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tiled2Unity;

[CustomTiledImporter]
public class SpawnPrefabPropHandler : ICustomTiledImporter {

    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> customProperties)
    {
        if (customProperties.ContainsKey("prefab"))
        {
            string targetAsset = customProperties["prefab"];

            string[] potentialPrefabs = AssetDatabase.FindAssets(targetAsset, new string[] { "Assets/Prefabs", "Assets/Resources" });

            if (potentialPrefabs.Length == 1)
            {
                // Save Prefab
                SpawnPrefabsInLevel.ReplaceObjectWithPrefab(ref gameObject, AssetDatabase.GUIDToAssetPath(potentialPrefabs[0]));
            }
            else if (potentialPrefabs.Length > 1)
            {
                // Mark it for the user to select the proper prefab after the import process is done
                string[] potentialPrefabPaths = potentialPrefabs.Select(t => AssetDatabase.GUIDToAssetPath(t)).ToArray();
                var g = gameObject.AddComponent<ReplaceWithPrefab>();
                g.potentialPrefabPaths = potentialPrefabPaths;
            }
            else
            {
                // Error
            }
        }
    }

    public void CustomizePrefab(GameObject prefab)
    {
        // Automatically add a camera to the level
        string cameraAssetPath = "Assets/Prefabs/Main Camera.prefab";

        var cameraAsset = AssetDatabase.LoadAssetAtPath(cameraAssetPath, typeof(GameObject)) as GameObject;
        var camera = GameObject.Instantiate<GameObject>(cameraAsset);

        camera.transform.SetParent(prefab.transform, true);
    }

}
