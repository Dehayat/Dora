using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour
{
    [SerializeField]
    private int health;
    [SerializeField]
    private Sprite[] healthSprites;
    [SerializeField]
    private Collider2D[] healthColliders;

    private SpriteRenderer spriteRenderer;

    public void Damage(int damageAmount)
    {
        if (health <= 0)
        {
            return;
        }
        health -= damageAmount;
        if (health <= 0)
        {
            health = 0;
        }
        UpdateState();
    }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        UpdateState();
    }
#endif

    private void UpdateState()
    {
        if (healthSprites != null && health < healthSprites.Length && health >= 0)
        {
            spriteRenderer.sprite = healthSprites[health];
        }
        if (healthColliders != null)
        {
            for (int i = 0; i < healthColliders.Length; i++)
            {
                if (healthColliders[i] != null)
                {
                    if (i == health)
                    {
                        healthColliders[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        healthColliders[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
