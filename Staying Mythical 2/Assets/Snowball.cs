using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StayingMythical.Reference;

public class Snowball : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(GameResources.SnowParticles, transform.position, Quaternion.identity, null);
        GameObjects.player.DestroyAndRemoveFromCollisions(gameObject);

    }
}
