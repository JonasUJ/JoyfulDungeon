using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestRoom : MonoBehaviour
{
    public GameObject[] Colortiles;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        GameObject CurrentTile = Colortiles[Random.Range(0,5)];
        player.transform.position = new Vector2(2, 1);
        for(int i=0;i<=10;i++){
            Instantiate(CurrentTile, new Vector2(0, i), Quaternion.identity);
        }
        for(int i=0;i<=8;i++){
            
            Instantiate(CurrentTile, new Vector2(i, 0), Quaternion.identity);
        }
        for(int i=0;i<=3;i++){
            Instantiate(CurrentTile, new Vector2(i+8, 1), Quaternion.identity);
        }
        for(int i=0;i<=10;i++){
            Instantiate(CurrentTile, new Vector2(i, 10), Quaternion.identity);
        }
        
        for(int i=0;i<=3;i++){
            Instantiate(CurrentTile, new Vector2(i+11, 2), Quaternion.identity);
        }
        for(int i=0;i<=7;i++){
            Instantiate(CurrentTile, new Vector2(i+14, 3), Quaternion.identity);
        }
        for(int i=0;i<=8;i++){
            
            Instantiate(CurrentTile, new Vector2(i, 0), Quaternion.identity);
        }
        for(int i=0;i<=21;i++){
            
            Instantiate(CurrentTile, new Vector2(i, 10), Quaternion.identity);
        }
        for(int i=0;i<=6;i++){
            
            Instantiate(CurrentTile, new Vector2(21, 4+i), Quaternion.identity);
        }

    }

}
