using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : HealthBarBase
{
    protected override void ChangeFillAmount(object sender, HealthChangedEventArgs e)
    {
        float total = e.NewHealth;
        for (int i = 0; i < GameController.GUICanvas.GetComponent<GUICanvasLogic>().heartContainer.Hearts.Length; i++)
        {
            var heart = GameController.GUICanvas.GetComponent<GUICanvasLogic>().heartContainer.Hearts[i];
            heart.GetComponent<Image>().fillAmount = total / 2;
            total -= 2;
        }
        hpMngr.SpawnHitNumber();
    }
}
