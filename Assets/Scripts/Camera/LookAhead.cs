using UnityEngine;
using System.Collections;

public class LookAhead : CameraBehaviourBase {

    public float centerOffset = 3;
    public float timeToAlign = 1;

    float currentOffset = 0;
    float velocity;

    public override void Evaluate()
    {
        var targetBlackboard = target.GetComponent<BehaviourMachine.Blackboard>();
        var sm = target.GetComponent<BehaviourMachine.StateMachine>();

        if (!sm || !targetBlackboard)
            return;

        if (sm.root.GetEnabledStateName() == "Movement")
        {
            float horizontalInput = targetBlackboard.GetFloatVar("HorizontalInput");
            if (horizontalInput != 0)
            {
                // We now know that the character is intentionally moving in a direction
                float desiredOffset = centerOffset * Mathf.Sign(horizontalInput);

                if (Mathf.Abs(currentOffset) != centerOffset)
                    currentOffset = Mathf.SmoothDamp(currentOffset, desiredOffset, ref velocity, timeToAlign);
            }
        }

        Vector3 pos = transform.position;
        pos.x = target.transform.position.x + currentOffset;
        transform.position = pos;

    }
}
