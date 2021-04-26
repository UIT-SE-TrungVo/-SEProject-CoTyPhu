﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDiceListener
{
    [SerializeField] int _id;
    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }
    public MoveStraightEvenly moveComponent = null;
    public PLOT Location_PlotID;
    bool _isBroke;
    bool _notSubcribeDice = true;
    [SerializeField] bool minePlayer;
    [SerializeField] Button btnRoll;

    // Internal, saves the Actions the UI is supposed to do
    ActionList UIActions;

    // Start is called before the first frame update
    void Start()
    {
        // Long:
        if (moveComponent == null)
            moveComponent = GetComponent<MoveStraightEvenly>();
        if (moveComponent == null)
        {
            gameObject.AddComponent<MoveStraightEvenly>();
            moveComponent = GetComponent<MoveStraightEvenly>();
        }
        //moveComponent.lockY = true;
        //moveComponent.lockX = false;
        //moveComponent.lockZ = false;
    
        // Thanh:
        if (Dice.Ins() != null)
        {
            Dice.Ins().SubscribeDiceListener(this);
            _notSubcribeDice = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_notSubcribeDice)
        {
            Dice.Ins().SubscribeDiceListener(this);
            _notSubcribeDice = false;
        }
    }

    /// <summary>
    /// Commands Player to move to target Plot
    /// This will NOT move the player step-by-step.
    /// </summary>
    /// <param name="plotID"></param>
    public void MoveTo(PLOT plotID)
    {
        moveComponent.Target = Plot.plotDictionary[plotID].transform.position;
    }
    /// <summary>
    /// Return an Action that commands Player to move to target Plot
    /// This will NOT move the player step-by-step.
    /// When the Player reach the target location, Will call nextAction in the UIActions list
    /// </summary>
    /// <param name="plotID"></param>
    /// <returns></returns>
    public ICompletableAction ActionMoveTo(PLOT plotID)
    {
        LambdaCompletableAction action = new LambdaCompletableAction(() =>
        {
            // Content of the Action
            MoveTo(plotID);
        });

        action.preAction = () => 
        {
            // Before Moveto, register OnComplete to moveComponent
            moveComponent.ListenTargetReached(new LambdaAction(action.PerformOnComplete));
        };

        return action;
    }
    /// <summary>
    /// Commands Player to move a number of Plot
    /// This WILL trigger PassBy Effect
    /// </summary>
    /// <param name="plotsToMove"></param>
    public void MoveTo(int plotsToMove)
    {
        AddMovementToQueue(plotsToMove);
        UIActions.PerformAction();
    }
    public void AddMovementToQueue(int plotsToMove)
    {
        int iter = ((int)Location_PlotID + 1) % Plot.PLOT_AMOUNT;

        for (int i = 0; i < plotsToMove; i++)
        {
            PLOT cur = (PLOT)iter;

            // Queue the action move to next tile
            UIActions.AddBlockingAction(ActionMoveTo(cur));

            // Queue the action activate OnTilePass, which is non-blocking
            IAction temp = Plot.plotDictionary[cur].ActionOnPass(this);
            if (temp != null)
                UIActions.AddNonBlockAction(temp);
            
            // Go to next tile.
            iter = (iter + 1) % Plot.PLOT_AMOUNT;
        }
    }

    public void StartPhase(int phaseID)
    {
        if(phaseID == 1 && minePlayer)
        {
            btnRoll.gameObject.SetActive(true);
        }
    }

    private void StartPhaseDice()
    {

    }

    public void EndPhase()
    {
        
    }

    public void PausePhase()
    {

    }

    public void Roll()
    {
        Dice.Ins().Roll(_id);

        if(TurnDirector.Ins.IsMyTurn(Id))
        {
            btnRoll.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// This function receive callback result from Dice when its finish rolling
    /// </summary>
    /// <param name="idPlayer"></param>
    /// <param name="result"></param>
    public void OnRoll(int idPlayer, List<int> result)
    {
        Debug.Log(result.ToArray());

        /// Do some fancy animation here
        /*
        if (isdouble(result))
        {
            Action AnimationAction = new LambdaAction(() =>
            {
                SubscribeWhatever(new LambdaAction(() =>
                {
                    CallNextAction();
                }));

                // Animation
            });
            UIActions.Enqueue(AnimationAction);
            AddMovementToQueue(result.Sum());
            UIActions.Enqueue(AnimationAction);
            
        
            if (UIActions.Count > 0)
                UIActions.Dequeue().PerformAction();
        }//*/

        // only the one who roll & that is control by me can announce end of phase
        if (idPlayer == Id && minePlayer)
        {
            Debug.Log("end of phase");
            TurnDirector.Ins.EndOfPhase();
        }
    }

    IAction OnRollResult;
}
