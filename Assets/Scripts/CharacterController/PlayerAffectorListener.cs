using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviourMachine;

public class PlayerAffectorListener : MonoBehaviour {

    // LayerMask
    public LayerMask layers;

    // Defaults
    public float moveSpeedDefault, sprintMultDefault;
    public float jumpHeightDefault, minJumpHeightDefault, timeToApexDefault;
    public float floatingGravModDefault, floatingMaxVelocityDefault;
    public float groundDragDefault, airDragDefault;

    // Values
    [HideInInspector]
    public float moveSpeed, sprintMult;

    [HideInInspector]
    public float jumpHeight, minJumpHeight, timeToApex;

    [HideInInspector]
    public float floatingGravMod, floatingMaxVelocity;
    
    [HideInInspector]
    public float groundDrag, airDrag;

    [HideInInspector]
    public Vector3 externalForce;

    void OnEnable()
    {
        Reset();
    }

    void Reset()
    {
        moveSpeed = moveSpeedDefault;
        sprintMult = sprintMultDefault;
        jumpHeight = jumpHeightDefault;
        minJumpHeight = minJumpHeightDefault;
        timeToApex = timeToApexDefault;
        floatingGravMod = floatingGravModDefault;
        floatingMaxVelocity = floatingMaxVelocityDefault;
        groundDrag = groundDragDefault;
        airDrag = airDragDefault;
        externalForce = Vector3.zero;
    }

    void Update()
    {
        layers = WeatherManager.getWeatherLayer(WeatherManager.Instance.checkCurrentWeather(gameObject)) | 1 << LayerMask.NameToLayer("Default");
        //LayerMask.
        //WeatherManager.WeatherType w = (WeatherManager.WeatherType)WeatherManager.Instance.checkCurrentWeather(gameObject);
        //string weatherString = "";
        //switch (w)
        //{
        //    case WeatherManager.WeatherType.sun:
        //        weatherString = "Sun";
        //        break;
        //    case WeatherManager.WeatherType.snow:
        //        weatherString = "Snow";
        //        break;
        //    case WeatherManager.WeatherType.rain:
        //        weatherString = "Rain";
        //        break;
        //}

        //layers = LayerMask.GetMask("Default", weatherString);

        Vector3 pos = transform.position;
        Vector3 inc = Vector2.one * 0.01f;
        Collider2D collider = GetComponent<Collider2D>();
        Vector2 topRight = collider.bounds.center - collider.bounds.extents;//pos - GetComponent<Collider2D>().bounds.extents - inc;
        Vector2 botLeft = collider.bounds.center + collider.bounds.extents;//pos + GetComponent<Collider2D>().bounds.extents + inc;

        Collider2D[] colliders = Physics2D.OverlapAreaAll(topRight, botLeft, layers);

        // Forces of highest priority will be summed
        Force forces = new Force();
        forces.priority = -1;

        // Highest priority will be chosen
        FrictionAffector friction = null;

        GravityAffector gravity = null;

        foreach (Collider2D c in colliders)
        {
            ForceAffector foa = c.GetComponent<ForceAffector>();
            if (foa != null)
            {
                if (forces.priority == foa.priority)
                {
                    forces.force += foa.force;
                }
                else if (foa.priority > forces.priority)
                {
                    forces.priority = foa.priority;
                    forces.force = foa.force;
                }
            }

            FrictionAffector fra = c.GetComponent<FrictionAffector>();
            if (fra != null)
            {
                if (friction == null || fra.priority > friction.priority)
                {
                    friction = fra;
                }
            }

            GravityAffector ga = c.GetComponent<GravityAffector>();
            if (ga != null)
            {
                if (gravity == null || ga.priority > gravity.priority)
                {
                    gravity = ga;
                }
            }
        }

        // Force
        externalForce = Vector3.zero;
        if (forces.priority > -1)
            externalForce = forces.force;

        // Friction
        if (GetComponent<Controller2D>().collisions.below)
        {
            moveSpeed = moveSpeedDefault;
            sprintMult = sprintMultDefault;
            groundDrag = groundDragDefault;
            airDrag = airDragDefault;

            if (friction != null)
            {
                if (friction.groundDrag >= 0)
                    groundDrag = friction.groundDrag;

                if (friction.airDrag >= 0)
                    airDrag = friction.airDrag;

                if (friction.maxMoveSpeed >= 0)
                    moveSpeed = friction.maxMoveSpeed;

                if (friction.sprintMult >= 1)
                    sprintMult = friction.sprintMult;

            }
        }

        // Air Drag
        //if (airDragAffect != null && airDragAffect.drag >= 0)
        //    airDrag = airDragAffect.drag;

        // Gravity
        jumpHeight = jumpHeightDefault;
        timeToApex = timeToApexDefault;
        if (gravity != null)
        {
            jumpHeight = gravity.jumpHeight;
            timeToApex = gravity.timeToApex;
        }
            
    }

    struct Force
    {
        public int priority;
        public Vector2 force;
    }

}
