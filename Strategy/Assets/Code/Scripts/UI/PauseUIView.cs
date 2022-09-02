using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPauseUIView
{
    public void TransitionToPauseMenuView();
    public void TransitionToLoadGameView();
    public void TransitionToSaveGameView();
    public void HideUI();
}

public class PauseUIView : MonoBehaviour, IPauseUIView
{
    private GameObject loadGameView;
    private GameObject saveGameView;
    private GameObject pauseMenuView;

    private void Awake()
    {
        loadGameView = GetChildGoReference<LoadGameView>();
        saveGameView = GetChildGoReference<SaveGameView>();
        pauseMenuView = GetChildGoReference<PauseMenuView>();
    }


    private GameObject GetChildGoReference<ChildComponentType>() where ChildComponentType : MonoBehaviour
    {
        Type childType = typeof(ChildComponentType);
        var child = GetComponentInChildren<ChildComponentType>().gameObject; 
        if(child == null)
        {
            Debug.LogError("Children are missing component " + childType.ToString());
            return null;
        }
        child.SetActive(false);
        return child;
    }

    public void TransitionToLoadGameView()
    {
        loadGameView.SetActive(true);
        saveGameView.SetActive(false);
        pauseMenuView.SetActive(false);
    }

    public void TransitionToPauseMenuView()
    {
        loadGameView.SetActive(false);
        saveGameView.SetActive(false);
        pauseMenuView.SetActive(true);
    }

    public void TransitionToSaveGameView()
    {
        loadGameView.SetActive(false);
        saveGameView.SetActive(true);
        pauseMenuView.SetActive(false);
    }

    public void HideUI()
    {
        loadGameView.SetActive(false);
        saveGameView.SetActive(false);
        pauseMenuView.SetActive(false);
    }
}
