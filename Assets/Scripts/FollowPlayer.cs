using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform playerTransform;
    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, playerTransform.position, .25f);
        transform.rotation = Quaternion.Lerp(transform.rotation, playerTransform.rotation, .25f);
    }
}
