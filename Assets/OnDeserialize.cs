using UnityEngine;
using System.Collections;

public class OnDeserialize : MonoBehaviour {

    public GameObject targetAnimator;

    void OnDeserialized()
    {
        var anim = GetComponent<Animator>();
        var animHolder = targetAnimator.GetComponent<Animator>();
        if (anim.runtimeAnimatorController == null && targetAnimator != null)
        {
            anim.runtimeAnimatorController = animHolder.runtimeAnimatorController;
        }
    }

}
