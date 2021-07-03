using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] GameObject crosshairParent;
    [SerializeField] GameObject[] crosshairs;

    [SerializeField] Slider scaleSlider;

    private void Start()
    {
        DisableCrosshairs();
        CreateMissingPrefs();
        EnableFromPrefs();
        ScaleFromPrefs();
        if (scaleSlider)
        {
            scaleSlider.value =(PlayerPrefs.GetFloat("crosshairScale") - 0.4f)/4.6f;
        }
    }

    void CreateMissingPrefs()
    {
        if (!PlayerPrefs.HasKey("crosshairStyle"))
        {
            PlayerPrefs.SetInt("crosshairStyle", 0);
        }
        if (!PlayerPrefs.HasKey("crosshairScale"))
        {
            PlayerPrefs.SetFloat("crosshairScale", 1);
        }
    }

    void DisableCrosshairs()
    {
        for (int i = 0; i < crosshairs.Length; i++)
        {
            crosshairs[i].SetActive(false);
        }
    }

    void EnableFromPrefs()
    {
        crosshairs[PlayerPrefs.GetInt("crosshairStyle")].SetActive(true);
    }

    void ScaleFromPrefs()
    {
        crosshairParent.transform.localScale = new Vector3(1, 1, 1) * PlayerPrefs.GetFloat("crosshairScale");
    }

    public void PreviousStyle()
    {
        DisableCrosshairs();
        PlayerPrefs.SetInt("crosshairStyle", PlayerPrefs.GetInt("crosshairStyle") - 1 + (PlayerPrefs.GetInt("crosshairStyle") > 0 ? 0 : crosshairs.Length));
        EnableFromPrefs();
    }

    public void NextStyle()
    {
        DisableCrosshairs();
        PlayerPrefs.SetInt("crosshairStyle", PlayerPrefs.GetInt("crosshairStyle") + 1 - ((PlayerPrefs.GetInt("crosshairStyle") + 1) < crosshairs.Length ? 0 : crosshairs.Length));
        EnableFromPrefs();
    }

    public void SetScale()
    {
        Debug.Log("changing scale!");
        float scale = 0.4f + scaleSlider.value * 4.6f;
        PlayerPrefs.SetFloat("crosshairScale", scale);
        ScaleFromPrefs();
    }
}
