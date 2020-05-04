using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public SceneLoader sceneLoader;
    private static SceneLoader _sceneLoader;
    public static List<Edge> NavMesh;
    public static Node[,] Nodes;
    public static PlayerData playerData = new PlayerData();
    public static Vector2 RespawnPoint;

    #region Instance
    public static GameController Instance;
    private static int aliveEnemies = 0;
    private static int aliveBosses = 0;
    public static int AliveEnemies
    {
        get => aliveEnemies;
        set
        {
            if (value == 0)
                GameController.LoadScene("MapScene");
            aliveEnemies = value;
        }
    }
    public static int AliveBosses
    {
        get => aliveBosses;
        set
        {
            if (value == 0)
                GameController.LoadScene("MainMenu");
            aliveBosses = value;
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            _sceneLoader = Instantiate(sceneLoader);
            DontDestroyOnLoad(_sceneLoader);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion
    #region Canvas
    private static Canvas guiCanvas = null;
    public static Canvas GUICanvas
    {
        get
        {
            if (guiCanvas == null)
                guiCanvas = GameObject.Find("GUICanvas").GetComponent<Canvas>();

            // if (guiCanvas == null)
            // {
            //     var obj = new GameObject("Canvas");
            //     guiCanvas = obj.AddComponent<Canvas>();
            // }
            return guiCanvas;
        }
    }
    #endregion
    #region World <=> Screen calulations
    public static Vector3 SpriteObjectWorldSize(GameObject origin)
    {
        SpriteRenderer render = origin.GetComponent<SpriteRenderer>();
        Vector3 worldSize = render.sprite.rect.size / render.sprite.pixelsPerUnit * origin.transform.lossyScale;
        return worldSize;
    }

    public static Vector3 SpriteObjectScreenSize(GameObject origin)
    {
        Vector2 screenSize = 0.5f * SpriteObjectWorldSize(origin) / Camera.main.orthographicSize;
        screenSize.y *= Camera.main.aspect;
        return screenSize;
    }

    private static Vector3 guiSpriteObjectScreenPixelSize(GameObject origin, Vector3 worldSize)
    {
        return Camera.main.WorldToScreenPoint(origin.transform.position + worldSize) - Camera.main.WorldToScreenPoint(origin.transform.position - worldSize);
    }

    public static Vector3 GUISpriteObjectScreenPixelSize(GameObject origin)
    {
        return guiSpriteObjectScreenPixelSize(origin, SpriteObjectWorldSize(origin));
    }

    public static float GUIScaleWithSpriteObject(GameObject origin, float width)
    {
        return GUISpriteObjectScreenPixelSize(origin).x / width;
    }
    #endregion
    #region SceneManagement
    public static void LoadScene(string sceneName)
    {
        _sceneLoader.LoadScene(sceneName);
    }
    #endregion
}

public enum WeaponTypes
{
    Stick,
    Sword,
    Bow,
}

public struct PlayerData
{
    public WeaponTypes weapon;
    public float health;
}
