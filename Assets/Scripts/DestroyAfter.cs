using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 10f;
    void Start()
    {
        StartCoroutine(DelayedDestroy());
    }
    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
