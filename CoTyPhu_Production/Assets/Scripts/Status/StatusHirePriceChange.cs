using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusHirePriceChange : BaseStatus, IHirePriceChange
{
    [SerializeField] float HirePriceChange;
    public PlotConstruction targetPlot;

    public float hirePriceChange
    {
        get
        {
            return HirePriceChange;
        }
        set
        {
            HirePriceChange = value;
        }
    }

    public float GethirePriceChange(float basePrice, float delta)
    {
        delta += hirePriceChange * basePrice;

        return delta;
    }

    public override bool LoadData()
    {
        return true;
    }

    public override bool StartListen()
    {
        if(targetPlot != null)
        {
            targetPlot.AddStatus(this);
            return true;
        }
        else
        {
            Debug.Log("Must set target plot before listening");
            return false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool Remove(bool triggerEvent)
    {
        targetPlot.RemoveStatus(this);
        base.Remove(triggerEvent);
        Destroy(this.gameObject);

        return true;
    }

    public override bool ExcuteAction()
    {
        return true;
    }
}
