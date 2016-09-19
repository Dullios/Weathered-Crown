using UnityEngine;
using System.Collections;
using BehaviourMachine;

public class FrictionAffector : PlayerAffector
{
    public float groundDrag = -1, airDrag = -1, maxMoveSpeed = -1, sprintMult = -1;
}
