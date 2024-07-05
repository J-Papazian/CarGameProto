using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyExplosive : Enemy
{
    [Header("Bomb")]
    [SerializeField] float BombTimer = 5.0f;
    [SerializeField] float BombRadius = 3.0f;
    [SerializeField] BombCollider Bomb = null;
    [SerializeField] TextMeshProUGUI BombTimerText = null;
    [SerializeField] bool Boss = false;

    private float bombTimer = 0.0f;
    private bool explosed = false;
    internal bool playerTakeDamages = false;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        player = gameManager.Player.gameObject;

        currentHealth = Health;

        StartBomb();
    }

    private void Update()
    {
        if (currentArena != null && !currentArena.isActiveWave)
            return;

        HealthText.text = currentHealth.ToString();

        bombTimer -= Time.deltaTime;
        BombTimerText.text = bombTimer.ToString("0");

        Vector3 bombScale = Bomb.transform.localScale * (BombTimer - bombTimer) / BombTimer;
        Bomb.BombZone().transform.localScale = bombScale;

        if (explosed)
        {
            if (Boss)
                return;

            Death();
        }

        if (bombTimer <= 0)
            Explose();

        Shoot();

        if (getDamage)
        {
            currentTimerProtected -= Time.deltaTime;
            if (currentTimerProtected <= 0.0f)
            {
                getDamage = false;
                Renderer.material.color = colorTemp;
                transform.localScale -= Vector3.one * 0.2f;
                currentTimerProtected = dataManager.Data.TimeEnemyProtectedToDamage;
            }
        }

        Move();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }

    protected override void OnCollisionStay(Collision collision)
    {
        base.OnCollisionStay(collision);
    }

    internal override void Init(Arena arena, bool initPosition = true)
    {
        Debug.Log("Boss");

        base.Init(arena, initPosition);

        bombTimer = BombTimer;
        explosed = false;

        if (!Boss)
            currentHealth = Health + (int)enemyLevel * 5;
    }

    private void StartBomb()
    {
        bombTimer = BombTimer;
        Bomb.transform.localScale = new Vector3(BombRadius, Bomb.transform.localScale.y, BombRadius);
    }

    private void Move()
    {
        if (GameManager.Instance.CurrentWave == DataManager.Instance.Data.FirstExplosiveEnemyArena)
            return;
        Vector3 direction = (gameManager.Player.transform.position - transform.position).normalized * Speed;
        rigidbody.velocity = direction;
    }

    protected override void Shoot()
    {
        if (Boss)
            base.Shoot();
    }

    protected override void ComputeLevel()
    {
        //base.ComputeLevel();
        Speed += 0.2f;
    }

    private void Explose()
    {
        bombTimer = BombTimer;
        if (!Boss)
            explosed = true;
        if (playerTakeDamages)
            BombTouchPlayer();
    }

    private void BombTouchPlayer()
    {
        GameManager.Instance.DealDamagesToPlayer(Damages);
    }

    internal override void Death()
    {
        base.Death();
    }
}
