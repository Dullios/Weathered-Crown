using UnityEngine;
using System.Collections;

public class WaterDetector : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D Hit)
    {
        if(Hit.GetComponent<Rigidbody2D>() == null)
        {
            if (Hit.GetComponent<BehaviourMachine.Blackboard>() != null)
            {
                BehaviourMachine.Blackboard bmb = Hit.gameObject.GetComponent<BehaviourMachine.Blackboard>();
                Vector3 vel = bmb.GetVector3Var("Velocity ");
                transform.parent.GetComponent<LakeManager>().Splash(transform.position.x, vel.y / 40.0f);
            }
        }
        else
        {
            if (Hit.name != "Trigger")
            {
                transform.parent.GetComponent<LakeManager>().Splash(transform.position.x, Hit.GetComponent<Rigidbody2D>().velocity.y * Hit.GetComponent<Rigidbody2D>().mass / 40f);
            }
        }
    }
}
