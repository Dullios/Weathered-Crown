using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
    //public GameObject pauseMenuObject;

    //public GameObject levelMenuObject;

    private bool controlDisplayed = false;
    public GameObject controlOverlay;

    private bool isActive
    {
        get
        {
            return Time.timeScale > 0;
        }
    }
    private bool levelMenuIsActive = false;

    public void Pause()
    {
        //Time.timeScale = 0;
        StartCoroutine(SetTimeScale(0));

        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (SetActiveCanvas(this.transform.GetChild(i).gameObject, true))
            {
                break;
            }
        }
    }

    public void Resume()
    {
        StartCoroutine(SetTimeScale(1));

        EventSystem.current.SetSelectedGameObject(null);

        for (int i = 0; i < this.transform.childCount; i++)
        {
            SetActiveCanvas(this.transform.GetChild(i).gameObject, false);
        }
    }

    public void menuToggle()
    {
        if(isActive)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    public void goToHub()
    {
        Application.LoadLevel("Hub");
    }

    public bool SetActiveCanvas(GameObject gameObject, bool active)
    {
        Canvas canvas = gameObject.GetComponent<Canvas>();
        if (canvas == null)
            canvas = gameObject.GetComponentInChildren<Canvas>();
        if (canvas)
        {
            canvas.gameObject.SetActive(active);
            return true;
        }
        return false;
    }

    IEnumerator SetTimeScale(float timeScale)
    {
        yield return new WaitForEndOfFrame();

        Time.timeScale = timeScale;
    }

    void OnEnable()
    {
        //LevelLoader.instance.LoadLevel("alpha_cloud");
    }

    public void toggleControls()
    {
        if(controlDisplayed)
        {
            controlOverlay.SetActive(false);
        }
        else
        {
            controlOverlay.SetActive(true);
        }

        controlDisplayed = !controlDisplayed;
    }

    public void quitSelect()
    {
        //LevelSelect.Instance.LoadLevel("Title Screen");
        Resume();
        Application.LoadLevel("Title Screen");
        SetActiveCanvas(gameObject, false);
    }
}
