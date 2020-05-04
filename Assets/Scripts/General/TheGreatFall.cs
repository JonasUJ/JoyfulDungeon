using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheGreatFall : MonoBehaviour
{
    public GameObject[] ColorTiles;

    void Start()
    {
        GameObject CurrentTile= ColorTiles[Random.Range(0,5)];
        for(int i=0;i<=92;i++){
            Instantiate(CurrentTile, new Vector2(0, i+8), Quaternion.identity);
            Instantiate(CurrentTile, new Vector2(8, i+8), Quaternion.identity);
        }
        for(int i=0;i<=24;i++){
            Instantiate(CurrentTile, new Vector2(i-8, 0), Quaternion.identity);
        }
        for(int i=0;i<=8;i++){
            Instantiate(CurrentTile, new Vector2(i-8, 8), Quaternion.identity);
            Instantiate(CurrentTile, new Vector2(i+8, 8), Quaternion.identity);
        }
        for(int i=0;i<=8;i++){
            Instantiate(CurrentTile, new Vector2(-8, i), Quaternion.identity);
            Instantiate(CurrentTile, new Vector2(16, i), Quaternion.identity);
        }
        for(int i=0;i<=24;i++){
            Instantiate(CurrentTile, new Vector2(i-8, 0), Quaternion.identity);
        }
        for(int i=0;i<=24;i++){
            Instantiate(CurrentTile, new Vector2(i-8, 0), Quaternion.identity);
        }
    }
}
