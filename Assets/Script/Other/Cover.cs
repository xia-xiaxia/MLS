using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    private void OnEnable()
    {
        foreach (Transform child in transform)
        {
            if (child != this)
                child.gameObject.SetActive(true);
        }
    }
    private void OnDisable()
    {
        foreach (Transform child in transform)
        {
            if (child != this)
                child.gameObject.SetActive(false);
        }
    }
}
