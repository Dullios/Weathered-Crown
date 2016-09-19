using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatmullRomSpline
{
    public Vector2 p0, p1, p2, p3;
    public CubicPolynomial cpX, cpY;

    public CatmullRomSpline(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        this.p0 = p0;
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;

        cpX = new CubicPolynomial();
        cpY = new CubicPolynomial();
    }
    public void InitNonuniformCatmullRom()
    {
        float dt0 = Mathf.Pow((p0 - p1).sqrMagnitude, 0.25f);
        float dt1 = Mathf.Pow((p1 - p2).sqrMagnitude, 0.25f);
        float dt2 = Mathf.Pow((p2 - p3).sqrMagnitude, 0.25f);

        float xt1, xt2;
        float yt1, yt2;

        ComputeTangents(p0.x, p1.x, p2.x, p3.x, dt0, dt1, dt2, out xt1, out xt2);
        cpX.Init(p1.x, p2.x, xt1, xt2);

        ComputeTangents(p0.y, p1.y, p2.y, p3.y, dt0, dt1, dt2, out yt1, out yt2);
        cpY.Init(p1.y, p2.y, yt1, yt2);
    }

    public void ComputeTangents(float v0, float v1, float v2, float v3, float dt0, float dt1, float dt2, out float t1, out float t2)
    {
        t1 = (v1 - v0) / dt0 - (v2 - v0) / (dt0 + dt1) + (v2 - v1) / dt1;
        t2 = (v2 - v1) / dt1 - (v3 - v1) / (dt1 + dt2) + (v3 - v2) / dt2;

        t1 *= dt1;
        t2 *= dt1;
    }

    public Vector2 Eval(float t)
    {
        return new Vector2(cpX.Evaluate(t), cpY.Evaluate(t));
    }

    public Vector2 Deriv(float t)
    {
        return new Vector2(cpX.Derivative(t), cpY.Derivative(t));
    }

    public struct CubicPolynomial
    {
        private float c0, c1, c2, c3;

        public void Init(float x0, float x1, float t0, float t1)
        {
            c0 = x0;
            c1 = t0;
            c2 = -3 * x0 + 3 * x1 - 2 * t0 - t1;
            c3 = 2 * x0 - 2 * x1 + t0 + t1;
        }

        public float Evaluate(float t)
        {
            float tsquared = t * t;
            float tcubed = tsquared * t;
            return c0 + c1 * t + c2 * tsquared + c3 * tcubed;
        }

        public float Derivative(float t)
        {
            float tsquared = t * t;
            return (c1 + 2 * c2 * t + 3 * c3 * tsquared);
            
        }
    }

}
