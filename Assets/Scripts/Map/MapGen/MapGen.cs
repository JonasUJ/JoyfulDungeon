using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapGen : MonoBehaviour
{
    private static TMP_Text _TileInfo;
    public TMP_Text TileInfo;
    public Button BossButton;
    private static bool bossFight = false;
    private static MapTile _MapTilePrefab;
    public MapTile MapTilePrefab;
    public Vector2 Size;
    public int Padding = 4;
    private float[] typeWeights = new float[] { 0f, 0.7f, 0.95f, 1f };
    private static Vector2 _PlayerPosition;
    public static Vector2 PlayerPosition
    {
        get => _PlayerPosition;
        set
        {
            numberOfMoves++;
            _PlayerPosition = value;
        }
    }
    private static int numberOfMoves = -1;
    public static MapTileData[,] Map;
    private static Vector2 tilesize;
    private static Vector2 totalsize;
    private static Vector2 _Size;
    private static List<MapTile> instances = new List<MapTile>();

    private void Start()
    {
        BossButton.gameObject.SetActive(false);
        _Size = Size;
        _MapTilePrefab = MapTilePrefab;
        _TileInfo = TileInfo;

        if (Map == null)
        {
            PlayerPosition = new Vector2(Mathf.Floor(Size.x / 2), Mathf.Floor(Size.y / 2));
            tilesize = MapTilePrefab.GetComponent<RectTransform>().rect.size;
            totalsize = GetTotalSize();
            GenerateNew();
            UpdateMap();
        }

        Render();
        if (bossFight)
            BossButton.gameObject.SetActive(true);
    }

    public void GenerateNew()
    {
        Map = new MapTileData[(int)Size.x, (int)Size.y];
        Vector2 pos = new Vector2(-totalsize.x / 2 + tilesize.x / 2, totalsize.y / 2 - tilesize.y / 2);

        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                Map[x, y] = new MapTileData();
                Map[x, y].pos = new Vector2(pos.x, pos.y);
                Map[x, y].coords = new Vector2(x, y);
                float rnd = Random.value;
                for (int i = 0; i < typeWeights.Length; i++)
                    if (rnd < typeWeights[i])
                    {
                        Map[x, y].type = (TileType)i;
                        break;
                    }
                if (Map[x, y].coords == PlayerPosition)
                {
                    Map[x, y].type = TileType.None;
                    Map[x, y].state = TileState.Current;
                }

                pos.x += tilesize.x + Padding;
            }
            pos.y -= tilesize.y + Padding;
            pos.x = -totalsize.x / 2 + tilesize.y / 2;
        }
    }

    public static void Render()
    {
        instances.ForEach(m => Destroy(m));
        instances.Clear();

        for (int x = 0; x < _Size.x; x++)
        {
            for (int y = 0; y < _Size.y; y++)
            {
                var tile = Instantiate<MapTile>(_MapTilePrefab);
                instances.Add(tile);
                tile.transform.position = Map[x, y].pos;
                tile.transform.SetParent(GameController.GUICanvas.transform, false);
                tile.Type = Map[x, y].type;
                tile.State = Map[x, y].state;
                tile.GetComponentInChildren<MapTileButton>().tileData = Map[x, y];
            }
        }
    }

    Vector2 GetTotalSize()
    {
        return new Vector2(
            tilesize.x * _Size.x + Padding * (_Size.x - 1),
            tilesize.y * _Size.y + Padding * (_Size.y - 1));
    }

    public static void UpdateMap()
    {
        int possible = 0;
        for (int x = 0; x < Map.GetLength(0); x++)
        for (int y = 0; y < Map.GetLength(1); y++)
        {
            if (Map[x, y].state == TileState.Current)
                Map[x, y].state = TileState.Used;
            if (Map[x, y].coords == PlayerPosition)
            {
                Map[x, y].state = TileState.Current;
                continue;
            }
            if (Mathf.Abs(Map[x, y].coords.x - PlayerPosition.x) + Mathf.Abs(Map[x, y].coords.y - PlayerPosition.y) > (_Size.x + _Size.y) / 2 - numberOfMoves && Map[x, y].state != TileState.Used)
                Map[x, y].state = TileState.Disabled;
            if (Map[x, y].state != TileState.Used && Map[x, y].state != TileState.Disabled)
                if ((Mathf.Abs(Map[x, y].coords.x - PlayerPosition.x) == 1 &&
                    Mathf.Abs(Map[x, y].coords.y - PlayerPosition.y) == 0) ^
                    (Mathf.Abs(Map[x, y].coords.y - PlayerPosition.y) == 1 &&
                    Mathf.Abs(Map[x, y].coords.x - PlayerPosition.x) == 0))
                    {
                        Map[x, y].state = TileState.Possible;
                        possible += 1;
                    }
                else
                    Map[x, y].state = TileState.Default;
        }

        if (possible == 0)
        {
            bossFight = true;
        }
    }

    public static void SetInfo(string info)
    {
        _TileInfo.text = info;
    }

    public static void ResetMap()
    {
        Map = null;
        numberOfMoves = -1;
        bossFight = false;
    }
}
