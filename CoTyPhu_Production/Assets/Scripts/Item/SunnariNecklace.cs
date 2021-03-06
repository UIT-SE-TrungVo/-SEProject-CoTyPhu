using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunnariNecklace : SunnaryItem, IPlotPassByListener, IPayPlotFeeListener
{
    #region Properties
    [SerializeField] StatusHirePriceChange status;
    PlotConstruction plotResult = null;

    public PLOT? assignedPlot;
    public PLOT? AssignedPlot { get => assignedPlot; set => assignedPlot = value; }
    #endregion
    #region Handle Event
    public void OnPayPlotFee(Player player, PlotConstruction plot)
    {
        if (plot == plotResult && player != Owner)
        {
            plotResult.RemoveStatus(status);
            plotResult.UnsubcribePayPlotFee(this);
            Destroy(this.gameObject, 0.1f);
        }
    }
    public void OnPlotPassBy(Player player, Plot plot)
    {
        //throw new System.NotImplementedException();
        if (plot is PlotStart)
        {
            if (player==Owner)
            {
                player.ChangeMana(1);

                foreach (var plot_temp in Plot.BuildingPlot)
                {
                    if ((Plot.plotDictionary[plot_temp] as PlotConstruction).Owner== player)
                    {
                        plotResult = Plot.plotDictionary[plot_temp] as PlotConstruction;
                    }    
                }

                if (plotResult)
                {
                    plotResult.AddStatus(status);
                    AssignedPlot = plotResult.Id;
                    plotResult.SubcribePayPlotFee(this);
                }
                else
                {
                    Destroy(this.gameObject, 0.1f);
                }
                Activate("");
                player.RemoveItem(this);
                Plot.plotDictionary[PLOT.START].UnsubcribePlotPassByListner(this);
            }    
        }    
        
    }
    #endregion
    #region SunnariItem override
    public override bool LoadData()
    {
        //throw new System.NotImplementedException();
        return true;
    }
    public override bool StartListen()
    {
        Plot.plotDictionary[PLOT.START].SubcribePlotPassByListener(this);
        
        return true;
    }
    public override Player Owner { get => base.Owner; set { StartListen(); base.Owner = value; }  }
    #endregion
    #region Unity Method
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   


    #endregion
}
