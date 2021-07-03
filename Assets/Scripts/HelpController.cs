using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpController : MonoBehaviour
{
    [SerializeField] GameObject helpObject;

    void Update()
    {
        if (Input.GetKeyDown("h"))
        {
            helpObject.SetActive(!helpObject.activeSelf); 
        }
    }
}
