using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlatformController))]
public class MovingPlatformSpline : MonoBehaviour {

    public enum eFollowMode
    {
        normal = 0,
        reverse = 1,
    }

    public bool active = true;
    public List<Vector2> points;
    public eFollowMode followMode;
    public float step = 1f;
    public float cleanUpDist = 0;
    public float speed = 1;
    public bool repeat = false;
    public bool generateLoop = false;
    public bool recalculate = false;

    private List<Vector2> localPoints = new List<Vector2>();
    private List<Vector2> path = new List<Vector2>();
    private int currPathIndex = 0;
    private int maxIndex { get { return path.Count - 2; } }
    private float lerpVal = 0;

    private PlatformController _controller;
    private PlatformController controller
    {
        get
        {
            if (_controller == null)
                _controller = GetComponent<PlatformController>();

            return _controller;
        }
    }

    private void localizePoints()
    {
        localPoints.Clear();
        for(int i = 0; i < points.Count; i++)
        {
            localPoints.Add(points[i] + (Vector2)transform.position);
        }
    }

    private void calculatePath()
    {
        path.Clear();
        currPathIndex = 0;
        lerpVal = 0;

        int cond = generateLoop ? localPoints.Count : localPoints.Count - 3;
        for (int i = 0; i < cond; i++)
        {
            Vector2[] splinePoints = new Vector2[4];

            for (int k = 0; k < 4; k++)
            {
                int j = (i + k) % localPoints.Count;
                splinePoints[k] = localPoints[j];
            }

            CatmullRomSpline crs = new CatmullRomSpline(splinePoints[0], splinePoints[1], splinePoints[2], splinePoints[3]);
            crs.InitNonuniformCatmullRom();

            for (float j = 0; j < 1.0f; )
            {
                Vector3 p = crs.Eval(j);
                Vector3 deriv = crs.Deriv(j);

                j += step / deriv.magnitude;

                path.Add(p);
            }

            Vector3 diff = path[path.Count - 1] - splinePoints[2];
            if (diff.magnitude >= cleanUpDist)
                path.RemoveAt(path.Count - 1);

            if (i == cond - 1)
                path.Add(splinePoints[2]);
        }

    }

    void Start()
    {
        localizePoints();
        calculatePath();

        if (followMode == eFollowMode.reverse)
            currPathIndex = maxIndex;
    }

    void Update()
    {
        if(recalculate)
        {
            localizePoints();
            calculatePath();
            recalculate = false;
        }

        if (active)
        {
            UpdateFollow();
        }
    }

    void UpdateFollow()
    {
        switch (followMode)
        {
            case eFollowMode.normal:
                UpdateNormal();
                break;

            case eFollowMode.reverse:
                UpdateReverse();
                break;
        }
    }

    void UpdateNormal()
    {
        if (currPathIndex > maxIndex && !repeat) return;

        lerpVal += Time.deltaTime * speed;

        int inc = Mathf.FloorToInt(lerpVal);

        lerpVal -= inc;
        currPathIndex += inc;

        while (currPathIndex > maxIndex)
        {
            if (repeat)
                currPathIndex -= maxIndex + 1;
            else
            {
                currPathIndex = maxIndex;
                lerpVal = 1;
            }
        }

        Vector2 p1 = path[currPathIndex];
        Vector2 p2 = path[currPathIndex + 1];

        controller.move =  Vector2.Lerp(p1, p2, lerpVal) - (Vector2)transform.position;
    }

    void UpdateReverse()
    {
        if (currPathIndex < 0 && !repeat) return;

        lerpVal += Time.deltaTime * speed;

        int dec = Mathf.FloorToInt(lerpVal);

        lerpVal -= dec;
        currPathIndex -= dec;

        while (currPathIndex < 0)
        {
            if (repeat)
                currPathIndex += maxIndex + 1;
            else
            {
                currPathIndex = 0;
                lerpVal = 1;
            }

        }

        Vector2 p1 = path[currPathIndex];
        Vector2 p2 = path[currPathIndex + 1];

        //transform.position = Vector2.Lerp(p2, p1, lerpVal);

    }

    void OnDrawGizmosSelected()
    {
        //localizePoints();

        if (recalculate)
        {
            calculatePath();
            recalculate = false;
        }

        Gizmos.color = Color.cyan;
        foreach (Vector3 p in localPoints)
        {
            Gizmos.DrawSphere(p, 0.2f);
        }

        for (int i = 1; i < path.Count; i++)
        {
            Gizmos.DrawLine(path[i - 1], path[i]);
        }
    }
}
