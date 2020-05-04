using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class RandomRoomGen : MonoBehaviour
{
    public GameObject player;
    public GameObject[] WallTiles;
    public int MinRoomSize = 10;
    public int MaxRoomSize = 20;
    public int MinSideSize = 8;
    public int MaxSideSize = 15;
    public float Straight = 0.7f;
    public float ONE = 0.125f;
    public int Enemies;
    public GameObject BolsjeDreng;
    public GameObject Platform;
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
        int size = Random.Range(MinRoomSize, MaxRoomSize);
        GameObject CurrentTile = WallTiles[Random.Range(0,5)];
        GameController.RespawnPoint = new Vector2(Mathf.Round((-1 * size) / 2) + 1, 3);
        player.transform.position = GameController.RespawnPoint;
        float tracker = 0f;
        int sizeSides = Random.Range(MinSideSize, MaxSideSize);

        // Generate floor
        for (float i = Mathf.Round((-1 * size) / 2); i <= Mathf.Round(size / 2); i++)
        {
            float Percentage = Random.value;
            if (tracker == -2f)
            {
                walls.Add(Instantiate(Platform, new Vector2(i - 1, 1), Quaternion.identity));
            }
            if (Percentage <= Straight)
            {
                walls.Add(Instantiate(CurrentTile, new Vector2(i, tracker), Quaternion.identity));
            }
            else if (Percentage <= Straight + ONE && Percentage > Straight && tracker <= 1)
            {
                tracker += 1f;
                walls.Add(Instantiate(CurrentTile, new Vector2(i, tracker), Quaternion.identity));
                walls.Add(Instantiate(CurrentTile, new Vector2(i, tracker - 1), Quaternion.identity));
            }
            else if (Percentage <= Straight + ONE + ONE && Percentage > Straight + ONE && tracker >= -1)
            {
                tracker -= 1f;
                walls.Add(Instantiate(CurrentTile, new Vector2(i, tracker), Quaternion.identity));
                walls.Add(Instantiate(CurrentTile, new Vector2(i - 1, tracker), Quaternion.identity));
            }
            else if (Percentage < 1 && Straight + ONE + ONE < Percentage && tracker >= 0)
            {
                tracker -= 2f;
                walls.Add(Instantiate(CurrentTile, new Vector2(i - 1, tracker + 1f), Quaternion.identity));
                walls.Add(Instantiate(CurrentTile, new Vector2(i - 1, tracker), Quaternion.identity));
                walls.Add(Instantiate(CurrentTile, new Vector2(i, tracker), Quaternion.identity));
            }
            else if (Percentage < 1 && Straight + ONE + ONE < Percentage && tracker < 0)
            {
                tracker += 2f;
                walls.Add(Instantiate(CurrentTile, new Vector2(i - 1, tracker - 1f), Quaternion.identity));
                walls.Add(Instantiate(CurrentTile, new Vector2(i - 1, tracker), Quaternion.identity));
                walls.Add(Instantiate(CurrentTile, new Vector2(i, tracker), Quaternion.identity));
            }
        }

        // Generate walls
        for (int i = 0; i <= sizeSides; i++)
        {
            walls.Add(Instantiate(CurrentTile, new Vector2(Mathf.Round((-1 * size) / 2) - 1, i), Quaternion.identity));
        }
        for (float i = tracker; i <= sizeSides; i++)
        {
            walls.Add(Instantiate(CurrentTile, new Vector2(Mathf.Round(size / 2) + 1, i), Quaternion.identity));
        }
        for (int i = 1; i <= Mathf.Round(size / 2); i++)
        {
            walls.Add(Instantiate(CurrentTile, new Vector2(i, sizeSides), Quaternion.identity));
        }
        for (int i = 1; i >= Mathf.Round((-1 * size) / 2); i--)
        {
            walls.Add(Instantiate(CurrentTile, new Vector2(i, sizeSides), Quaternion.identity));
        }

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

        // Spawn enemies
        var SpawnPoints = Nodes.Cast<Node>().Where(n => n.IsWalkable).Select(n => n.pos + NavMeshGenerator.Offset);
        for(int i=0;i<Enemies;i++){
            int CurrentPoint = Random.Range(0,SpawnPoints.Count());
            var enemy = Instantiate(BolsjeDreng, new Vector2(SpawnPoints.ElementAt(CurrentPoint).x,SpawnPoints.ElementAt(CurrentPoint).y), Quaternion.identity);
            enemy.GetComponent<BolsjeDreng>().Target = player;
        }
        GameController.AliveEnemies = Enemies;
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