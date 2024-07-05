using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindVFX : MonoBehaviour
{
    [SerializeField] ParticleSystem ParticleSystem = null;

    private bool transition = false;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }


    private void Update()
    {
        if (gameManager.Transition && !transition)
            UseVFX();
        else
            transition = false;
    }

    private void UseVFX()
    {
        transition = true;
        ParticleSystem.Play();
    }
}
