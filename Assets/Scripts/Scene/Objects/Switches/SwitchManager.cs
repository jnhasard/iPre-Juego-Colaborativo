using System.Collections.Generic;
using UnityEngine;

public class SwitchManager : MonoBehaviour
{

    #region Attributes

    List<GroupOfSwitchs> listOfGroups;

    #region Sprites

    #region Shoot switches

    public Sprite ShootBlueOn;
    public Sprite ShootBlueOff;
    public Sprite ShootRedOn;
    public Sprite ShootRedOff;
    public Sprite ShootYellowOn;
    public Sprite ShootYellowOff;
    public Sprite ShootAnyOn;
    public Sprite ShootAnyOff;

    #endregion

    #region Step switches

    public Sprite StepBlueOn;
    public Sprite StepBlueOff;
    public Sprite StepRedOn;
    public Sprite StepRedOff;
    public Sprite StepYellowOn;
    public Sprite StepYellowOff;
    public Sprite StepAnyOn;
    public Sprite StepAnyOff;

    #endregion

    #endregion

    #endregion

    #region Awake

    void Awake()
    {
        listOfGroups = new List<GroupOfSwitchs>();
    }

    #endregion

    #region Common

    public Switch GetSwitch(int groupId, int individualId)
    {
        foreach (GroupOfSwitchs group in listOfGroups)
        {
            if (group.groupId == groupId)
            {
                return group.GetSwitch(individualId);
            }
        }
        return null;
    }

    public void Add(Switch switchi)
    {
        GroupOfSwitchs group = GetGroup(switchi.groupId);
        group.AddSwitch(switchi);
    }

    private GroupOfSwitchs GetGroup(int id)
    {
        foreach (GroupOfSwitchs group in listOfGroups)
        {
            if (group.groupId == id)
            {
                return group;
            }
        }
        return NewGroup(id);
    }

    private GroupOfSwitchs NewGroup(int id)
    {
        GroupOfSwitchs group = new GroupOfSwitchs(id);
        listOfGroups.Add(group);
        return group;
    }

    public void CallAction(int groupId)
    {
        foreach (GroupOfSwitchs group in listOfGroups)
        {
            if (group.groupId == groupId)
            {
                group.CallAction();
            }
        }
    }

    #endregion

}
