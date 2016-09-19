using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[DisallowMultipleComponent]
public class MenuHelper : MonoBehaviour {

    void OnEnable()
    {
        StartCoroutine(SetActiveButton());
    }

    IEnumerator SetActiveButton()
    {
        yield return new WaitForEndOfFrame();

        Button button = GetComponent<Button>();
        if (button == null)
            button = GetComponentInChildren<Button>();

        if (button != null)
            EventSystem.current.SetSelectedGameObject(button.gameObject);
    }
}
