using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapTileButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [HideInInspector]
    public MapTileData tileData;

    public void OnPointerClick(PointerEventData e) {
        if (tileData.state == TileState.Possible)
        {
            MapGen.PlayerPosition = tileData.coords;
            MapGen.UpdateMap();
            MapGen.Render();
            switch (tileData.type)
            {
                case TileType.Enemy:
                    GameController.LoadScene("RoomScene");
                    break;
                case TileType.Chest:
                    GameController.LoadScene("TreasureScene");
                    break;
                case TileType.Random:
                    if (Random.value < 0.75f)
                        GameController.LoadScene("RoomScene");
                    else
                        GameController.LoadScene("TreasureScene");
                    break;
                default:
                    break;
            }
        }
    }

    public void OnPointerEnter(PointerEventData e) {
        MapGen.SetInfo(tileData.Display());
    }
}
