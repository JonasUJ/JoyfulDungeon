using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    private void Start() {
        InitCharacter();
        Health = GameController.playerData.health;
    }

    private void Update() {
        if (transform.position.y < -10)
        {
            DealDamageTo(1);
            transform.position = GameController.RespawnPoint;
        }
    }

    public override void TookDamage(object sender, HealthChangedEventArgs e)
    {
        GameController.playerData.health = e.NewHealth;
        if (e.CausedDeath)
        {
            GameController.LoadScene("DeathMenu");
        }
    }
}
