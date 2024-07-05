using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    [SerializeField] Pooler ClassicPooler = null;
    [SerializeField] Pooler ExplosivePooler = null;
    [SerializeField] Pooler ObstaclePooler = null;
    [SerializeField] Pooler EnemyExplosionPooler = null;
    [SerializeField] Pooler BossPooler = null;
    [SerializeField] int MaxEnemies = 6;
    [SerializeField] int MaxExplosive = 2;
    [SerializeField] int MaxObstacle = 2;
    [SerializeField] BoxCollider ForwardBorderCollider = null;

    internal List<Enemy> Enemies { get; private set; } = new List<Enemy>();
    internal bool isActiveWave { get; private set; } = false;

    private int currentWave = 0;

    private GameManager gameManager = null;
    private DataManager dataManager = null;

    private void Awake()
    {
        gameManager = GameManager.Instance;

        dataManager = DataManager.Instance;
    }

    private void OnEnable()
    {
        ForwardBorderCollider.enabled = true;
    }

    internal void OnSwitchArena()
    {
        gameManager.SetCurrentArena(this);
        isActiveWave = true;
    }

    internal void Init(int wave, bool isActiveWave = false)
    {
        this.isActiveWave = isActiveWave;
        if (isActiveWave)
            gameManager.SetCurrentArena(this);
        currentWave = wave;

        // tuto explosif enemies
        if (wave == dataManager.Data.FirstExplosiveEnemyArena)
        {
            StartCoroutine(SpecificExplosiveWave());
            return;
        }

        // tuto shooting enemies
        else if (wave == dataManager.Data.FirstShootingEnemyArena)
        {
            StartCoroutine(SpecificShootingWave());
            return;
        }

        // first wave tuto
        else if (wave == 0)
        {
            StartCoroutine(FirstWave());
            return;
        }

        else if (wave == dataManager.Data.BossWave)
        {
            StartCoroutine(BossWave());
            return;
        }

        StartCoroutine(GenerateEnemies(wave));
    }

    private void OnDestroy()
    {
        for (int i = 0; i < Enemies.Count; ++i)
            Enemies[i].OnEnemyDied -= OnEnemyDied;
    }

    private IEnumerator GenerateEnemies(int wave)
    {
        yield return new WaitForEndOfFrame();

        int random = Random.Range(1 + wave / 2, MaxEnemies + 1);
        
        for (int i = 0; i < random; ++i)
        {
            if (wave <= dataManager.Data.FirstExplosiveEnemyArena)
                Enemies.Add(ClassicPooler.GetPooledObject().GetComponent<Enemy>());
            else
            {
                // random between classic enemy and explosive enemy
                int modulo = Random.Range(1, 3);
                if (modulo % 2 == 0)
                    Enemies.Add(ClassicPooler.GetPooledObject().GetComponent<Enemy>());
                else
                {
                    // check explosive ennemy number
                    int explosiveEnemies = 0;
                    for (int j = 0; j < i; ++j)
                    {
                        if (Enemies[j].GetComponent<EnemyExplosive>() != null)
                            explosiveEnemies++;
                    }

                    if (explosiveEnemies == MaxExplosive)
                        Enemies.Add(ClassicPooler.GetPooledObject().GetComponent<Enemy>());
                    else
                        Enemies.Add(ExplosivePooler.GetPooledObject().GetComponent<EnemyExplosive>());
                }
            }

            Enemies[i].gameObject.SetActive(true);
            Enemies[i].Init(this);
            Enemies[i].transform.position = gameManager.Player.transform.position;
            Enemies[i].transform.position += Vector3.forward * Random.Range(5, 41) + Vector3.forward * dataManager.Data.ArenaWidth;

            Enemies[i].OnEnemyDied += OnEnemyDied;
        }

        if (wave >= 5)
        {
            for (int i = 0; i < MaxObstacle; ++i)
            {
                int modulo = Random.Range(1, 4);
                if (modulo % 3 == 0)
                    ObstaclePooler.GetPooledObject().GetComponent<Obstacle>().Init(this);
            }
        }
    }

    private IEnumerator BossWave()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < 1; ++i)
        {
            Enemies.Add(BossPooler.GetPooledObject().GetComponent<EnemyExplosive>());
            Enemies[i].gameObject.SetActive(true);
            Enemies[i].Init(this, false);
            Enemies[i].transform.position += Vector3.forward * 90.0f;
        }

        Enemies[0].OnEnemyDied += OnEnemyDied;
    }

    private IEnumerator SpecificExplosiveWave()
    {
        yield return new WaitForEndOfFrame();
        
        for (int i = 0; i < 2; ++i)
        {
            Enemies.Add(ExplosivePooler.GetPooledObject().GetComponent<EnemyExplosive>());
            Enemies[i].gameObject.SetActive(true);
            Enemies[i].Init(this);
            Enemies[i].transform.position = gameManager.Player.transform.position;
            Enemies[i].transform.position += Vector3.forward * 15.0f + Vector3.forward * 15.0f * i + Vector3.forward * dataManager.Data.ArenaWidth;

            Enemies[i].OnEnemyDied += OnEnemyDied;
        }
    }

    private IEnumerator SpecificShootingWave()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < 1; ++i)
        {
            Enemies.Add(ClassicPooler.GetPooledObject().GetComponent<Enemy>());
            Enemies[i].gameObject.SetActive(true);
            Enemies[i].Init(this);
            Enemies[i].transform.position += Vector3.forward * 15.0f + Vector3.forward * currentWave * dataManager.Data.ArenaWidth;

            Enemies[i].OnEnemyDied += OnEnemyDied;
        }
    }

    private IEnumerator FirstWave()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < 3; ++i)
        {
            Enemies.Add(ClassicPooler.GetPooledObject().GetComponent<Enemy>());
            Enemies[i].gameObject.SetActive(true);
            Enemies[i].Init(this);
            Enemies[i].transform.position += Vector3.forward * 10.0f * (i + 1);

            Enemies[i].OnEnemyDied += OnEnemyDied;
        }
    }

    private void OnEnemyDied(Enemy enemy)
    {
        enemy.OnEnemyDied -= OnEnemyDied;
        Enemies.Remove(enemy);

        GameObject explosion = EnemyExplosionPooler.GetPooledObject();
        explosion.transform.position = enemy.transform.position + Vector3.one * 0.25f;
        explosion.SetActive(true);
        enemy.gameObject.SetActive(false);

        StartCoroutine(HideExplosion(explosion));

        if (Enemies.Count == 0)
            ArenaIsOver();
    }

    private void ArenaIsOver()
    {
        ForwardBorderCollider.enabled = false;
        gameManager.SwitchArena();
    }

    private IEnumerator HideExplosion(GameObject explosion)
    {
        yield return new WaitForSeconds(5.0f);

        explosion.SetActive(false);
    }
}
