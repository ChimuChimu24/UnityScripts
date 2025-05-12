using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public static class SceneLoader
{

    public enum Scene
    {
        TitleScene,    
        GameScene,
        LoadingScene,
    }

    private static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        SceneLoader.targetScene = targetScene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }

    public static void Load(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public static void ReloadCurrentScene()
    {
        Load(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

}
