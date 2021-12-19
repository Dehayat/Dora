using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerAction : MonoBehaviour
{
    [SerializeField]
    private UnityEvent action;
    [SerializeField]
    private bool trigerOnce = true;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody != null && collision.attachedRigidbody.CompareTag("Player"))
        {
            action?.Invoke();
            if (trigerOnce)
            {
                Destroy(gameObject);
            }
        }
    }

}
