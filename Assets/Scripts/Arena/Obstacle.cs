using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Arena currentArena = null;

    private GameManager gameManager = null;
    private DataManager dataManager = null;

    void Update()
    {
        if (!currentArena.isActiveWave)
            return;

        if (gameManager.Transition)
            gameObject.SetActive(false);

        transform.position += transform.forward * dataManager.Data.ObstacleSpeed * Time.deltaTime;

        if (transform.position.z < gameManager.Player.transform.position.z - 20.0f)
            gameObject.SetActive(false);
    }

    internal void Init(Arena arena)
    {
        currentArena = arena;

        gameObject.SetActive(true);

        gameManager = GameManager.Instance;
        dataManager = DataManager.Instance;

        transform.position = Vector3.zero;

        transform.position += Vector3.right * Random.Range(-5, 6);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, gameManager.Player.transform.position.z + 200.0f + dataManager.Data.ArenaWidth);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameManager.Player.TakeDamages(dataManager.Data.ObstacleDamage);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().Death();
        }
    }
}
