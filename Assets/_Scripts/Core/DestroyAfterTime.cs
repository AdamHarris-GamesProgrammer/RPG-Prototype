using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float lifetime = 15.0f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
