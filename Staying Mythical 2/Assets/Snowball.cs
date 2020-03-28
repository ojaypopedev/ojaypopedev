using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mythical;

public class Snowball : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(GameObjectReference.SnowParticles, transform.position, Quaternion.identity, null);
        StayingMythical.player.DestroyAndRemoveFromCollisions(gameObject);

    }
}
