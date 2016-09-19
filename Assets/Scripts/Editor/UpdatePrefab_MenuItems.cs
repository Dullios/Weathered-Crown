using UnityEngine;
using UnityEditor;
using System.Collections;

public class UpdatePrefab_MenuItems
{

    [MenuItem("Tools/Update Prefabs/Player")]
    static void UpdatePlayer()
    {
        GameObject newPlayer = Resources.Load<GameObject>("Player");

        GameObject[] levels = Resources.LoadAll<GameObject>("Levels/");
        foreach (var levelPrefab in levels)
        {
            var level = PrefabUtility.InstantiatePrefab(levelPrefab) as GameObject;
            var oldPlayerTrans = level.transform.GetComponentInChildren<PlayerAffectorListener>();
            if (oldPlayerTrans != null)
            {
                GameObject oldPlayer = oldPlayerTrans.gameObject;

                GameObject repPlayer = PrefabUtility.InstantiatePrefab(newPlayer) as GameObject;

                repPlayer.transform.position = oldPlayer.transform.position;
                repPlayer.transform.SetParent(oldPlayer.transform.parent);

                GameObject.DestroyImmediate(oldPlayer);

                PrefabUtility.ReplacePrefab(level, levelPrefab);
            }
            GameObject.DestroyImmediate(level);
        }
    }

    [MenuItem("Tools/Update Prefabs/Camera")]
    static void UpdateCamera()
    {
        GameObject newCamera = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Main Camera.prefab", typeof(GameObject)) as GameObject;

        GameObject[] levels = Resources.LoadAll<GameObject>("Levels/");
        foreach (var levelPrefab in levels)
        {
            var level = PrefabUtility.InstantiatePrefab(levelPrefab) as GameObject;
            var oldCameraComp = level.transform.GetComponentInChildren<Camera>();
            if (oldCameraComp != null)
            {
                GameObject oldCamera = oldCameraComp.gameObject;

                GameObject repCamera = PrefabUtility.InstantiatePrefab(newCamera) as GameObject;

                repCamera.transform.position = oldCamera.transform.position;
                repCamera.transform.SetParent(oldCamera.transform.parent);

                GameObject.DestroyImmediate(oldCamera);

                PrefabUtility.ReplacePrefab(level, levelPrefab);
            }
            GameObject.DestroyImmediate(level);
        }
    }

    [MenuItem("Tools/Update Prefabs/Shadows")]
    static void UpdateShadows()
    {
        GameObject[] levels = Resources.LoadAll<GameObject>("Levels/");
        foreach (var levelPrefab in levels)
        {
            var level = PrefabUtility.InstantiatePrefab(levelPrefab) as GameObject;

            ImportStaticShadows iss = new ImportStaticShadows();

            iss.CustomizePrefab(level);

            PrefabUtility.ReplacePrefab(level, levelPrefab);

            GameObject.DestroyImmediate(level);
        }
    }

    [MenuItem("Tools/Update Prefabs/Particle Colliders")]
    static void UpdateParticleColliders()
    {
        GameObject[] levels = Resources.LoadAll<GameObject>("Levels/");
        foreach (var levelPrefab in levels)
        {
            var level = PrefabUtility.InstantiatePrefab(levelPrefab) as GameObject;

            ImportParticleColliders ipc = new ImportParticleColliders();

            ipc.CustomizePrefab(level);

            PrefabUtility.ReplacePrefab(level, levelPrefab);

            GameObject.DestroyImmediate(level);
        }
    }
}
