using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CameraBehaviourComponent))]
public abstract class CameraBehaviourBase : MonoBehaviour {

    public int priority;

    Camera _camera;
    protected Camera camera
    {
        get
        {
            if (_camera == null)
                _camera = GetComponent<Camera>();

            return _camera;
        }
    }


    CameraBehaviourComponent _vars;
    protected GameObject target
    {
        get
        {
            if (_vars == null)
                _vars = GetComponent<CameraBehaviourComponent>();

            return _vars.targetObj;
        }
    }

    public abstract void Evaluate();
}
