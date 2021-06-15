using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Birger_JeweryDeal : BaseSkill
{
    [SerializeField] StatusJeweryDeal _statusPrefab;

    public Skill_Birger_JeweryDeal()
    {
        Set(
            name: "Jewery deal",
            manacost: 5,
            currentmanacost: 5,
            description: "In the next TWO ROUND, each time you go into OTHER PLAYER'S MARKET PLOT, get 200 GOLD and DO NOT pay rent, otherwise, LOSE 100 GOLD."
            );
    }

    public override bool CanActivate()
    {
        if (Owner.GetMana() >= CurrentManaCost)
            return true;
        return base.CanActivate();
    }

    public override bool Activate()
    {
        if (CanActivate())
        {
            var status = Instantiate(_statusPrefab, Owner.transform);
            //status.targetPlayer = Owner;
            status.StartListen();
            return base.Activate();
        }
        return false;
    }
}