using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public LayerMask Walls;
    public LayerMask Enemies;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (Walls == (Walls | (1 << other.gameObject.layer)))
        {
            Destroy(gameObject);
        }
        else if (Enemies == (Enemies | (1 << other.gameObject.layer)))
        {
            other.gameObject.GetComponent<Character>().Hit(new HitInfo(transform.position, 5, 3f));
            Destroy(gameObject);
        }
    }
}
