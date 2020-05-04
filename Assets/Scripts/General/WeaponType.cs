using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponType : MonoBehaviour
{
    public bool isSword = true;
    bool entered = false;

    void Update()
    {
        if (entered && Input.GetButtonDown("Interact"))
        {
            if (isSword == true)
                GameController.playerData.weapon = WeaponTypes.Sword;
            else
                GameController.playerData.weapon = WeaponTypes.Bow;
            GameController.LoadScene("MapScene");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        entered = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        entered = false;
    }
}
