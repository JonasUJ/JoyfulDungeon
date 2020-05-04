using System;
using UnityEngine;
using TMPro;

public class HitNumber : MonoBehaviour
{
    public float Number
    {
        get => float.Parse(text.text);
        set
        {
            if (text == null)
                text = GetComponent<TextMeshProUGUI>();
            text.text = (Mathf.FloorToInt(value * 10) / 10).ToString();
            text.fontSize = Size(value);
        }
    }

    public Color Color
    {
        get => text.color;
        set
        {
            if (text == null)
                text = GetComponent<TextMeshProUGUI>();
            text.color = value;
        }
    }

    private TextMeshProUGUI text;
    private float spawnedAt;
    private float direcion;
    private bool increasing;
    private Vector3 startPos;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        spawnedAt = Time.timeSinceLevelLoad;
        direcion = RandomDirection();
        increasing = direcion >= 0f;
        startPos = Camera.main.ScreenToWorldPoint(transform.position);
    }

    void Update()
    {
        float time = Time.timeSinceLevelLoad - spawnedAt;
        float alpha = Alpha(time);
        if (alpha <= 0.001f)
            Destroy(gameObject);
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        transform.position = PositionFromTime(time);
    }

    private Vector3 PositionFromTime(float time)
    {
        time = (increasing ? time : -time) / 3;
        return Camera.main.WorldToScreenPoint(startPos + new Vector3(3 * time, -20 * time * time + direcion * time, 0f));
    }

    private float Size(float amount)
    {
        return 20f / (1 + 100 * (float)Math.Pow(Math.E, -0.2f * amount)) + 16;
    }

    private float Alpha(float s)
    {
        return 255f / (1f + 10f * (float)Math.Pow(Math.E, 20f * s - 3f));
    }

    private float RandomDirection()
    {
        return (UnityEngine.Random.value - 0.5f) * 12f;
    }
}
