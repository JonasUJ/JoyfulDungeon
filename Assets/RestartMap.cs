using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartMap : MonoBehaviour
{
    void Awake()
    {
        MapGen.ResetMap();
        GameController.playerData.health = 6f;
    }
}
