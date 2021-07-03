using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] GameObject crosshairParent;
    [SerializeField] GameObject[] crosshairs;
    [Header("Crosshair size values")]
    [SerializeField]
    float defaultCrosshairScale = 1f;
    [SerializeField]
    float crosshairScaleMin = 0.5f;
    [SerializeField]
    float crosshairScaleMax = 2f;
    [SerializeField]
    float crosshairScaleChangeMultiplier = 0.85f;
    [Header("Current crosshair size")]
    [SerializeField]
    float crosshairScale;
    [Header("Current crosshair style")]
    int currentCrosshairIndex = 0;

    private void Start()
    {
        crosshairScale = defaultCrosshairScale;
        for(int i=1; i<crosshairs.Length;i++)
        {
            crosshairs[i].SetActive(false);
        }
    }

    private void SetCrosshairSize()
    {
        crosshairScale = Mathf.Clamp(crosshairScale, crosshairScaleMin, crosshairScaleMax);
        crosshairParent.transform.localScale = new Vector3(crosshairScale, crosshairScale, crosshairScale);
    }

    void Update()
    {
        if (Input.GetKeyDown("9"))
        {
            crosshairs[currentCrosshairIndex % crosshairs.Length].SetActive(false);
            currentCrosshairIndex++;
            crosshairs[currentCrosshairIndex % crosshairs.Length].SetActive(true);
        }
        if (Input.GetKeyDown("0"))
        {
            crosshairScale = 1f;
            SetCrosshairSize();
        }
        if (Input.GetKeyDown("-"))
        {
            crosshairScale*= crosshairScaleChangeMultiplier;
            SetCrosshairSize();
        }
        if (Input.GetKeyDown("="))
        {
            crosshairScale /= crosshairScaleChangeMultiplier;
            SetCrosshairSize();
        }
    }
}
