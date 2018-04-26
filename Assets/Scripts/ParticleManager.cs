using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour {

    public static ParticleManager instance = null;

    Transform particleLocation;
    public ParticleSystem rottenSystem;
    public ParticleSystem regularSystem;
    public ParticleSystem beeSystem;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        particleLocation = GetComponent<Transform>();
    }

    public void generateParticles(string particleType, Transform generatorLocation)
    {
        Debug.Log(generatorLocation);
        particleLocation.position = new Vector3(generatorLocation.position.x,generatorLocation.position.y,0);

        //Location.Translate(0, 0, -5);

        if (particleType == "rotten")
        {
            Instantiate<ParticleSystem>(rottenSystem, particleLocation);

        }
        else if(particleType == "regular")
        {
            Instantiate<ParticleSystem>(regularSystem, particleLocation);
        }
        else if(particleType == "bee")
        {
            Instantiate<ParticleSystem>(beeSystem, particleLocation);
        }
    }
}
