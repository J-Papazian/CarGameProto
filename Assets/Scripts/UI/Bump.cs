using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class Bump : MonoBehaviour
{
    [SerializeField] bool UseUnscaledDeltaTime = false;
    [SerializeField] Vector3 BumpSize = Vector3.one;
    [SerializeField] float Speed = 5.0f;

    private bool bump = true;

    private RectTransform rectTransform = null;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (bump)
        {
            if (UseUnscaledDeltaTime)
                rectTransform.localScale += Vector3.one * Speed * Time.unscaledDeltaTime;
            else
                rectTransform.localScale += Vector3.one * Speed * Time.deltaTime;
            if (rectTransform.localScale.magnitude >= BumpSize.magnitude)
                bump = false;
        }
        else
        {
            if (UseUnscaledDeltaTime)
                rectTransform.localScale -= Vector3.one * Speed * Time.unscaledDeltaTime;
            else
                rectTransform.localScale -= Vector3.one * Speed * Time.deltaTime;
            if (rectTransform.localScale.magnitude <= Vector3.one.magnitude)
                bump = true;
        }
    }
}
