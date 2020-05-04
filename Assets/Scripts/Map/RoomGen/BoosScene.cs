using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoosScene : MonoBehaviour
{
    public GameObject[] ColorTile;
    public GameObject Platform;
    public GameObject player;
    public int Bosses;
    public GameObject KingMeowster;
    public GameObject SpawnPoint;
    private List<GameObject> walls = new List<GameObject>();
    private Node[,] Nodes;
    private List<Edge> Edges;
    private Vector2 bottomLeft;
    private Vector2 topRight;
    private Vector2 mapSize;
    public bool DrawNodes = false;
    public bool DrawEdges = false;
    void Start()
    {
        
        GameObject CurrentTile = ColorTile[Random.Range(0,5)];
        player.transform.position = new Vector2(2, 2);

        for (int i = 0; i <= 11; i++)
        {
            walls.Add(Instantiate(CurrentTile, new Vector2(20, i), Quaternion.identity));
        }
        for (float i = 0; i <=11; i++)
        {
            walls.Add(Instantiate(CurrentTile, new Vector2(0, i), Quaternion.identity));
        }
        for (int i = 0; i <= 20; i++)
        {
            walls.Add(Instantiate(CurrentTile, new Vector2(i, 11), Quaternion.identity));
        }
        for (int i = 0; i <= 20; i++)
        {
            walls.Add(Instantiate(CurrentTile, new Vector2(i, 0), Quaternion.identity));
        }
        for(int i=0;i<=3;i++){
            // walls.Add(Instantiate(Platform, new Vector2(i+9, 5), Quaternion.identity));
            walls.Add(Instantiate(Platform, new Vector2(i+13,3), Quaternion.identity));
            walls.Add(Instantiate(Platform, new Vector2(i+4, 3), Quaternion.identity));
        }
        walls.Add(Instantiate(CurrentTile, new Vector2(10, 1), Quaternion.identity));
        walls.Add(Instantiate(CurrentTile, new Vector2(10, 2), Quaternion.identity));
        walls.Add(Instantiate(Platform, new Vector2(1, 2), Quaternion.identity));
        walls.Add(Instantiate(Platform, new Vector2(19, 2), Quaternion.identity));
        
        // Generate bool array of the map
        bottomLeft = new Vector2(walls.Select(w => w.transform.position.x).Min(), walls.Select(w => w.transform.position.y).Min());
        topRight = new Vector2(walls.Select(w => w.transform.position.x).Max(), walls.Select(w => w.transform.position.y).Max());
        mapSize = new Vector2(topRight.x - bottomLeft.x + 1, topRight.y - bottomLeft.y + 1);

        bool[,] map = new bool[(int)mapSize.x, (int)mapSize.y];
        foreach (GameObject w in walls)
        {
            Vector2 pos = (Vector2)w.transform.position - bottomLeft;
            map[(int)pos.x, (int)pos.y] = true;
        }

        // Generate NavMesh
        NavMeshGenerator.Offset = bottomLeft;
        Nodes = NavMeshGenerator.GenerateNavMesh(map);
        GameController.Nodes = Nodes;

        Vector3 spawn = SpawnPoint.transform.position;
        for(int i=0;i<Bosses;i++)
        {
            var boss = Instantiate(KingMeowster, spawn, Quaternion.identity);
            boss.GetComponent<KingMeowster>().Target = player;
        }
        GameController.AliveBosses = Bosses;
    }

    private void OnDrawGizmos()
    {
        if (DrawNodes)
        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                bool draw = true;
                switch (GameController.Nodes[x, y].type)
                {
                    case RoomTileType.Platform:
                        Gizmos.color = Color.black;
                        break;
                    case RoomTileType.PlatformSingle:
                        Gizmos.color = Color.yellow;
                        break;
                    case RoomTileType.PlatformLeft:
                    case RoomTileType.PlatformRight:
                        Gizmos.color = Color.magenta;
                        break;
                    default:
                        draw = false;
                        break;
                }
                if (draw)
                    Gizmos.DrawSphere(new Vector3(x + bottomLeft.x, y + bottomLeft.y - 0.5f, 0), 0.25f);
            }
        }

        if (DrawEdges)
        foreach (Node n in GameController.Nodes)
        {
            foreach (Edge edge in n.edges)
            {
                bool draw = true;
                switch(edge.type)
                {
                    case LinkType.Straight:
                    case LinkType.Fall:
                        Gizmos.color = Color.blue;
                        break;
                    case LinkType.Jump:
                        Gizmos.color = Color.green;
                        break;
                    default:
                        draw = false;
                        break;
                }

                if (draw)
                    edge.DrawPath(bottomLeft + Vector2.down * 0.5f);
            }
        }
    }
}
