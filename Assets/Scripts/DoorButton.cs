using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class DoorButton : MonoBehaviour {

    public enum buttonType
    {
        Once,
        Toggle,
        Hold
    }

    public CascadeDoor target;

    //public float timeToMove;

    //public Vector2 move;
    //private Vector2 startPos;
    //private Vector2 endPos
    //{
    //    get
    //    {
    //        return startPos + move;
    //    }
    //}

    public buttonType type;

    public LayerMask activationLayer;

    bool activated;
    //Vector2 velocity;

    void Start()
    {
        GetComponent<Rigidbody2D>().isKinematic = true;
        //startPos = target.transform.position;
    }

    //void FixedUpdate()
    //{
    //    Vector2 destination = activated ? endPos : startPos;

    //    Vector2 targetPos = target.transform.position;

    //    targetPos = Vector2.SmoothDamp(targetPos, destination, ref velocity, timeToMove);

    //    target.transform.position = targetPos;
    //}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (System.Convert.ToBoolean(activationLayer.value & 1 << collider.gameObject.layer))
        {
            if (type == buttonType.Once || type == buttonType.Hold)
            {
                activated = true;
                target.Open();
            }
            else if (type == buttonType.Toggle)
            {
                activated = !activated;
                if (activated)
                    target.Open();
                else
                    target.Close();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (System.Convert.ToBoolean(activationLayer.value & 1 << collider.gameObject.layer))
        {
            if (type == buttonType.Hold)
            {
                activated = false;
                target.Close();
            }
        }
    }
}
