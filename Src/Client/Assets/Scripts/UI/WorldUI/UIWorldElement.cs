using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElement : MonoBehaviour
{
    [HideInInspector] public Transform owner;
    [SerializeField] private float heightOffset = 2f;

    private void Update()
    {
        if (Camera.main != null)
            transform.forward = Camera.main.transform.forward;
        if(owner != null)
            transform.position = owner.position + heightOffset * Vector3.up;
    }
}
