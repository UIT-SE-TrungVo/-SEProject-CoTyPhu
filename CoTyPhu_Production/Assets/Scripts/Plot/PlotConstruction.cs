using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PLOT_CONSTRUCTIONS ARE MARKET AND TEMPLE WHICH CAN BE BUILT ON
/// </summary>
public class PlotConstruction: Plot
{
	#region Status
	[SerializeField] List<IHirePriceChange> _listStatusHirePrice;

	#endregion


	//  Events ----------------------------------------
	List<IPayPlotFeeListener> _payPlotListeners = new List<IPayPlotFeeListener>();


    //  Properties ------------------------------------
    public int EntryFee { get
        {
			int baseEntryFee = _entryFee;
			int deltaFee = 0;
			foreach(var status in _listStatusHirePrice)
            {
				if(status != null)
                {
					deltaFee = (int)status.GethirePriceChange(baseEntryFee, deltaFee);
                }
            }
			return baseEntryFee + deltaFee;
        } }
	public int Price { get => _price; }
	public Player Owner 
	{ 
		get => _owner; 
		set { _owner = value; } 
	}
	public static float ReBuyOffset { get => _reBuyOffset; }
	public int PurchasePrice { get => Mathf.RoundToInt(_price * _reBuyOffset); }


	//  Fields ----------------------------------------
	protected int _entryFee = 50;
	[SerializeField] protected int _price;
	[SerializeField] protected Player _owner;
	protected static float _reBuyOffset = 1.5f;


	//  Initialization --------------------------------
	public PlotConstruction(PLOT id, string name, string description, int entryFee, int price) : base(id, name, description)
	{
		this._entryFee = entryFee;
		this._price = price;
		this._owner = null;
	}


	//  Methods ---------------------------------------
	public override IAction ActionOnEnter(Player obj)
    {
		//TODO: Check Owner --> do action based on Owner state
		return null;
    }

	public void AddStatus(IHirePriceChange newStatus)
    {
		if(_listStatusHirePrice == null)
        {
			_listStatusHirePrice = new List<IHirePriceChange>();
        }
		if(!_listStatusHirePrice.Contains(newStatus))
        {
			_listStatusHirePrice.Add(newStatus);
        }
    }

	public void RemoveStatus(IHirePriceChange status)
    {
		_listStatusHirePrice.Remove(status);
    }

	public void SubcribePayPlotFee(IPayPlotFeeListener listener)
    {
		if(_payPlotListeners == null)
        {
			_payPlotListeners = new List<IPayPlotFeeListener>();
        }

		if(!_payPlotListeners.Contains(listener))
        {
			_payPlotListeners.Add(listener);
        }
    }

	public void UnsubcribePayPlotFee(IPayPlotFeeListener listener)
    {
		if (_payPlotListeners == null)
		{
			_payPlotListeners = new List<IPayPlotFeeListener>();
		}

		_payPlotListeners.Remove(listener);
	}

	protected void NotifyPayPlotFee(Player player)
    {
		foreach(var listener in _payPlotListeners)
        {
			listener.OnPayPlotFee(player, this);
        }
    }
    //  Event Handlers --------------------------------
}
