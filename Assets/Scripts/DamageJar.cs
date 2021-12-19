using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageJar : MonoBehaviour
{

    [SerializeField]
    private int damageColumn = 1;
    [SerializeField]
    private bool destroyOnHitColumn = true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.attachedRigidbody.GetComponent<Column>() != null)
        {
            collision.collider.attachedRigidbody.GetComponent<Column>().Damage(damageColumn);

            if (destroyOnHitColumn)
            {
                Destroy(gameObject);
            }
        }
    }
}
