using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] int MenuIndex = 1;

    internal void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    internal void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    internal void LoadNextLevel()
    {
        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextLevelIndex);
    }

    internal void LoadMenu()
    {
        SceneManager.LoadScene(MenuIndex);
    }
}
