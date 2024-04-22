using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCycler : MonoBehaviour
{
    public void CycleScenes()
    {
        var sceneNum = SceneManager.GetActiveScene().buildIndex;
        if (sceneNum + 1 >= SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(sceneNum + 1);
        }
    }
}
