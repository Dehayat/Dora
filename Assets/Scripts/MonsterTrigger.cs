using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody != null && collision.attachedRigidbody.CompareTag("Monster"))
        {
            collision.attachedRigidbody.GetComponent<Monster>().FlipDirection();
        }
    }
}
