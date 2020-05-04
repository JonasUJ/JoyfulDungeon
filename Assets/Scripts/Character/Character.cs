using System;
using UnityEngine;

public struct HitInfo
{
    public Vector3 source;
    public int damage;
    public float knockback;
    public HitInfo(Vector3 source, int damage, float knockback)
    {
        this.source = source;
        this.damage = damage;
        this.knockback = knockback;
    }
}

[RequireComponent(typeof(Rigidbody2D))]
public class Character : HealthManager
{
    public GameObject HealthBarVarient;
    // public bool HasLineOfSight;
    // public float GoldValue;
    // public GameObject GoldNumber;
    protected HealthBarBase healthBarReference;
    private float previousHit = 0f;
    private float invincibilityTime = 0.3f;
    protected CharacterMovement controller;
    public Vector3 Knockback;
    protected void InitCharacter()
    {
        controller = GetComponent<CharacterMovement>();
        healthBarReference = Instantiate(this.HealthBarVarient, GameController.GUICanvas.transform).GetComponent<HealthBarBase>();
        HealthChanged += TookDamage;
        healthBarReference.Origin = this;
        // GoldValue *= 1 + GameController.Instance.PlayerData.Stats.Money;
    }

    // private void SpawnGoldNumber()
    // {
    //     GoldNumber gold = Instantiate(GoldNumber, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity, GameController.GUICanvas.transform).GetComponent<GoldNumber>();
    //     gold.Number = GoldValue;
    //     GameController.Instance.AddGold(GoldValue);
    // }

    public virtual void TookDamage(object sender, HealthChangedEventArgs e)
    {
        if (e.CausedDeath)
        {
            // GameController.AddDeath();
            // SpawnGoldNumber();
            Destroy(healthBarReference.gameObject);
            Destroy(gameObject);
        }
    }

    public virtual void Hit(HitInfo hitInfo)
    {
        if (Time.time <= previousHit + invincibilityTime)
            return;

        previousHit = Time.time;
        Knockback = (transform.position - hitInfo.source).normalized * hitInfo.knockback;
        Knockback.y -= NavMeshGenerator.Gravity * Time.deltaTime;
        DealDamageTo(hitInfo.damage);
    }
}
