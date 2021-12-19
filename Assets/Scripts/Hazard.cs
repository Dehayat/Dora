using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField]
    private int damageAmount = 1;
    [SerializeField]
    private bool destroyOnDamage = false;
    [SerializeField]
    private GameObject owner;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody.CompareTag("Player"))
        {
            var player = collision.attachedRigidbody.GetComponent<PlayerController>();
            player.Damage(damageAmount);
            if (destroyOnDamage)
            {
                Destroy(owner);
            }
        }
    }
}
