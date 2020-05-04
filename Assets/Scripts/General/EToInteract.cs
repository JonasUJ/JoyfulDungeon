using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EToInteract : MonoBehaviour
{
    public GameObject Esprite;
    private GameObject E;
    private Vector3 pos;
    void Start()
    {
        pos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(E);
        E = (GameObject)Instantiate(Esprite, pos, transform.rotation);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        Destroy(E);
    }
}
