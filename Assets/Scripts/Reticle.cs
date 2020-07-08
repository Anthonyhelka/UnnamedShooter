using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
    private RectTransform recticle;
    public float restingSize = 35.0f;
    public float currentSize = 0.0f;
    public float speed = 20.0f;
    public bool isFiring = false;
    public float resize;
    public float newSize, previousSize;

    void Start()
    {
        recticle = GetComponent<RectTransform>();
    }

    void Update()
    {
        currentSize = Mathf.Lerp(previousSize, newSize, Time.deltaTime * speed);
        recticle.sizeDelta = new Vector2(currentSize, currentSize);
    }

    public void UpdateReticle(float spread)
    {
        previousSize = currentSize;
        newSize = restingSize + spread * 500;
    }
}
