/*
 * Written by: Russell Brabers
 */

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class TitleMenuManager : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        var hubManagers = GameObject.FindObjectsOfType<HubManager>();
        for (int i = 0; i < hubManagers.Length; i++)
            Destroy(hubManagers[i].gameObject);

        //DontDestroyOnLoad(gameObject);
	}
	
    public void newGameSelect(string newGameLevel)
    {
        Application.LoadLevel(newGameLevel);
        //LevelSelect.Instance.LoadLevel(newGameLevel);

        //HubManager.Instance.sunCompleted = false;
        //HubManager.Instance.rainCompleted = false;
        //HubManager.Instance.snowCompleted = false;
    }

    public void exitSelect()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    //void OnLevelWasLoaded(int level)
    //{
    //    //LevelSelect levelSelectScript = FindObjectOfType<LevelSelect>();
    //    //levelSelectScript.LoadLevel("alpha_cloud");
    //}
}