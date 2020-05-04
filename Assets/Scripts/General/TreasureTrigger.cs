using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TreasureTrigger : MonoBehaviour
{

    public GameObject[] Upgrades;
    bool entered = false;
    void Update()
    {
        if (entered && Input.GetButtonDown("Interact"))
        {
            GameController.playerData.health = Mathf.Min(GameController.playerData.health + 2f, 6f);
            GameController.LoadScene("MapScene");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")){
            entered = true;
            }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        entered = false;
    }
}
