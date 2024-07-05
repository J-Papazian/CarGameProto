using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    internal int Damages { get; private set; } = 0;
    private float shootSpeed = 5.0f;
    private float bulletSpeed = 15.0f;
    private float currentTimer = 0.0f;
    private Enemy target = null;

    private bool gameOver = false;

    private Player player = null;

    private GameManager gameManager = null;
    private DataManager dataManager = null;

    void Update()
    {
        if (gameOver)
            return;

        target = player.GetClosestEnemy();
        if (target != null)
        {
            gameManager.AddMarkerToTarget(target);
            transform.LookAt(target.transform);
        }

        if (target != null && currentTimer <= 0.0f)
            Shoot();

        currentTimer -= Time.deltaTime;
    }

    internal void Init(Player player)
    {
        gameManager = GameManager.Instance;
        dataManager = DataManager.Instance;

        this.player = player;
        Damages = dataManager.Data.TurretDamage;
        shootSpeed = dataManager.Data.TurretReloading;
        bulletSpeed = dataManager.Data.BulletSpeed;

        currentTimer = shootSpeed;
    }

    internal void UpdateReloading(float value)
    {
        shootSpeed -= value;
    }

    internal void UpdateAttack(int value)
    {
        Damages += value;
    }

    private void Shoot()
    {
        Bullet bullet = player.BulletPooler.GetPooledObject().GetComponent<Bullet>();
        bullet.gameObject.SetActive(true);
        bullet.transform.position = player.transform.position + Vector3.up * 0.2f;
        bullet.transform.SetParent(null);
        bullet.transform.LookAt(target.transform.position + Vector3.up * 0.2f);
        currentTimer = shootSpeed;

        bullet.Init(bulletSpeed);
    }

    internal void Over()
    {
        gameOver = true;
    }
}
