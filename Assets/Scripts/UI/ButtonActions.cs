using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActions : MonoBehaviour
{
    public void RunAction(string ActionName)
    {
        switch (ActionName)
        {
            case "Play":
                GameController.playerData.health = 6f;
                GameController.LoadScene("ArmoryScene");
                break;
            case "LoadRoom":
                GameController.LoadScene("RoomScene");
                break;
            case "LoadArmory":
                GameController.LoadScene("ArmoryScene");
                break;
            case "Quit":
                Application.Quit();
                break;
            case "Boss":
                GameController.LoadScene("BossScene");
                break;
            case "Return":
                GameController.LoadScene("MainMenu");
                break;
            case "HowTo":
                GameController.LoadScene("HowToScene");
                break;
            default:
                Debug.LogError($"No such action '{ActionName}'");
                break;
        }
    }
}
