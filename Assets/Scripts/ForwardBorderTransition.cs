using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ForwardBorderTransition : MonoBehaviour
{
    private GameManager gameManager = null;
    private DataManager dataManager = null;

    void Start()
    {
        gameManager = GameManager.Instance;
        dataManager = DataManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            gameManager.Player.GainXP(dataManager.Data.ExperienceByArena + 2.0f * gameManager.CurrentWave);
    }
}
