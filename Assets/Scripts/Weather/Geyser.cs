using UnityEngine;
using System.Collections;

public class Geyser : MonoBehaviour {

    public enum GeyserDirection
    {
        none,
        up,
        down
    };

    public GeyserDirection direction = GeyserDirection.none;

    [HideInInspector]
    private GeyserDirection _oldDirection = GeyserDirection.none;
    public GeyserDirection oldDirection { get { return _oldDirection; } }


    public float speed = 1;
    public int maxHeightInTiles = 10;
    
    public GameObject topPrefab;
    public GameObject stackBlockPrefab;

    Transform top;
    Transform[] stackBlockPool;

    Coroutine currentAction = null;
    public bool isMoving { get { return currentAction != null; } }
    
    float stackBlockHeight;
    int _stackIndex = 0;

    public int stackClipCount = 4;

    int stackIndex
    {
        get { return _stackIndex; }
        set
        {
            _stackIndex = Mathf.Clamp(value, 0, stackBlockPool.Length - 1);
        }
    }

    float lerpVal = 0;
    int stackClipNameHash;

	// Use this for initialization
	void Start () {
        top = Instantiate(topPrefab).transform;
        top.SetParent(this.transform);
        top.localPosition = Vector3.zero;

        stackBlockHeight = stackBlockPrefab.GetComponent<Renderer>().bounds.size.y;

        stackBlockPool = new Transform[maxHeightInTiles - 1];
        for (int i = 0; i < stackBlockPool.Length; i++)
        {
            stackBlockPool[i] = Instantiate(stackBlockPrefab).transform;

            stackBlockPool[i].SetParent(this.transform);
            stackBlockPool[i].localPosition = new Vector3(0, i * stackBlockHeight, 0);

            stackBlockPool[i].gameObject.SetActive(i == 0);
        }

        stackClipNameHash = stackBlockPrefab.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).shortNameHash;

        StartCoroutine(WatchForDirectionSwitch());
	}

    IEnumerator WatchForDirectionSwitch()
    {
        GeyserDirection currentDirection = GeyserDirection.none;
        while (true)
        {
            while (currentDirection == direction)
                yield return null;

            if (direction != GeyserDirection.none)
                _oldDirection = direction;

            currentDirection = direction;

            if (currentAction != null)
                StopCoroutine(currentAction);

            switch (direction)
            {
                case GeyserDirection.none:
                    currentAction = null;
                    break;
                case GeyserDirection.up:
                    if (lerpVal == -1)
                    {
                        if (stackIndex == 0)
                            lerpVal = 0;
                        else
                            lerpVal = 1;
                    }

                    currentAction = StartCoroutine(MoveUp());
                    break;
                case GeyserDirection.down:
                    if (lerpVal == -1)
                    {
                        if (stackIndex == 0)
                            lerpVal = 0;
                        else
                            lerpVal = 1;
                    }

                    currentAction = StartCoroutine(MoveDown());
                    break;
            }
        }
    }

    IEnumerator MoveUp()
    {
        if (stackIndex == stackBlockPool.Length - 1 && lerpVal == -1)
        {
            Vector3 pos = top.transform.localPosition;
            pos.y = (stackIndex + 1) * stackBlockHeight;
            top.transform.localPosition = pos;

            currentAction = null;
            yield break;
        }
        if (lerpVal > 1) lerpVal = 0;

        Transform t = this.transform;
        float lastStackBlock = stackIndex * stackBlockHeight;
        float nextStackBlock = (stackIndex + 1) * stackBlockHeight;

        while (lerpVal <= 1)
        {
            Vector3 pos = top.transform.localPosition;

            pos.y = Mathf.Lerp(lastStackBlock, nextStackBlock, lerpVal);

            top.transform.localPosition = pos;

            lerpVal += Time.deltaTime * speed;

            yield return new WaitForFixedUpdate();
        }
        if (stackIndex == stackBlockPool.Length - 1)
            lerpVal = -1;
        else
        {
            stackIndex++;
            stackBlockPool[stackIndex].gameObject.SetActive(true); 
            
            Animator anim = stackBlockPool[0].GetComponent<Animator>();
            float normTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            anim.Play(stackClipNameHash, 0, normTime);

            for (int i = 1; i <= stackIndex; i++)
            {
                normTime -= 1f / stackClipCount;
                stackBlockPool[i].GetComponent<Animator>().Play(stackClipNameHash, 0, normTime);
            }

            ////yield return null;

            //Animator anim1 = stackBlockPool[stackIndex].GetComponent<Animator>();



            ////yield return null;

            //AnimatorStateInfo animState = anim1.GetCurrentAnimatorStateInfo(0);
            //float normTime = animState.normalizedTime;

            //Animator anim2 = stackBlockPool[stackIndex].GetComponent<Animator>();
            ////normTime += 1 / (float)stackClipCount;
            ////normTime -= (int)normTime;
            ////if (normTime >= 1) normTime -= 1f;
            //anim1.Play("default", 0, normTime);
            //anim2.Play("default", 0, normTime);
        }

        currentAction = StartCoroutine(MoveUp());
    }
	
    IEnumerator MoveDown()
    {
        if (stackIndex == 0 && lerpVal == -1)
        {
            Vector3 pos = top.transform.localPosition;
            pos.y = 0;
            top.transform.localPosition = pos;

            currentAction = null;
            yield break;
        }
        if (lerpVal < 0) lerpVal = 1;

        Transform t = this.transform;
        float lastStackBlock = stackIndex * stackBlockHeight;
        float nextStackBlock = (stackIndex + 1) * stackBlockHeight;

        while (lerpVal >= 0)
        {
            Vector3 pos = top.transform.localPosition;

            pos.y = Mathf.Lerp(lastStackBlock, nextStackBlock, lerpVal);

            top.transform.localPosition = pos;

            lerpVal -= Time.deltaTime * speed;

            yield return null;
        }

        if (stackIndex == 0)
            lerpVal = -1;
        else
        {
            stackBlockPool[stackIndex].gameObject.SetActive(false);
            stackIndex--;
        }

        currentAction = StartCoroutine(MoveDown());
    }

    void Update()
    {
        //for (int i = 0; i < stackBlockPool.Length; i++)
        //{
        //    var a = stackBlockPool[i].GetComponent<Animator>();
        //    var asi = a.GetCurrentAnimatorStateInfo(0);
        //    stackNormTimes[i] = asi.normalizedTime;
        //}
    }

}
