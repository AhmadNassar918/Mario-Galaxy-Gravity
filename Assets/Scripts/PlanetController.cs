using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().currentPlanet = transform;
        }
        if (other.CompareTag("PlayerCollider"))
        {
            other.GetComponentInParent<PlayerController>().currentPlanet = transform;
        }
    }
}
