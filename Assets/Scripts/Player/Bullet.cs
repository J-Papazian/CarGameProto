using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 5.0f;
    private float lifetime = 10.0f;

    private void Start()
    {
        GameManager.Instance.OnSwitchArena += OnSwitchArena;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnSwitchArena -= OnSwitchArena;
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        lifetime -= Time.deltaTime;
        if (lifetime <= 0.0f)
            gameObject.SetActive(false);
    }

    internal void Init(float speed)
    {
        this.speed = speed;
        lifetime = DataManager.Instance.Data.BulletLifetime;
    }

    private void OnSwitchArena()
    {
        gameObject.SetActive(false);
    }
}
