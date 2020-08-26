using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveParticle : MonoBehaviour
{
    public LayerMask groundLayers;
    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;


    //u need to attach this script in a particle system , 
    //make sure to set the triggers module on the Particle System
    //to on and also turn on thr collision module, also the label 
    //Send Collision messages needs to be enabled.
    //
    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);
        int i = 0;

        while (i < numCollisionEvents)
        {
            if (other.tag == "Fire")
            {
                //do smth
            }

            i++;
        }
    }
}
