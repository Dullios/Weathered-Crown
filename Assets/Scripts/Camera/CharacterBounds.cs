using UnityEngine;
using System.Collections;

// Enforces hard character bounds
// If the character leaves the bounds defined here, the camera will immediately move to keep the player in view
public class CharacterBounds : CameraBehaviourBase {

    public float topMargin, bottomMargin;
    public float leftMargin, rightMargin;

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
        Vector2 shift = Vector2.zero;

        if (targetPos.x < bounds.xMin)
            shift.x = targetPos.x - bounds.xMin;
        else if (targetPos.x > bounds.xMax)
            shift.x = targetPos.x - bounds.xMax;

        if (targetPos.y > bounds.yMin)
            shift.y = targetPos.y - bounds.yMin;
        else if (targetPos.y < bounds.yMax)
            shift.y = targetPos.y - bounds.yMax;

        transform.Translate(shift);
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

        Gizmos.color = Color.red;

        Vector2 pos = transform.position;
        Vector2 topLeft = new Vector2(bounds.xMin, bounds.yMin);
        Vector2 botRight = new Vector2(bounds.xMax, bounds.yMax); 

        Gizmos.DrawLine(topLeft, botRight);
    }
}
