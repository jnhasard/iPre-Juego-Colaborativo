
using System;

public class RoomHpMp
{

    #region Attributes

    public int currentExp;  //TODO: Variable Global EXP
    public float maxHP;
    public float maxMP;
    public float maxExp;
    public float currentHP;
    public float currentMP;
    public float percentageHP;
    public float percentageMP;
    public float percentageExp;
    private bool mpAtLimit;

    Room room;

    #endregion

    #region Constructor

    public RoomHpMp(Room room)
    {
        this.room = room;
        maxHP = 250;
        maxMP = 250;
        //maxExp = 250;
        currentHP = maxHP;
        currentMP = maxMP;
        currentExp = 0;
        percentageHP = 1;
        percentageMP = 1;
        percentageExp = 0;
    }

    #endregion

    #region Common

    public void StopChangeHpAndMpHUD(string ip)
    {
        room.SendMessageToAllPlayersExceptOne("DisplayStopChangeHPMPToClient/", ip, false);
    }

    public void RecieveHpAndMpHUD(string changeRate, string ip)
    {
        ChangeHP(changeRate, ip);
        ChangeMP(changeRate, ip);
    }

    public void ChangeHP(string deltaHP, string ip)
    {
        float valueDeltaHP = float.Parse(deltaHP);
        if (valueDeltaHP == 0)
        {
            return;
        }
        currentHP += valueDeltaHP;

        if (currentHP >= maxHP)
        {
            currentHP = maxHP;
        }
        else if (currentHP <= 0)
        {
            currentHP = 0;
            room.SendMessageToAllPlayers("PlayersAreDead/" + Server.instance.sceneToLoad,false);
            room.log.WritePlayersAreDead();
            currentHP = maxHP;
            currentMP = maxMP;
            room.SendMessageToAllPlayers("NewChatMessage/" + room.actualChat,false);
        }

        percentageHP = currentHP / maxHP;
        room.SendMessageToAllPlayersExceptOne("DisplayChangeHPToClient/" + percentageHP, ip, false);
    }

    public void ChangeMaxHP(string NewMaxHP, string ip)
    {
        maxHP = float.Parse(NewMaxHP);
        ChangeHP(NewMaxHP, ip);
    }

    public void ChangeMP(string deltaMP, string ip)
    {
        float valueDeltaMP = float.Parse(deltaMP);

        currentMP += valueDeltaMP;

        if (currentMP > maxMP)
        {
            currentMP = maxMP;
        }
        else if (currentMP <= 0)
        {
            currentMP = 0;
        }

        percentageMP = currentMP / maxMP;

        if (percentageMP == 1 || percentageMP == 0)
        {
            if (mpAtLimit)
            {
                return;
            }
            mpAtLimit = true;
        }
        else
        {
            mpAtLimit = false;
        }

        room.SendMessageToAllPlayersExceptOne("DisplayChangeMPToClient/" + percentageMP, ip, false);
    }

    public void ChangeMaxMP(string NewMaxMP, string ip)
    {
        maxMP = float.Parse(NewMaxMP);
        ChangeMP(NewMaxMP, ip);
    }

    public void ChangeExp(string deltaExp)
    {
        int valueDeltaExp = Int32.Parse(deltaExp);
        currentExp += valueDeltaExp;

        room.SendMessageToAllPlayers("DisplayChangeExpToClient/" + currentExp, true);
    }

    public float GetExp()
    {
        return currentExp;
    }

    /* public void ChangeExp(string deltaExp)
    {
        float valueDeltaExp = float.Parse(deltaExp);
        currentExp += valueDeltaExp;

        if (currentExp >= maxExp)
        {
            currentExp = 0;
            // levelUp
        }

        percentageExp = currentExp / maxExp;
        room.SendMessageToAllPlayers("DisplayChangeExpToClient/" + percentageExp, false);
    }

    public void ChangeMaxExp(string NewMaxExp)
    {
        maxExp = float.Parse(NewMaxExp);
        ChangeExp(NewMaxExp);
    } */
    
    #endregion

}
