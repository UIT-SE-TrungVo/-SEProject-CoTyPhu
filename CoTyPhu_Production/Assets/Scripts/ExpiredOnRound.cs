﻿public class ExpiredOnRound
{
	//  Events ----------------------------------------


	//  Properties ------------------------------------
	public BaseStatus Status
	{
		get { return _status; }
		set { _status = value; }
	}

	public int ExpiredRound
	{
		get { return _expiredRound; }
		set { _expiredRound = value; }
	}

	//  Fields ----------------------------------------
	private BaseStatus _status;
	private int _expiredRound;
	private object _player; // sẽ thay bằng class Player sau khi merge

	//  Initialization --------------------------------
	public ExpiredOnRound(BaseStatus status, int expiredTurn, object player)
	{

	}


	//  Methods ---------------------------------------
	public bool StartListen()
	{
		//+= RemoveExpiredStatus
		return true;
	}

	public bool RemoveExpiredStatus()
	{
		return true;
	}


	//  Event Handlers --------------------------------

}