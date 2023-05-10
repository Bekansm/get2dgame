using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene {
         LobbyScene,
         LoadingScene,
         GameScene,
    }
    private static Scene targetScene;
    private static Action onLoaderCallback;


    public static void LoadNetwork(Scene targetScene) {
        onLoaderCallback = () => {
            NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
        };

            NetworkManager.Singleton.SceneManager.LoadScene(Scene.LoadingScene.ToString(), LoadSceneMode.Single);
           
    }


    
    public static void LoaderCallback() {
        if (onLoaderCallback != null) {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    
    }
}
