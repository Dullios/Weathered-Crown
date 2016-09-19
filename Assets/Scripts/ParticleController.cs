using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Tiled2Unity
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleController : Singleton<ParticleController>
    {
        [HideInInspector]
        public ParticleSystem globalParticleSystem;
        public ParticleSystem instanceParticleSystem;
        private ParticleSystem.Particle[] globalParticles;
        private ParticleSystem.Particle[] instanceParticles;

        List<ParticleSystem.Particle> globalParticleList = new List<ParticleSystem.Particle>();
        List<ParticleSystem.Particle> instanceParticleList = new List<ParticleSystem.Particle>();

        public PlayerAffectorListener pal
        {
            get
            {
                return GameObject.FindObjectOfType<PlayerAffectorListener>();
            }
        }

        private WeatherManager.WeatherType type;
        private WeatherManager.WeatherType instType;

        //public int x, width;
        private Rect r;
        public bool instanceIsRunning = false;

        public float dampTime;
        private float smoothVel;

        // Gravity variables
        private float gravity;
        private float jumpHeight;
        private float timeToJumpApex;
        public float leaveFall;
        public float dandyFall;
        public float rainFall;
        public float snowFall;

        public Material leaveMat, dandyMat, rainMan, snowMat;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void LateUpdate()
        {
            // Check if the global weather has changed and change the variable accordingly
            type = type != WeatherManager.Instance.type ? WeatherManager.Instance.type : type;

            globalParticleList.Clear();
            instanceParticleList.Clear();
            
            InitializeIfNeeded(ref globalParticleSystem, ref globalParticles);
            InitializeIfNeeded(ref instanceParticleSystem, ref instanceParticles);

            int globalNumParticles = globalParticleSystem.GetParticles(globalParticles);
            int instanceNumParticles = instanceParticleSystem.GetParticles(instanceParticles);

            globalParticleSystem.GetComponent<Renderer>().material = getParticleTexture(type);
            instanceParticleSystem.GetComponent<Renderer>().material = getParticleTexture(instType);

            for (int i = 0; i < globalNumParticles; i++)
            {
                checkParticle(ref globalParticles[i]);
            }
            for (int i = 0; i < instanceNumParticles; i++)
            {
                checkParticle(ref instanceParticles[i]);
            }

            globalParticleSystem.SetParticles(globalParticleList.ToArray(), globalParticleList.Count);
            instanceParticleSystem.SetParticles(instanceParticleList.ToArray(), instanceParticleList.Count);
        }

        // Check if a particle exists inside the area of the instance weather
        private void checkParticle(ref ParticleSystem.Particle p)
        {
            bool inForceAffector = false;
            bool inGravityAffector = false;

            bool addToGlobal; // else add to instance

            // Check each global particle if they are in the instance weather zone
            if (instanceIsRunning && p.position.x > r.x && p.position.x < r.x + r.width)
            {
                handleForces(instType, ref p, out inForceAffector, out inGravityAffector);

                //instanceParticleList.Add(p);
                addToGlobal = false;
            }
            else
            {
                handleForces(type, ref p, out inForceAffector, out inGravityAffector);

                //globalParticleList.Add(p);
                addToGlobal = true;
            }

            Vector3 pos = p.position;
            Vector3 vel = p.velocity;

            if (!inForceAffector)
            {
                vel.x = Mathf.SmoothDamp(vel.x, 0, ref smoothVel, dampTime);
                p.velocity = vel;
            }

            if (addToGlobal)
                globalParticleList.Add(p);
            else
                instanceParticleList.Add(p);
        }

        // Check if a particle is inside any collider on a weather layer that's currently in affect
        private void handleForces(WeatherManager.WeatherType t, ref ParticleSystem.Particle p, out bool inForceAccector, out bool inGravityAffector)
        {
            inForceAccector = false;
            inGravityAffector = false;

            Force externalForce = new Force();
            externalForce.priority = -1;

            Collider2D[] col = Physics2D.OverlapPointAll(p.position, WeatherManager.getWeatherLayer(t));

            foreach (Collider2D c in col)
            {
                ForceAffector foa = c.GetComponent<ForceAffector>();

                if (foa != null)
                {
                    inForceAccector = true;
                    
                    if (externalForce.priority == foa.priority)
                    {
                        externalForce.force += foa.force;
                        p.velocity = externalForce.force;
                    }
                    else if (foa.priority > externalForce.priority)
                    {
                        externalForce.priority = foa.priority;
                        externalForce.force = foa.force;
                    }
                }

                GravityAffector ga = c.GetComponent<GravityAffector>();

                if(ga != null)
                {
                    inGravityAffector = true;

                    jumpHeight = ga.jumpHeight;
                    timeToJumpApex = ga.timeToApex;
                }
                else
                {
                    jumpHeight = pal.jumpHeightDefault;
                    timeToJumpApex = pal.timeToApexDefault;
                }
            }

            if(jumpHeight == 0 && timeToJumpApex == 0)
            {
                jumpHeight = pal.jumpHeightDefault;
                timeToJumpApex = pal.timeToApexDefault;
            }

            float gravityMod = 1;

            switch(t)
            {
                case WeatherManager.WeatherType.sun:
                    gravityMod = dandyFall;
                    break;
                case WeatherManager.WeatherType.rain:
                    gravityMod = rainFall;
                    break;
                case WeatherManager.WeatherType.snow:
                    gravityMod = snowFall;
                    break;
            }

            Vector3 velocity = p.velocity;

            gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2) * gravityMod;

            velocity.x = externalForce.force.x;

            // Reduce or increase gravity depending on the magnitude and direction of the external force
            float forceDiff = gravity + externalForce.force.y;
            float m = 0.05f;
            float forceMod = forceDiff > 0 ?
                Mathf.Pow(2, m * -forceDiff) :
                -Mathf.Pow(2, m * forceDiff) + 2;
            velocity.y = gravity * forceMod;

            p.velocity = velocity;
        }

        // Called from the WeatherManager to set the scale of the instance weather
        public void setInstanceZone(Rect _r, bool isRunning, int typeNum)
        {
            //x = _x;
            //width = _width;
            r = _r;
            instanceIsRunning = isRunning;
            instType = (WeatherManager.WeatherType)typeNum;
        }

        // Get ParticleSystem Compoment and an array of all particles
        void InitializeIfNeeded(ref ParticleSystem ps, ref ParticleSystem.Particle[] psArray)
        {
            if (ps == null)
                ps = GetComponent<ParticleSystem>();

            if (psArray == null || psArray.Length < ps.maxParticles)
                psArray = new ParticleSystem.Particle[ps.maxParticles];
        }

        // Return color based on current weather type
        private Material getParticleTexture(WeatherManager.WeatherType t)
        {
            Material m = null;

            switch (t)
            {
                case WeatherManager.WeatherType.sun:
                    {
                        m = dandyMat;
                        break;
                    }
                case WeatherManager.WeatherType.rain:
                    {
                        m = rainMan;
                        break;
                    }
                case WeatherManager.WeatherType.snow:
                    {
                        m = snowMat;
                        break;
                    }
            }

            return m;
        }

        struct Force
        {
            public int priority;
            public Vector2 force;
        }
    }
}