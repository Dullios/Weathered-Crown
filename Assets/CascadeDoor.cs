using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CascadeDoor : MonoBehaviour {

    // For editor testing
    public bool open = false, close = false;

    public float tileSize = 2;
    public float speed = 5;

    public GameObject bottom;
    public GameObject bodyParent;

    public List<GameObject> bodyTiles;

    public enum doorState
    {
        open,
        opening,
        closed,
        closing
    }
    public doorState state = doorState.closed;

    void Start()
    {
        for (int i = 0; i < bodyParent.transform.childCount; i++)
        {
            bodyTiles.Add(bodyParent.transform.GetChild(i).gameObject);
        }
        bodyTiles.Sort((a, b) => b.transform.position.y.CompareTo(a.transform.position.y));
    }

    void Update()
    {
        if (open)
        {
            Open();
            open = false;
        }
        if (close)
        {
            Close();
            close = false;
        }
    }

    public void Open()
    {
        StartCoroutine(AnimateOpen());
    }

    IEnumerator AnimateOpen()
    {
        while (state == doorState.closing)
            yield return null;

        if (state == doorState.open || state == doorState.opening)
            yield break;

        state = doorState.opening;

        Vector3 start = bodyTiles[0].transform.position;
        Vector3 end = start + new Vector3(0, tileSize, 0);
        for (int i = 0; i < bodyTiles.Count; i++)
        {
            float lerp = 0;
            while (lerp <= 1)
            {
                float delta = Mathf.Lerp(start.y, end.y, lerp);
                lerp += Time.deltaTime * speed;
                delta = Mathf.Lerp(start.y, end.y, lerp) - delta;

                for (int j = i; j < bodyTiles.Count; j++)
                {
                    bodyTiles[j].transform.Translate(new Vector3(0, delta, 0));
                }
                bottom.transform.Translate(new Vector3(0, delta, 0));

                yield return null;
            }
        }

        state = doorState.open;
    }

    public void Close()
    {
        StartCoroutine(AnimateClose());
    }

    IEnumerator AnimateClose()
    {
        while (state == doorState.opening)
            yield return null;

        if (state == doorState.closed || state == doorState.closing)
            yield break;

        state = doorState.closing;

        Vector3 start = bodyTiles[0].transform.position;
        Vector3 end = start - new Vector3(0, tileSize, 0);
        for (int i = bodyTiles.Count - 1; i >= 0 ; i--)
        {
            float lerp = 0;
            while (lerp <= 1)
            {
                float delta = Mathf.Lerp(start.y, end.y, lerp);
                lerp += Time.deltaTime * speed;
                delta = Mathf.Lerp(start.y, end.y, lerp) - delta;

                for (int j = i; j < bodyTiles.Count; j++)
                {
                    bodyTiles[j].transform.Translate(new Vector3(0, delta, 0));
                }
                bottom.transform.Translate(new Vector3(0, delta, 0));

                yield return null;
            }
        }

        state = doorState.closed;
    }
}
