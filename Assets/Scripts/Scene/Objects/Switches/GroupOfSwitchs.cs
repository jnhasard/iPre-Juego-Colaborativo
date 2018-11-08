using System.Collections.Generic;

public class GroupOfSwitchs
{

    #region Attributes 

    private List<Switch> switches;

    public int groupId;
    bool activated;

    #endregion

    #region Constructor

    public GroupOfSwitchs(int _groupId)
    {
        switches = new List<Switch>();
        groupId = _groupId;
    }

    #endregion

    #region Common

    public void AddSwitch(Switch switchi)
    {
        switchi.switchGroup = this;
        switches.Add(switchi);
    }

    public Switch GetSwitch(int id)
    {
        foreach (Switch switchi in switches)
        {
            if (switchi.individualId == id)
            {
                return switchi;
            }
        }
        return null;
    }

    public void CallAction()
    {
        if (activated)
        {
            return;
        }

        activated = true;
        SwitchActions handler = new SwitchActions(this);
        handler.DoSomething();
        SendSwitchesGroupReadyToServer();
    }

    public void CheckIfReady(PlannerSwitch switchObj, Planner planner)
    {
        foreach (Switch switchi in switches)
        {
            if (!switchi.isActivated)
            {
                return;
            }
        }

        CallAction();

        if (switchObj != null)
        {
            switchObj.ActivateSwitch();
            planner.Monitor();
        }
    }

    #endregion

    #region Utils

    public List<Switch> GetSwitchs()
    {
        return switches;
    }

    #endregion

    #region Messaging

    private void SendSwitchesGroupReadyToServer()
    {
        string message = "SwitchGroupReady/" + groupId;

        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, true);
        }
    }

    #endregion

}
