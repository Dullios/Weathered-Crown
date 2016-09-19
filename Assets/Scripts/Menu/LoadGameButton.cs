using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadGameButton : MonoBehaviour {

    private LevelSerializer.SaveEntry saveGame;

    public void Init(LevelSerializer.SaveEntry saveGame)
    {
        this.saveGame = saveGame;

        Text nameText = transform.GetChild(0).GetComponent<Text>();
        nameText.text = saveGame.Caption;
    }

    public void Load()
    {
        saveGame.Load();
        GameManager.Instance.Resume();
    }
}
