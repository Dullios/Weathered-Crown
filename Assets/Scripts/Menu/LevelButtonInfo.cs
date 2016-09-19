using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelButtonInfo : MonoBehaviour {

    [HideInInspector]
    public LevelInfo levelInfo;

    public void Rename()
    {
        Text nameText = transform.Find("LevelNameBG").GetChild(0).GetComponent<Text>();
        nameText.text = levelInfo.name;
    }

    public void Load()
    {
        LevelSelect.Instance.LoadLevel(levelInfo);
    }

    public void Load(int weather)
    {
        LevelSelect.Instance.LoadLevel(levelInfo, (WeatherManager.WeatherType)weather);
    }
}
