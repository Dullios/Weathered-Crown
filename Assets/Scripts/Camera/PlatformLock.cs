using UnityEngine;
using System.Collections;

// Aligns the camera vertically whenever the player lands on a platform
public class PlatformLock : CameraBehaviourBase {

    public float offsetFromPlayer;

    IEnumerator platformLockMethod;

	// Use this for initialization
	void Start () {
        platformLockMethod = PlatformLockMethod();
	}

    public override void Evaluate()
    {
        if (!platformLockMethod.MoveNext())
            platformLockMethod.Reset();
    }

    IEnumerator PlatformLockMethod()
    {
        Controller2D targetController = target.GetComponent<Controller2D>();

        while (true)
        {
            // target has landed on a platform
            yield return null;

            // Smooth to platform
            float x = transform.position.y - offsetFromPlayer;
            float shiftBy = target.transform.position.y - x;
            float shiftFrom = transform.position.y;
            float shiftTo = shiftFrom + shiftBy;

            for (float i = 0; i <= 1; i += Time.deltaTime * 6)
            {
                Vector3 myPos = transform.position;
                myPos.y = Mathf.Lerp(shiftFrom, shiftTo, i);
                transform.position = myPos;

                yield return null;
            }
            {
                Vector3 myPos = transform.position;
                myPos.y = Mathf.Lerp(shiftFrom, shiftTo, 1);
                transform.position = myPos;
            }

            while (targetController.collisions.below)
            {
                yield return null;
            }

            while (!targetController.collisions.below)
            {
                yield return null;
            }
        }
    }

}
