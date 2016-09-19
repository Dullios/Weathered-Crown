using UnityEngine;
using System.Collections;

public class LoadGameMenu : MonoBehaviour {

    public RectTransform list;

    private GameObject _buttonPrefab;
    private GameObject buttonPrefab
    {
        get
        {
            if (_buttonPrefab == null)
            {
                _buttonPrefab = Resources.Load<GameObject>("Menu/LoadButton");
            }

            if (_buttonPrefab == null)
            {
                Debug.LogError("Could not find button prefab");
            }
            return _buttonPrefab;
        }
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        // Remove all children
        for (int i = 0; i < list.childCount; i++)
        {
            Destroy(list.GetChild(i).gameObject);
        }

        // Instantiate Buttons for Levels
        foreach (var sg in LevelSerializer.SavedGames[LevelSerializer.PlayerName])
        {
            GameObject button = GameObject.Instantiate(buttonPrefab) as GameObject;
            LoadGameButton buttonScript = button.GetComponent<LoadGameButton>();

            buttonScript.Init(sg);

            button.transform.localScale = Vector3.one;
            button.transform.SetParent(list, false);
        }
    }
}
