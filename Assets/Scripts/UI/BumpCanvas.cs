using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpCanvas : MonoBehaviour
{
    float timer = 0.0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= DataManager.Instance.Data.BumpThemAllPanelTime)
            GameManager.Instance.DestroyObject(gameObject);
    }
}
