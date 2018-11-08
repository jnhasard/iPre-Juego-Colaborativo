using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkEnemy
{

    #region Attributes

    public float patrollingPointX;
    public float patrollingPointY;
    public float positionX;
    public float positionY;
    public bool fromEditor;
    public int directionX;
    public int instanceId;
    public float hp;
    public int id;

    public LevelManager levelManager;
    public Room room;

    #endregion

    #region Constructor

    public NetworkEnemy(int instanceId, int enemyId, float hp, Room room)
    {
        this.instanceId = instanceId;
        this.id = enemyId;
        this.room = room;
        this.hp = hp;
    }

    #endregion

    #region Common

    public void SetPosition(int directionX, float positionX, float positionY)
    {
        this.directionX = directionX;
        this.positionX = positionX;
        this.positionY = positionY;
    }

    public void SetPatrollingPoint(int directionX, float positionX, float positionY, float patrollingPointX, float patrollingPointY)
    {
        SetPosition(directionX, positionX, positionY);
        this.patrollingPointX = patrollingPointX;
        this.patrollingPointY = patrollingPointY;
    }

    public void Die()
    {
        room.RemoveEnemy(this);
    }

    #endregion

    #region Utils

    private bool IsDead()
    {
        return hp <= 0;
    }

    #endregion

}
