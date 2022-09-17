using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private string persistentId = "";

    private SceneIndexes currentScene;

    private List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

    Action loadSceneState;

    private void Awake()
    {
        instance = this;

        persistentId = GetComponent<SaveSystem.PersistentId>().Id;
        currentScene = SceneIndexes.MAIN_MENU;
        SceneManager.LoadSceneAsync((int)currentScene, LoadSceneMode.Additive);
    }

    public void LoadGame(Dictionary<string, object> state, Action<Dictionary<string, object>> loadState)
    {
        loadSceneState = () => loadState(state);
        if (state.TryGetValue(persistentId, out object savedState))
        {
            var savedData = (SaveData)savedState;
            LoadScene((SceneIndexes)savedData.sceneIndex);
        }
    }

    public void SaveGame(ref Dictionary<string, object> state)
    {
        state[persistentId] = new SaveData()
        {
            sceneIndex = (int)currentScene
        };
    }

    public void NewGame()
    {
        LoadScene(SceneIndexes.GAME_SCENE);
    }

    public void LoadScene(SceneIndexes sceneIndex)
    {
        SceneManager.UnloadSceneAsync((int)currentScene);
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)sceneIndex, LoadSceneMode.Additive));
        currentScene = sceneIndex;
        StartCoroutine(TrackSceneLoadingProgress());
    }

    public IEnumerator TrackSceneLoadingProgress()
    {
        foreach (var scene in scenesLoading)
        {
            while (!scene.isDone)
            {
                yield return null;
            }
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)currentScene));
        if (loadSceneState != null)
        { 
            loadSceneState();
        }
    }

    [System.Serializable]
    public class SaveData
    {
        public int sceneIndex;
        public SaveData()
        {
            sceneIndex = (int)SceneIndexes.MAIN_MENU;
        }
    }
}