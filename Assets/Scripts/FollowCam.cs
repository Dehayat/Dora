using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{

    [SerializeField]
    private Transform target;
    [SerializeField, Range(0.0f, 1f)]
    private float smooth = 0.125f;
    [SerializeField]
    private float minSmoothLength = 1f;

    private float xOffset;

    private void Awake()
    {
        xOffset = (transform.position - target.position).x;
    }

    private void LateUpdate()
    {
        Vector3 position = transform.position;
        position.x = target.position.x + xOffset;
        if (Vector3.Distance(position, transform.position) > minSmoothLength)
        {
            transform.position = Vector3.Lerp(transform.position, position, smooth);
        }
        else
        {
            transform.position = position;
        }
    }
}
