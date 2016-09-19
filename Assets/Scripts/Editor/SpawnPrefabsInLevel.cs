using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SpawnPrefabsInLevel : EditorWindow{

    GameObject prefab;
    GameObject instance;
    Queue<ReplaceWithPrefab> objectsToReplace = null;

    Vector2 scrollPos;

    public static void Init(string rootPrefabPath)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath(rootPrefabPath, typeof(GameObject)) as GameObject;
        GameObject instance = GameObject.Instantiate<GameObject>(prefab);
        
        if (prefab == null) return;

        var otr = instance.GetComponentsInChildren<ReplaceWithPrefab>();

        if (otr.Length > 0)
        {
            var window = ScriptableObject.CreateInstance<SpawnPrefabsInLevel>();

            window.prefab = prefab;
            window.instance = instance;
            window.objectsToReplace = new Queue<ReplaceWithPrefab>(otr);
            window.objectsToReplace.TrimExcess();

            window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 350);
            window.Show();
        }
        else
            DestroyImmediate(instance);
    }

    public static void ReplaceObjectWithPrefab(GameObject gameObject, string prefabPath)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
        if (prefab == null) return;

        // Create Prefab and carry over the important properties from the gameobject
        var pInstance = GameObject.Instantiate<GameObject>(prefab);

        pInstance.transform.SetParent(gameObject.transform.parent, true);

        pInstance.transform.localPosition = gameObject.transform.localPosition;
        pInstance.transform.localRotation = gameObject.transform.localRotation;
        pInstance.transform.localScale = gameObject.transform.localScale;

        // Remove "(Clone)" from the name
        pInstance.name = pInstance.name.Remove(pInstance.name.Length - 7);

        // Destroy old gameobject
        GameObject.DestroyImmediate(gameObject);
    }

    public static void ReplaceObjectWithPrefab(ref GameObject gameObject, string prefabPath)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
        if (prefab == null) return;

        // Create Prefab and carry over the important properties from the gameobject
        var pInstance = GameObject.Instantiate<GameObject>(prefab);

        pInstance.transform.SetParent(gameObject.transform.parent, true);

        pInstance.transform.localPosition = gameObject.transform.localPosition;
        pInstance.transform.localRotation = gameObject.transform.localRotation;
        pInstance.transform.localScale = gameObject.transform.localScale;

        // Destroy old gameobject
        GameObject.DestroyImmediate(gameObject);

        // Update reference
        gameObject = pInstance;
    }

    void OnGUI()
    {
        if (objectsToReplace == null)
        {
            this.Close();
            return;
        }

        string[] possibleAssets = objectsToReplace.Peek().potentialPrefabPaths;

        int index = -1;
        EditorGUILayout.BeginVertical();

        //GUILayout.Label("More than one prefab was found with the name '" + unpr.search + ".'");
        GUILayout.Label("Please select the desired asset.");

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
        for (int i = 0; i < possibleAssets.Length; i++)
        {
            if (GUILayout.Button(possibleAssets[i]) && index < 0)
            {
                index = i;
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();

        if (index >= 0)
        {
            var obj = objectsToReplace.Dequeue();

            ReplaceObjectWithPrefab(obj.gameObject, obj.potentialPrefabPaths[index]);

            scrollPos = Vector2.zero;
        }

        // If there are no more the check, close the window
        if (objectsToReplace.Count == 0)
        {
            SavePrefab();

            this.Close();
        }
    }

    void SavePrefab()
    {
        PrefabUtility.ReplacePrefab(instance, prefab);

        GameObject.DestroyImmediate(instance);
    }

    //static void ProcessQueue()
    //{
    //    if (objectsToReplace == null) return;

    //    while (objectsToReplace.Count > 0)
    //    {
    //        string[] potentialAssets = AssetDatabase.FindAssets(objectsToReplace.Dequeue().desiredPrefabName, folders);
    //        if (potentialAssets.Length == 1)
    //        {
    //             Load Immediately
    //        }
    //        else if (potentialAssets.Length > 1)
    //        {
    //             Show Dialogue to select proper asset

    //            var window = ScriptableObject.CreateInstance<SelectPrefabWizard>();
    //            window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 350);
    //            window.ShowPopup();
    //        }
    //        else
    //        {
    //             Error
    //        }

    //    }
    //}

    //static void ReplaceObject(string )


//    static Queue<string> prefabsToUpdate = new Queue<string>();
//    static SelectPrefabWizard window;

//    private string[] assetList;
//    private Vector2 scrollPos;

//    [MenuItem("Window/Select Prefab Test")]
//    public static void Test()
//    {
//        int n = Random.Range(1, 5);
//        List<string> testAssets = new List<string>();
//        for (int i = 0; i < n; i++)
//            testAssets.Add("Test " + i);

//        Add(null, testAssets.ToArray(), "");
//    }

//    public static void Add(GameObject gameObject, string[] assets, string searchString)
//    {
//        if (window == null)
//        {
//            window = ScriptableObject.CreateInstance<SelectPrefabWizard>();
//            window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 250);
//            window.ShowPopup();
//        }
//        //unknownPrefabs.Enqueue(new UnknownPrefab(gameObject, assets, searchString));
//    }

//    void OnGUI()
//    {
//        //UnknownPrefab unpr = unknownPrefabs.Peek();
//        //int index = -1;
//        //EditorGUILayout.BeginVertical();

//        //GUILayout.Label("More than one prefab was found with the name '" + unpr.search + ".'");
//        //GUILayout.Label("Please select the desired asset.");

//        //scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
//        //for (int i = 0; i < unpr.possibleAssets.Length; i++)
//        //{
//        //    if (GUILayout.Button(unpr.possibleAssets[i]) && index < 0)
//        //    {
//        //        index = i;
//        //    }
//        //}
//        //EditorGUILayout.EndScrollView();

//        //EditorGUILayout.EndVertical();

//        //if (index >= 0)
//        //{
//        //    unpr.Init(index);
//        //    unknownPrefabs.Dequeue();
//        //    scrollPos = Vector2.zero;
//        //}
//        //// If there are no more the check, close the window
//        //if (unknownPrefabs.Count == 0)
//        //    this.Close();
//    }

//    void OnDestroy()
//    {
//        window = null;
//    }
    
//    //class UnknownPrefab
//    //{
//    //    public GameObject gameObject;
//    //    public string[] possibleAssets;
//    //    public string search;

//    //    public UnknownPrefab(GameObject gameObject, string[] possibleAssets, string search)
//    //    {
//    //        this.gameObject = gameObject;
//    //        this.possibleAssets = possibleAssets;
//    //        this.search = search;
//    //    }

//    //    public void Save(int index)
//    //    {
//    //        if (string.IsNullOrEmpty(search))
//    //            return;

//    //        if (index >= 0 && index < possibleAssets.Length)
//    //        {
//    //        // Save into levels
//    //        string prefabPath = String.Format("Assets/Resources/Levels/{0}.prefab", prefabName);
//    //        //string prefabPath = ImportUtils.GetPrefabPathFromName(prefabName);

//    //        UnityEngine.Object finalPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

//    //        if (finalPrefab == null)
//    //        {
//    //            // The prefab needs to be created
//    //            ImportUtils.ReadyToWrite(prefabPath);
//    //            finalPrefab = PrefabUtility.CreateEmptyPrefab(prefabPath);
//    //        }

//    //        // Replace the prefab, keeping connections based on name.
//    //        PrefabUtility.ReplacePrefab(tempPrefab, finalPrefab, ReplacePrefabOptions.ReplaceNameBased);

//    //        }
//    //    }
//    //}
}


