using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using Tiled2Unity;
public class LevelSelect : Singleton<LevelSelect> {

    public GameObject levelList;

    private GameObject _buttonPrefab;
    private GameObject buttonPrefab
    {
        get
        {
            if (_buttonPrefab == null)
            {
                _buttonPrefab = Resources.Load<GameObject>("Menu/LevelButton");
            }

            if (_buttonPrefab == null)
            {
                Debug.LogError("Could not find button prefab");
            }
            return _buttonPrefab;
        }
    }

    private RectTransform _buttonListParent;
    private RectTransform buttonListParent
    {
        get
        {
            if (_buttonListParent == null)
            {
                _buttonListParent = levelList.GetComponent<RectTransform>();
                if (_buttonListParent == null)
                {
                    Debug.LogError("Can not find list parent");
                }
            }

            return _buttonListParent;
        }
    }

    void Start()
    {
        //StartCoroutine(OnLevelSelectEnable());
        Refresh();
    }

    IEnumerator OnLevelSelectEnable()
    {
        // Wait for enable
        while (!levelList.activeInHierarchy)
            yield return null;
        yield return null;

        Refresh();

        // Wait for disable
        //while (levelList.activeInHierarchy)
        //    yield return null;

        //StartCoroutine(OnLevelSelectEnable());
    }

    public void Refresh()
    {
        LevelInfo[] levels = Resources.LoadAll<LevelInfo>("Levels");

        // Remove all children
        for (int i = 0; i < buttonListParent.transform.childCount; i++)
        {
            Destroy(buttonListParent.transform.GetChild(i).gameObject);
        }

        // Instantiate Buttons for Levels
        for (int i = 0; i < levels.Length; i++)
        {
            GameObject levelButton = GameObject.Instantiate(buttonPrefab) as GameObject;
            LevelButtonInfo buttonInfo = levelButton.GetComponent<LevelButtonInfo>();

            buttonInfo.levelInfo = levels[i];
            buttonInfo.Rename();

            levelButton.transform.localScale = Vector3.one;
            //levelButton.transform.parent = buttonListParent;
            levelButton.transform.SetParent(buttonListParent, false);
        }
    }

    public void LoadLevel(string levelName)
    {
        LevelInfo level = Resources.Load<LevelInfo>("Levels/" + levelName);

        LoadLevel(level);
    }

    public void LoadLevel(LevelInfo level)
    {
        LoadLevel(level, level.defaultWeather);
    }

    public void LoadWeatherLevel(string levelName, WeatherManager.WeatherType weather)
    {
        LevelInfo level = Resources.Load<LevelInfo>("Levels/" + levelName);

        LoadLevel(level, weather);
    }

    public void LoadWeatherLevel(LevelInfo level, WeatherManager.WeatherType weather)
    {
        LoadLevel(level, weather);
    }

    public void LoadLevel(LevelInfo level, WeatherManager.WeatherType weather)
    {
        Debug.Log("Loading Level: " + level.name + "  Weather: " + weather.ToString());

        // Unload current level
        LevelInfo curr = FindObjectOfType<LevelInfo>();
        if (curr != null)
            Destroy(curr.gameObject);

        // Load new level
        Instantiate(level.gameObject);
        //WeatherScript.Instance.type = weather;
        StartCoroutine(SetWeather(weather));
    }

    public IEnumerator SetWeather(WeatherManager.WeatherType weather)
    {
        yield return new WaitForEndOfFrame();

        WeatherManager.Instance.type = weather;
    }
}
