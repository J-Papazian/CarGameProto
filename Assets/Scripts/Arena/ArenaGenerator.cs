using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaGenerator : MonoBehaviour
{
    [SerializeField] Pooler Pooler = null;

    private Arena firstArena = null;
    private Arena secondArena = null;

    private GameManager gameManager = null;
    private DataManager dataManager = null;

    private void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.OnSwitchArena += OnSwitchArena;

        dataManager = DataManager.Instance;

        firstArena = GenerateNewArena();
        firstArena.name = "First Arena";
        firstArena.gameObject.SetActive(true);
        firstArena.Init(0, true);

        secondArena = GenerateNewArena();
        secondArena.name = "Second Arena";
        secondArena.transform.position = Vector3.forward * dataManager.Data.ArenaWidth * (GameManager.Instance.CurrentWave + 1);
        secondArena.gameObject.SetActive(true);
        secondArena.Init(1);
    }

    private void OnDestroy()
    {
        gameManager.OnSwitchArena -= OnSwitchArena;
    }

    private Arena GenerateNewArena()
    {
        return Pooler.GetPooledObject().GetComponent<Arena>();
    }

    private void OnSwitchArena()
    {
        if (firstArena.isActiveWave)
        {
            InitArena(firstArena, secondArena.transform.position);
            secondArena.OnSwitchArena();
        }
        else
        {
            InitArena(secondArena, firstArena.transform.position);
            firstArena.OnSwitchArena();
        }
    }

    private void InitArena(Arena arena, Vector3 position)
    {
        arena.transform.position = position + Vector3.forward * dataManager.Data.ArenaWidth;
        arena.Init(gameManager.CurrentWave + 1);
    }
}
