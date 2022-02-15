using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class GameManager : Singleton<GameManager>
{

    int gameStep;
    int maxGameStep = 2;
    int successMovement, faieldMovement;

    public int GameStep => gameStep;
    public int MaxGameStep => maxGameStep;

    public TextMeshProUGUI infoText;
    string infoString = "Step: ";

    public Image[] stepIndicators;


    private void OnEnable()
    {
        EventManager.GetInstance().OnGameStart += OnGameStarted;
        EventManager.GetInstance().OnPrepareLevel += OnPrepareLevel;
        EventManager.GetInstance().OnMakeMove += OnMakeMove;
        EventManager.GetInstance().OnUndo += OnUndo;
        EventManager.GetInstance().OnSuccessStep += SuccessGameStep;
        EventManager.GetInstance().OnFailStep += FailGameStep;
    }

    private void OnDisable()
    {
        if (EventManager.GetInstance() == null) return;
        EventManager.GetInstance().OnGameStart -= OnGameStarted;
        EventManager.GetInstance().OnPrepareLevel -= OnPrepareLevel;
        EventManager.GetInstance().OnMakeMove -= OnMakeMove;
        EventManager.GetInstance().OnUndo -= OnUndo;
        EventManager.GetInstance().OnSuccessStep -= SuccessGameStep;
        EventManager.GetInstance().OnFailStep -= FailGameStep;


    }
    private void Start()
    {
        infoText.text = infoString + " " + gameStep + "/" + maxGameStep;
    }
    private void OnGameStarted()
    {

    }
    public void OnPrepareLevel()
    {

    }
    public void UndoGame() { EventManager.GetInstance().Undo(); }
    void OnUndo()
    {
        StartCoroutine(nameof(LateOnUndo));

    }
    void OnMakeMove()
    {

        //StartCoroutine(nameof(LateMakeMove));
        //yield return new WaitForEndOfFrame();
        if (gameStep < maxGameStep)
            gameStep++;
        infoText.text = infoString + " " + gameStep + "/" + maxGameStep;
    }
    IEnumerator LateOnUndo()
    {
        if (gameStep > 0)
            stepIndicators[gameStep - 1].color = Color.white;

        yield return new WaitForEndOfFrame();
        if (gameStep > 0)
            gameStep--;
        infoText.text = infoString + " " + gameStep + "/" + maxGameStep;
    }

    void SuccessGameStep(int gameStep)
    {
        if (stepIndicators[gameStep - 1] != null)
            stepIndicators[gameStep - 1].color = Color.green;
    }
    void FailGameStep(int gameStep)
    {
        if (stepIndicators[gameStep - 1] != null)
            stepIndicators[gameStep - 1].color = Color.red;
    }
    //IEnumerator LateMakeMove()
    //{

    //}

    public void GameReset()
    {
        while (gameStep > 0) EventManager.GetInstance().Undo();
    }
    public void CheckFinal()
    {
        if (successMovement == maxGameStep && faieldMovement == 0)
        {
            print("Win");
        }
        else
        {
            print("Lose");
        }

    }
}
