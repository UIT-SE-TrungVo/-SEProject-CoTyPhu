using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// THE BANK CONTROLS ALL MONEYS FLOW IN GAME
/// </summary>
public class Bank: MonoBehaviour
{
	//  Events ----------------------------------------

	//	Singleton -------------------------------------
	private static Bank _ins;
	public static Bank Ins
    {
		get { return _ins; }
    }

	//  Properties ------------------------------------
	public int MoneyBank { get => _moneyBank; }
	public Dictionary<Player,int> AllMoneyPlayers { get => _moneyPlayer; }
	
	[System.Serializable]
	public class PairPlayer
    {
		public Player player;
		public int money;
    }
	public List<PairPlayer> _moneyPlayers = new List<PairPlayer>();

	//  Fields ----------------------------------------
	private int _moneyBank;
	[SerializeField] private Dictionary<Player, int> _moneyPlayer = new Dictionary<Player, int>();


	//  Initialization --------------------------------
	public Bank()
    {

    }

	public Bank(int moneyBank, Player[] arrPlayers)
	{
		_moneyBank = moneyBank;
		foreach (Player player in arrPlayers)
        {
			AddPlayer(player);
        }
	}

    //  Unity Methods ---------------------------------
    private void Start()
    {
		_ins = this;
    }


    //  Methods ---------------------------------------
    public int MoneyPlayer(Player player)
    {
		if (_moneyPlayer.ContainsKey(player))
			return _moneyPlayer[player];
		else 
			return -65536;
    }

	public void AddPlayer(Player player)
    {
		if(_moneyPlayer.ContainsKey(player))
        {
			Debug.LogError("Player added duplicated in Bank.AddPlayer()");
        }

		_moneyPlayer.Add(player, 500);
		_moneyPlayers.Add(new PairPlayer() { player = player, money = 500 });
	}

	public void RemovePlayer(Player player)
	{
		//Take all money of the player and remove him
		if (_moneyPlayer.ContainsKey(player))
        {
			TakeMoney(player, MoneyPlayer(player));
			_moneyPlayer.Remove(player);
		}
	}

	public void TakeMoney(Player player, int amount)
    {
		if (!_moneyPlayer.ContainsKey(player)) return;

		_moneyPlayer[player] -= amount;
		_moneyPlayers.Find(x => x.player == player).money -= amount;
		if (_moneyPlayer[player] <= 0)
        {
			//TODO: Bankrupt the player
        }
	}

	public void SendMoney(Player player, int amount)
	{
		if (!_moneyPlayer.ContainsKey(player)) return;

		_moneyPlayer[player] += amount;
	}

	public void TransactBetweenPlayers(Player player1, Player player2, int amount)
	{
		if (!_moneyPlayer.ContainsKey(player1) || !_moneyPlayer.ContainsKey(player2) || player1 == player2) return;

		TakeMoney(player1, amount);
		SendMoney(player2, amount);
	}

	//  Event Handlers --------------------------------
}