using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public delegate void OnGameStartDelegate();
    public event OnGameStartDelegate OnGameStart;

    public delegate void OnFinalStartDelegate();
    public event OnFinalStartDelegate OnFinalStart;

    public delegate void OnFinalEndDelegate();
    public event OnFinalEndDelegate OnFinalEnd;

    public delegate void OnFinishSuccDelegate();
    public event OnFinishSuccDelegate OnFinishSucc;

    public delegate void OnFinishFailDelegate();
    public event OnFinishFailDelegate OnFinishFail;

    public delegate void OnPrepareLevelDelegate();
    public event OnPrepareLevelDelegate OnPrepareLevel;

    public delegate void OnUndoDelegate();
    public event OnUndoDelegate OnUndo;

    public delegate void OnMakeMoveDelegate();
    public event OnMakeMoveDelegate OnMakeMove;

    public delegate void OnSuccessStepDelegate(int gameStep);
    public event OnSuccessStepDelegate OnSuccessStep;

    public delegate void OnFailStepDelegate(int gameStep);
    public event OnFailStepDelegate OnFailStep;

    public void GameStart()
    {
        OnGameStart?.Invoke();
    }


    public void PrepareLevel()
    {
        OnPrepareLevel?.Invoke();
    }
    public void Undo()
    {
        OnUndo?.Invoke();
    }
    public void MakeMove()
    {
        OnMakeMove?.Invoke();
    }

    public void SuccessStep(int gameStep)
    {
        OnSuccessStep?.Invoke(gameStep);
    }
    public void FailStep(int gameStep)
    {
        OnFailStep?.Invoke(gameStep);
    }
}
