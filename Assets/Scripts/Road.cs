using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    private Transform target = null;

    void Start()
    {
    }

    void Update()
    {
        if (target == null)
            target = GameManager.Instance.Player.transform;

        transform.position = new Vector3(transform.position.x, 0.0f, target.position.z);
    }
}
