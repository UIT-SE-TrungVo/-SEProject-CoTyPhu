using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// This object handle all the buy, rebuy, sell, ... request, callback related to plot
/// </summary>
public class PlotManager : MonoBehaviourPun
{
    #region Event
    #region Buy
    public delegate void Buy(Player player, PlotConstruction plot);
    public event Buy OnBuyCallback;

    public delegate void BuyFail(string msg);
    public event BuyFail OnBuyFailCallback;
    #endregion

    #region Upgrade
    public delegate void UpgradeSuccess(Player player, PlotConstructionMarket plot, int level);
    public event UpgradeSuccess OnUpgradeSuccess;

    public delegate void UpgradeFail(string msg);
    public event UpgradeFail OnUpgradeFail;
    #endregion

    #region Prison Release

    public delegate void OnReleaseSuccess();
    OnReleaseSuccess _onReleaseSuccess;

    public delegate void OnReleaseFail(string reason);
    OnReleaseFail _onReleaseFail;

    #endregion

    #endregion

    #region Properties

    #endregion

    #region Unity methods

    private void Update()
    {
        
    }

    #endregion

    #region Method
    #region Buy
    [PunRPC]
    private void RequestBuyServer(int playerId, int plotId, string clientID)
    {
        Player player = TurnDirector.Ins.GetPlayer(playerId);
        PlotConstruction plot = Plot.plotDictionary[(PLOT)plotId] as PlotConstruction;
        var cs = PhotonNetwork.PlayerList;
        Photon.Realtime.Player client = PhotonNetwork.PlayerList.Single(x => x.UserId == clientID);

        if(player == null | plot == null)
        {
            photonView.RPC("BuyFailCallback", client, "playerID " + playerId + " or plotID " + plotId + " not found");
            return;
        }
        else if(Bank.Ins.MoneyPlayer(player) < plot.PurchasePrice)
        {
            photonView.RPC("BuyFailCallback", client, "Not enough money");
            return;
        }
        else
        {
            photonView.RPC("BuySuccessCallback", RpcTarget.All, playerId, plotId);
            return;
        }
    }

    [PunRPC]
    public void BuySuccessCallback(int playerId, int plotId)
    {
        Player player = TurnDirector.Ins.GetPlayer(playerId);
        PlotConstruction plot = Plot.plotDictionary[(PLOT)plotId] as PlotConstruction;

        Bank.Ins.TakeMoney(player, plot.PurchasePrice, true);
        if(plot.Owner != null)
        {
            Bank.Ins.SendMoney(plot.Owner, plot.PurchasePrice, true);
        }

        plot.Owner = player;

        OnBuyCallback?.Invoke(player, plot);
    }

    [PunRPC]
    public void BuyFailCallback(string msg)
    {
        OnBuyFailCallback?.Invoke(msg);
    }

    public void RequestBuy(Player player, PlotConstruction plot)
    {
        if(Bank.Ins.MoneyPlayer(player) >= plot.PurchasePrice)
        {
            photonView.RPC("RequestBuyServer", RpcTarget.MasterClient, player.Id, (int)plot.Id, PhotonNetwork.LocalPlayer.UserId);
        }
        else
        {
            OnBuyFailCallback?.Invoke("Not enough money");
        }
    }
    #endregion

    #region Upgrade Market
    [PunRPC]
    private void _RequestUpgradeServer(int playerID, int plotID, int level, string clientID)
    {
        Player player = TurnDirector.Ins.GetPlayer(playerID);
        var client = PhotonNetwork.PlayerList.Single(x => x.UserId == clientID);

        if (player == null)
        {
            photonView.RPC("UpgradeFailCallback", client, "Cannot find player with playerID: " + playerID.ToString());
            return;
        }

        if (!Plot.plotDictionary.ContainsKey((PLOT)plotID))
        {
            photonView.RPC("UpgradeFailCallback", client, "Cannot find plot with plotID" + plotID.ToString());
            return;
        }

        if (!(Plot.plotDictionary[(PLOT)plotID] is PlotConstructionMarket))
        {
            photonView.RPC("UpgradeFailCallback", client, 
                           "Plot with plotID: " + plotID.ToString() + " is not a Plot Construction Market");
            return;
        }

        PlotConstructionMarket plot = Plot.plotDictionary[(PLOT)plotID] as PlotConstructionMarket;

        if (plot.UpgradeFee(plot.Level, level) > Bank.Ins.MoneyPlayer(player))
        {
            photonView.RPC("UpgradeFailCallback", client, "Not enough money to upgrade");
            return;
        }

        photonView.RPC("UpgradeSuccessCallback", RpcTarget.All, playerID, plotID, level, plot.UpgradeFee(plot.Level, level));
    }

    [PunRPC]
    private void UpgradeSuccessCallback(int playerID, int plotID, int level, int money)
    {
        Player player = TurnDirector.Ins.GetPlayer(playerID);

        PlotConstructionMarket plot = Plot.plotDictionary[(PLOT)plotID] as PlotConstructionMarket;

        if (player == null) Debug.LogError("Upgrade Success Callback cannot find player " + playerID.ToString());
        if(plot == null) Debug.LogError("Upgrade Success Callback cannot find plot " + playerID.ToString() + " as PlotConstructionMarket");

        Bank.Ins.TakeMoney(player, money);
        plot.Upgrade(level);
        OnUpgradeSuccess?.Invoke(player, plot, level);
    }

    [PunRPC]
    private void UpgradeFailCallback(string msg)
    {
        OnUpgradeFail?.Invoke(msg);
    }

    public void RequestUpgrade(int playerID, PLOT plotID, int level)
    {
        Player player = TurnDirector.Ins.GetPlayer(playerID);

        if(player == null)
        {
            UpgradeFailCallback("Cannot find player with playerID: " + playerID.ToString());
            return;
        }

        if(!Plot.plotDictionary.ContainsKey(plotID))
        {
            UpgradeFailCallback("Cannot find plot with plotID" + plotID.ToString());
            return;
        }

        if (!(Plot.plotDictionary[plotID] is PlotConstructionMarket))
        {
            UpgradeFailCallback("Plot with plotID: " + plotID.ToString() +" is not a Plot Construction Market");
            return;
        }

        PlotConstructionMarket plot = Plot.plotDictionary[plotID] as PlotConstructionMarket;

        if (plot.UpgradeFee(plot.Level, level) > Bank.Ins.MoneyPlayer(player))
        {
            UpgradeFailCallback("Not enough money to upgrade");
            return;
        }

        photonView.RPC("_RequestUpgradeServer", RpcTarget.MasterClient, playerID, (int)plotID, level, PhotonNetwork.LocalPlayer.UserId);
    }

    #endregion

    #region Imprison, Release

    public delegate void ReleaseFunc(Player player);
    public ReleaseFunc releaseFunc; // this function is for Prison Plot to set, because Imprison in PrisonPlot is private

    [PunRPC]
    private void _releaseServer(int idPlayer, string idClient)
    {
        photonView.RPC("_releaseClient", RpcTarget.AllBuffered, idPlayer);
    }

    [PunRPC]
    private void _releaseWithMoneyServer(int idPlayer, string idClient)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Player player = TurnDirector.Ins.GetPlayer(idPlayer);

        int playerMoney = Bank.Ins.MoneyPlayer(player);
        int releaseFee = (Plot.plotDictionary[PLOT.PRISON] as PlotPrison).GetReleaseFee(player);

        if (playerMoney > releaseFee)
        {
            photonView.RPC("_releaseWithMoneyClient", RpcTarget.AllBuffered, idPlayer, (Plot.plotDictionary[PLOT.PRISON] as PlotPrison).GetReleaseFee(player));
        }
        else
        {
            var client = PhotonNetwork.PlayerList.Single(x => x.UserId == idClient);
            photonView.RPC("_releaseWithMoneyFailClient", client, "Not enough money");
        }
    }

    [PunRPC]
    private void _releaseClient(int idPlayer)
    {
        Player player = TurnDirector.Ins.GetPlayer(idPlayer);

        releaseFunc(player);

        // TODO 
        // Remove the free release card from player's inventory


        ///////////////////////////////////////////////////////
    }

    [PunRPC]
    private void _releaseWithMoneyClient(int idPlayer, int money)
    {
        Player player = TurnDirector.Ins.GetPlayer(idPlayer);
        Bank.Ins.TakeMoney(player, money);

        if(_onReleaseSuccess != null)
        {
            _onReleaseSuccess();
        }


        releaseFunc(player);
    }

    [PunRPC]
    private void _releaseWithMoneyFailClient(string reason)
    {
        if(_onReleaseFail != null)
        {
            _onReleaseFail(reason);
        }
    }

    public void RequestRelease(Player player)
    {
        photonView.RPC("_releaseServer", RpcTarget.MasterClient, player.Id, PhotonNetwork.LocalPlayer.UserId);
    }


    public void RequestReleaseWithMoney(Player player, OnReleaseSuccess onSuccessCallback, OnReleaseFail onFailCallback)
    {
        _onReleaseSuccess = onSuccessCallback;
        _onReleaseFail = onFailCallback;

        photonView.RPC("_releaseWithMoneyServer", RpcTarget.MasterClient, player.Id, PhotonNetwork.LocalPlayer.UserId);
    }

    #endregion
    #endregion
}
