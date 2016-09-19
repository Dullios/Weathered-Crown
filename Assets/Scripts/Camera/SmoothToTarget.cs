using UnityEngine;
using System.Collections;
using BehaviourMachine;

// Soft character bounds
// If the player leaves the defined bounds, the camera will smoothly move to keep the player within view
public class SmoothToTarget : CameraBehaviourBase
{
    public float topMargin, bottomMargin;
    public float leftMargin, rightMargin;

    public float leadingSpace;
    private bool facingRight;

    public float xSpeed, ySpeed;

    public float xVelocity, yVelocity;

    public override void Evaluate()
    {
        Rect bounds = new Rect(
            camera.transform.position.x,
            camera.transform.position.y,
            0,
            0);

        bounds.xMin -= leftMargin;
        bounds.xMax += rightMargin;
        bounds.yMin += topMargin;
        bounds.yMax -= bottomMargin;

        Vector2 targetPos = target.transform.position;
        Vector3 myPos = transform.position;

        facingRight = target.GetComponent<Blackboard>().GetBoolVar("FacingRight");
        
        //if(facingRight)
        //{
        //    bounds.xMin += leadingSpace;
        //    bounds.xMax += leadingSpace;
        //}
        //else
        //{
        //    bounds.xMin -= leadingSpace;
        //    bounds.xMax -= leadingSpace;
        //}

        if (targetPos.x < bounds.xMin || targetPos.x > bounds.xMax)
        {
            if (Mathf.Sign(targetPos.x - myPos.x) != Mathf.Sign(xVelocity))
                xVelocity = 0;

            targetPos.x = targetPos.x < bounds.xMin ? bounds.xMin : bounds.xMax;
            myPos.x = Mathf.SmoothDamp(myPos.x, targetPos.x, ref xVelocity, xSpeed);
        }

        if (targetPos.y > bounds.yMin || targetPos.y < bounds.yMax)
        {
            if (Mathf.Sign(targetPos.y - myPos.y) != Mathf.Sign(yVelocity))
                yVelocity = 0;

            myPos.y = Mathf.SmoothDamp(myPos.y, targetPos.y, ref yVelocity, ySpeed);
        }

        transform.transform.position = myPos;
    }

    void OnDrawGizmosSelected()
    {
        Rect bounds = new Rect(
            camera.transform.position.x,
            camera.transform.position.y,
            0,
            0);

        bounds.xMin -= leftMargin;
        bounds.xMax += rightMargin;
        bounds.yMin += topMargin;
        bounds.yMax -= bottomMargin;

        Gizmos.color = Color.magenta;

        Vector2 pos = transform.position;
        Vector2 topRight = new Vector2(bounds.xMax, bounds.yMin);
        Vector2 botLeft = new Vector2(bounds.xMin, bounds.yMax);

        Gizmos.DrawLine(topRight, botLeft);
    }
}
