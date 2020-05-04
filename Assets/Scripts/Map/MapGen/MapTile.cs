using System.Text;
using UnityEngine;
using UnityEngine.UI;

public enum TileType
{
    None,
    Enemy,
    Random,
    Chest,
    Boss,
}

public enum TileState
{
    Default,
    Disabled,
    Current,
    Used,
    Possible,
}

public struct MapTileData
{
    public Vector2 pos;
    public Vector2 coords;
    public TileType type;
    public TileState state;
    public string Display()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"Type: {type.ToString()}");
        return sb.ToString();
    }
}

public class MapTile : MonoBehaviour
{
    private TileType _Type;
    public TileType Type
    {
        get => _Type;
        set
        {
            switch (value)
            {
                case TileType.None:
                    foreground.color = Color.clear;
                    break;
                case TileType.Enemy:
                case TileType.Chest:
                case TileType.Random:
                case TileType.Boss:
                    foreground.sprite = Sprites[(int)value - 1];
                    break;
                default:
                    Debug.LogError("Unknown TileType in MapTile.TileType");
                    break;
            }
            _Type = value;
        }
    }
    private TileState _State = TileState.Default;
    public TileState State
    {
        get => _State;
        set
        {
            switch (value)
            {
                case TileState.Default:
                    background.color = Color.white;
                    break;
                case TileState.Disabled:
                    background.color = Color.red;
                    Type = TileType.Boss;
                    break;
                case TileState.Current:
                    background.color = Color.green;
                    break;
                case TileState.Used:
                    background.color = Color.grey;
                    break;
                case TileState.Possible:
                    background.color = Color.blue;
                    break;
                default:
                    Debug.LogError("Unknown TileState in MapTile.State");
                    break;

            }
            _State = value;
        }
    }
    public Sprite[] Sprites;
    private Image background;
    private Image foreground;

    void Awake()
    {
        Image[] images = GetComponentsInChildren<Image>();
        background = images[0];
        foreground = images[1];
    }
}
