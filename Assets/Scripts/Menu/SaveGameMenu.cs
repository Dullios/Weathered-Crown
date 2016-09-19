using UnityEngine;
using System.Collections;

public class SaveGameMenu : MonoBehaviour {

    public void SaveGame()
    {
        LevelSerializer.SaveGame("Save");
    }
}
