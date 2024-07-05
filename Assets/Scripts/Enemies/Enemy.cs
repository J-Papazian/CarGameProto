using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EnemyLevel
{
    Zero = 0,
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Height = 8,
    Nine = 9,
}

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int Health = 10;
    [SerializeField] protected float Speed = 3.0f;
    [SerializeField] float RotateAngle = 30.0f;
    [SerializeField] protected int Damages = 10;
    [SerializeField] protected Image HealthImage = null;
    [SerializeField] protected TextMeshProUGUI HealthText = null;
    [SerializeField] protected Renderer Renderer = null;
    [Header("Shoot")]
    [SerializeField] Pooler MissilePooler = null;
    [SerializeField] float MissileSpeed = 10.0f;

    [SerializeField] LineRenderer LineRenderer = null;

    protected GameObject player;
    protected EnemyLevel enemyLevel = EnemyLevel.Zero;
    protected int currentHealth = 0;
    protected bool getDamage = false;
    protected Color colorTemp;
    protected float currentTimerProtected = 0.0f;
    
    private bool direction = false;

    // shoot
    private float minTimeBetweeTwoShoot = 5.0f;
    private float maxTimeBetweeTwoShoot = 7.0f;
    private float minTimeToShoot = 1.0f;
    private float maxTimeToShoot = 5.0f;
    private bool startShoot = false;
    private bool shoot = false;
    private float nextShootTimer = Mathf.Infinity;
    private Coroutine shootCoroutine = null;
    //

    protected RigidbodyConstraints constraints;

    protected Arena currentArena = null;

    protected new Rigidbody rigidbody = null;

    internal System.Action<Enemy> OnEnemyDied;

    protected GameManager gameManager;
    protected DataManager dataManager;

    protected virtual void Awake()
    {
        gameManager = GameManager.Instance;
        dataManager = DataManager.Instance;
    }

    protected virtual void Start()
    {
        player = gameManager.Player.gameObject;

        minTimeBetweeTwoShoot = dataManager.Data.MinTimeBetweeTwoShoot;
        maxTimeBetweeTwoShoot = dataManager.Data.MaxTimeBetweeTwoShoot;
        minTimeToShoot = dataManager.Data.MinTimeToShoot;
        maxTimeToShoot = dataManager.Data.MaxTimeToShoot;

        currentHealth = Health;


        int random = Random.Range(1, 3);
        if (random % 2 == 0)
            direction = true;
    }

    void Update()
    {
        if (currentArena != null && !currentArena.isActiveWave)
            return;

        HealthText.text = currentHealth.ToString();

        if (!shoot)
            Move();
        if (startShoot && gameManager.CurrentWave >= dataManager.Data.FirstShootingEnemyArena)
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

        nextShootTimer -= Time.deltaTime;
        if (nextShootTimer <= 0.0f)
            startShoot = true;
    }

    #region Collision

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !getDamage || collision.gameObject.CompareTag("Player") && gameManager.Player.IsDashing)
        {
            if (!gameManager.Player.IsDashing)
            {
                int damages = gameManager.Player.GetDamages();
                TakeDamages(damages);
            }
            else
            {
                int damages = gameManager.Player.GetDashDamages();
                gameManager.Player.DashOver();
                TakeDamages(damages);
            }
        }

        else if (collision.gameObject.CompareTag("Bullet") && !getDamage)
        {
            if (!gameManager.CurrentArena.Enemies.Contains(this))
                return;

            int damages = gameManager.Player.GetTurretDamages();
            TakeDamages(damages);
            collision.gameObject.SetActive(false);
        }
    }

    protected virtual void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !getDamage)
        {
            int damages = gameManager.Player.GetDamages();
            ChangeHealthFillAmout(damages);
            TakeDamages(damages);
        }
    }

    #endregion

    protected void TakeDamages(int damages)
    {
        getDamage = true;

        gameManager.Vibrate();
        StopShoot();
        ChangeHealthFillAmout(damages);

        currentHealth -= damages;

        colorTemp = Renderer.material.color;
        Renderer.material.color = Color.red;

        transform.localScale += Vector3.one * 0.2f;

        if (currentHealth <= 0)
            Death();
    }

    internal virtual void Death()
    {
        if (getDamage)
        {
            transform.localScale -= Vector3.one * 0.2f;
            Renderer.material.color = colorTemp;
        }
        getDamage = false;

        OnEnemyDied?.Invoke(this);
    }

    protected virtual void ChangeHealthFillAmout(int damages)
    {
        HealthImage.fillAmount = ((currentHealth - damages) * 100.0f / Health) / 100.0f;
    }

    internal virtual void Init(Arena arena, bool initPosition = true)
    {
        enemyLevel = (EnemyLevel)gameManager.CurrentWave;

        rigidbody = GetComponent<Rigidbody>();
        constraints = rigidbody.constraints;

        ComputeLevel();

        currentArena = arena;
        currentTimerProtected = dataManager.Data.TimeEnemyProtectedToDamage;
        HealthImage.fillAmount = 1.0f;

        transform.position = gameManager.Player.transform.position;
        if (initPosition)
            transform.position += Vector3.right * Random.Range(-5, 6);

        nextShootTimer = Random.Range(minTimeToShoot, maxTimeToShoot);
    }

    protected virtual void ComputeLevel()
    {
        Damages += (int)enemyLevel;
        minTimeBetweeTwoShoot -= 0.5f;
        maxTimeBetweeTwoShoot -= 0.5f;
        minTimeToShoot -= 0.2f;
        maxTimeToShoot -= 0.2f;
        currentHealth = Health + (int)enemyLevel * 5;
    }

    private void Move()
    {
        float xVelocity = 0;
        if (direction)
            xVelocity = Mathf.Sin(Time.time) * Speed;
        else
            xVelocity = Mathf.Sin(Time.time) * -Speed;

        rigidbody.velocity = new Vector3(xVelocity, 0.0f, 0.0f);

        if (xVelocity > 0)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, RotateAngle, 0.0f), Time.deltaTime);
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, -RotateAngle, 0.0f), Time.deltaTime);
    }

    protected virtual void Shoot()
    {
        if (shoot)
            return;

        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        shoot = true;
        rigidbody.velocity = Vector3.zero;

        shootCoroutine = StartCoroutine(TraceLineRenderer());
    }

    protected IEnumerator TraceLineRenderer()
    {
        LineRenderer.gameObject.SetActive(true);
        LineRenderer.SetPosition(0, transform.position + Vector3.up * 0.25f);
        float timer = 0.0f;
        while (Vector3.Distance(LineRenderer.GetPosition(1), gameManager.Player.transform.position) > 1.0f)
        {
            Vector3 lerp = Vector3.Lerp(transform.position + Vector3.up * 0.25f, gameManager.Player.transform.position + Vector3.up * 0.25f, timer);
            LineRenderer.SetPosition(1, lerp);
            timer += Time.deltaTime * 0.5f;

            yield return null;
        }

        rigidbody.constraints = constraints;

        LineRenderer.SetPosition(1, transform.position);
        LineRenderer.gameObject.SetActive(false);

        Missile currentMissle = MissilePooler.GetPooledObject().GetComponent<Missile>();

        currentMissle.gameObject.SetActive(true);
        currentMissle.transform.position = transform.position;
        currentMissle.Init(gameManager.Player.transform.position + (Vector3.up / 2.0f), MissileSpeed, Damages, transform);

        shoot = false;
        startShoot = false;

        nextShootTimer = Random.Range(minTimeBetweeTwoShoot, maxTimeBetweeTwoShoot);
    }

    protected void StopShoot()
    {
        if (shootCoroutine != null)
            StopCoroutine(shootCoroutine);

        shoot = false;
        startShoot = false;
        rigidbody.constraints = constraints;

        if (LineRenderer != null)
        {
            LineRenderer.SetPosition(1, transform.position);
            LineRenderer.gameObject.SetActive(false);
        }

        nextShootTimer = Random.Range(minTimeBetweeTwoShoot, maxTimeBetweeTwoShoot);
    }
}
