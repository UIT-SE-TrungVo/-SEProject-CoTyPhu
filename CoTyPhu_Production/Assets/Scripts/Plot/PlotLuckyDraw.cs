using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotLuckyDraw : Plot
{
    #region Construction
    PlotLuckyDraw(PLOT id, string name, string description)
        : base(id, name, description)
    { }
    #endregion

    #region base class
    public override IAction ActionOnEnter(Player player)
    {
        return new LambdaAction(() =>
        {
            NotifyPlotEnter(player);
            Bank.Ins.TakeLuckyDrawMoney(player, Bank.Ins.LuckyDrawMoney);

        }, base.ActionOnEnter(player));
    }
    #endregion

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }
}
