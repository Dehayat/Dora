using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    private Move mover;

    private void Awake()
    {
        mover = GetComponent<Move>();
    }

    public void FlipDirection()
    {
        mover.Flip();
    }
}
