using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : HealthBarBase
{
    private Image img;
    private Vector3 offset;
    public override Character Origin
    {
        get => origin;
        set
        {
            if (origin != null)
                origin.HealthChanged -= ChangeFillAmount;

            origin = value;
            hpMngr = origin.GetComponent<HealthManager>();

            origin.HealthChanged += ChangeFillAmount;
            offset = new Vector3(0f, GameController.GUISpriteObjectScreenPixelSize(origin.gameObject).y / 3, 0f);

            if (img == null)
                img = GetComponent<Image>();
            transform.localScale = Vector3.one / Camera.main.orthographicSize * 2f;
        }
    }

    void Start()
    {
        img = GetComponent<Image>();
        img.enabled = false;
    }

    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(origin.transform.position) + offset;
    }

    protected override void ChangeFillAmount(object sender, HealthChangedEventArgs e)
    {
        img.fillAmount = hpMngr.PercentHealth;
        img.enabled = e.NewHealth != origin.MaxHealth;
        hpMngr.SpawnHitNumber();
    }
}
