using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 10f;
    async void Start()
    {
        float destroyTime = Time.time + lifeTime;
        while (gameObject != null && Time.time <= destroyTime)
        {
            await Task.Yield();
        }
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
