using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    public float smoothSpeed = 0.125f;

    void FixedUpdate(){
        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}
