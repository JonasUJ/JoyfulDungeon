using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public GameObject HitNumber;
    public float MaxHealth;
    private float health;
    public float Health {
        get => health;
        set
        {
            float newHealth = Math.Min(MaxHealth, Math.Max(0f, value));
            if (newHealth == health)
                return;
            HealthChangedEventArgs e = new HealthChangedEventArgs
            {
                OldHealth = health,
                NewHealth = newHealth,
                HealthChange = newHealth - health,
                CausedDeath = newHealth == 0f
            };
            health = newHealth;
            OnHealthChanged(e);
        }
    }

    public float PercentHealth { get => health / MaxHealth; }
    private HealthChangedEventArgs prevDmgTaken;
    public delegate void HealthChangedEventHandler(object sender, HealthChangedEventArgs e);
    public event HealthChangedEventHandler HealthChanged;

    protected virtual void OnHealthChanged(HealthChangedEventArgs e)
    {
        HealthChangedEventHandler handler = HealthChanged;
        prevDmgTaken = e;
        handler?.Invoke(this, e);
    }

    void Awake()
    {
        health = MaxHealth;
    }

    public void DealDamageTo(float damage)
    {
        Health = Health - damage;
    }

    public void Heal(float amount)
    {
        Health = Health + amount;
    }

    public void SpawnHitNumber()
    {
        if (prevDmgTaken == null)
            return;

        HitNumber hit = Instantiate(HitNumber, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity, GameController.GUICanvas.transform).GetComponent<HitNumber>();
        hit.Number = -prevDmgTaken.HealthChange;
        hit.Color = prevDmgTaken.HealthChange <= 0 ? Color.white : Color.green;
    }
}

public class HealthChangedEventArgs : EventArgs
{
    public float OldHealth;
    public float NewHealth;
    public float HealthChange;
    public bool CausedDeath;
}