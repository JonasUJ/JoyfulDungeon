using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoverable : MonoBehaviour
{
    [Range(0.5f, 2f)]
    public float SizeIncrease = 1.1f;

    void OnMouseEnter() {
        transform.localScale = Vector3.one * SizeIncrease;
    }

    void OnMouseExit() {
        transform.localScale = Vector3.one;
    }
}
