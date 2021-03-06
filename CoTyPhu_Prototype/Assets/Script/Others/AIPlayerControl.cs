using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerControl : PlayerControl
{
    new void Update()
    {
        Debug.Log(turnBaseManager.phase);
        if (number_of_moving_turn <= 0)
        {

        }
        if (number_of_moving_turn > 0)
        {
            if (turnBaseManager.phase == 0)
            {
                if (true)
                {
                    turnBaseManager.turn_count++;
                    turn_maximum_count++;
                    diceManager.Roll(currentNumberOfDices);
                    if (currentNumberOfDices == 2 && diceManager.IsDouble())
                    {
                        if (turn_maximum_count >= turn_maximum_limit)
                        {
                            number_of_moving_turn = 1;
                            //code to fly to prison here

                            //
                            turnBaseManager.phase = 4;
                            return;
                        }
                        else
                        {
                            number_of_moving_turn++;
                        }
                        state_jail = 1;
                    }
                    Jump(state_jail * diceManager.dice_sum);
                    turnBaseManager.phase = 1;
                    ResetBuildStatus();
                }
                return;
            }

            if (turnBaseManager.phase == 1)
            {
                if (jump_delay_count < jump_delay)
                {
                    jump_delay_count += Time.deltaTime;
                    transform.position += (next_position - prev_position) * Time.deltaTime / jump_delay;
                }
                else
                {
                    if (transform.position != next_position)
                    {
                        transform.position = next_position;
                        plotManager.GetComponent<PlotManager>().listPlot.Find(p => p.plotID == cur_location).ActivePlotPassByEffect(this);
                    }

                    if (dest_location != cur_location)
                    {
                        prev_position = SetNewPostition(cur_location);
                        cur_location += 1;
                        if (cur_location >= 32)
                        {
                            cur_location -= 32;
                        }
                        jump_delay_count = 0;
                        next_position = SetNewPostition(cur_location);
                    }
                }

                //code when step on the plot
                if (transform.position == SetNewPostition(dest_location))
                {
                    turnBaseManager.phase = 2;
                }

                return;
            }

            if (turnBaseManager.phase == 2)
            {
                plotManager.GetComponent<PlotManager>().listPlot.Find(p => p.plotID == dest_location).ActivePlotEffect(this);
                turnBaseManager.phase = 4;

                return;
            }

            if (turnBaseManager.phase == 4)
            {
                number_of_moving_turn--;
                if (number_of_moving_turn > 0)
                {

                }
                else
                {
                    PlayerControl p = turnBaseManager.listPlayer.Dequeue();
                    p.number_of_moving_turn++;
                    turnBaseManager.listPlayer.Enqueue(p);
                    turn_maximum_count = 0;
                }
                turnBaseManager.phase = 0;
                return;
            }
        }
    }
}
