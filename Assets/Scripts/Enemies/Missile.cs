using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    private bool init = false;
    private Vector3 targetPosition;
    private float speed;
    private int damages;
    private Transform parent;

    void Update()
    {
        if (init)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            if (Vector3.Distance(transform.position, targetPosition) >= 40.0f)
                ResetMissile();
        }
    }

    internal void Init(Vector3 targetPosition, float speed, int damages, Transform parent)
    {
        this.targetPosition = targetPosition;
        this.speed = speed;
        this.damages = damages;
        transform.LookAt(targetPosition);

        init = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.DealDamagesToPlayer(damages);
            ResetMissile();
        }
    }

    private void ResetMissile()
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }
}
