using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Retry : MonoBehaviour
{
    public static void RestartGame()
    {
        SceneManager.LoadScene(SceneUtility.GetBuildIndexByScenePath("Scenes/GameScene"));
    }
}
