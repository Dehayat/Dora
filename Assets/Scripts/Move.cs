using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField]
    private Vector2 velocity;

    void Update()
    {
        transform.Translate(velocity * Time.deltaTime);
    }
}
