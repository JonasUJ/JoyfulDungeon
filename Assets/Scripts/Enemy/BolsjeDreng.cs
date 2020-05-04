using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BolsjeDreng : PathingCharacter
{
    public GameObject deadObject;
    public float attackCooldown = 1f;
    public float attackRange = 0.5f;
    private float lastAttack = 0f; 
    private Animator _animator;
    private Vector3 curPos;
    private Vector3 lastPos;
    private SpriteRenderer spriteRenderer;
    private GameObject player;

    void Start()
    {
        _animator = GetComponent<Animator>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        player = GameObject.Find("player");
        InitCharacter();
    }

    void Update()
    {
        UpdatePathing();
        if (transform.position.y < -10)
        {
            DealDamageTo(MaxHealth);
        }
        if (Vector2.Distance(Target.transform.position, transform.position) < attackRange && Time.time > lastAttack + attackCooldown)
        {
            Target.GetComponent<PlayerCharacter>().Hit(new HitInfo(transform.position, 1, 6f));
            lastAttack = Time.time;
        }
        curPos = transform.position;
        if (curPos == lastPos)
        {
            _animator.SetTrigger("Idle");
        }
        else
        {
            _animator.SetTrigger("Walk");
        }
        lastPos = curPos;
        this.spriteRenderer.flipX = player.transform.position.x > this.transform.position.x;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerSword")
        {
            Hit(new HitInfo(other.transform.position - Vector3.up * 0.2f, 5, 4f));
            ForcePathingNextUpdate();
        }
    }

    public override void TookDamage(object sender, HealthChangedEventArgs e)
    {
        if (e.CausedDeath)
        {
            var dead = Instantiate(deadObject);
            dead.transform.position = transform.position;
            dead.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 8);
            Destroy(dead, 5f);

            GameController.AliveEnemies -= 1;
            Destroy(healthBarReference.gameObject);
            Destroy(gameObject);
        }
    }
}
