using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStatusDetail : MonoBehaviour
{
    public bool isRemoveByTurnDurationEnd;
    public int duration_by_turn = 1;
    public bool isRemoveByActivationTime;
    public int number_of_max_activation = 0;
    public bool isRemoveByNextOwnerTurn;

    public PlayerControl source;
    public int turn_start;
    public string description;

    //stat for plot status
    public float percent_hire_price_change = 0;
    public float percent_buy_price_change = 0;
    public float percent_buy_back_price_change = 0;
    //stat for player status
    public float percent_pay_change = 0;
    public float percent_gain_change = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveStatus()
    {
        Destroy(this.gameObject);
    }
}
