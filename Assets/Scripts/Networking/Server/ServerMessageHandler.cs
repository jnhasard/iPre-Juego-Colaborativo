using UnityEngine;
using System.Collections;
using System;
using System.Globalization;


public class ServerMessageHandler
{
    Server server;

    public ServerMessageHandler(Server server)
    {
        this.server = server;
    }

    public void HandleMessage(string message, string ip)
    {
        char[] separator = new char[1] { '/' };
        string[] msg = message.Split(separator);

        switch (msg[0])
        {
            case "ChangeScene":
                HandleChangeScene(msg, ip);
                break;
            case "ObjectMoved":
                SendObjectMoved(message, ip);
                break;
            case "ObjectDestroyed":
                SendObjectDestroyed(message, msg, ip);
                break;
            case "ObstacleDestroyed":
                HandleObstacleDestroyed(msg, ip);
                break;
            case "ChangeObjectPosition":
                SendUpdatedObjectPosition(message, ip);
                break;
            case "InstantiateObject":
                SendInstantiation(message, ip);
                break;
            case "NewChatMessage":
                SendNewChatMessage(message, ip);
                break;
            case "ChangeHpHUDToRoom":
                SendHpHUDToRoom(msg, ip);
                break;
            case "ChangeMpHUDToRoom":
                SendMpHUDToRoom(msg, ip);
                break;
            case "StopChangeHpAndMpHUDToRoom":
                StopChangeHPMpHUDToRoom(msg, ip);
                break;
            case "ChangeHpAndMpHUDToRoom": //Necessary coz' ChatZone changes both at the same rate
                SendHpHAndMpHUDToRoom(msg, ip);
                break;
            case "EnteredChatZone":
                PlayerEnteredChatZone(message, ip);
                break;
            case "LeftChatZone":
                SendPlayerLeftChatZoneSignal(message, ip);
                break;
            case "GainExp":
                SendExpToRoom(msg, ip);
                break;
            case "EnemyRegisterId":
                NewEnemy(msg, ip);
                break;
            case "EnemyDied":
                EnemyDied(message, msg, ip);
                break;
            case "EnemyChangePosition":
                EnemyChangePosition(message, msg, ip);
                break;
            case "EnemyPatrollingPoint":
                SendEnemyPatrollingPoint(message, msg, ip);
                break;
            case "EnemiesStartPatrolling":
                EnemiesStartPatrolling(ip);
                break;
            case "PlayerRequestId":
                SendAllData(ip, Server.instance.GetPlayer(ip).room); //Manda todo para manejar mejor reconexiones. Inclusive información de playerId.
                break;
            case "PlayerAttack":
                SendAttackState(message, ip, msg);
                break;
            case "PlayerPower":
                SendPowerState(message, ip, msg);
                break;
            case "PlayerChangePosition":
                SendUpdatedPosition(message, ip, msg);
                break;
            case "PlayerChangePositionForNewScene":
                UpdatePositionForNewScene(message, ip, msg);
                break;
            case "PlayerTookDamage":
                SendPlayerTookDamage(message, ip);
                break;
            case "PlayerVote":
                SendPlayerVoted(message, ip);
                break;
            case "PlayerPreVote":
                SendPlayerPreVoted(message, ip);
                break;
            case "CreateGameObject":
                SendNewGameObject(message, ip);
                break;
            case "DestroyObject":
                SendDestroyObject(message, ip);
                break;
            case "InventoryUpdate":
                SendInventoryUpdate(message, ip);
                break;
            case "ChangeSwitchStatus":
                SendChangeSwitchStatus(message, msg, ip);
                break;
            case "SwitchGroupReady":
                SendSwitchGroupAction(message, msg, ip);
                break;
            case "ActivateSystem":
                SendActivateSystem(message, ip, msg);
                break;
            case "ActivateNPCLog": // No se si es necesario o no, ya que puedes llamar el metodo desde afuera (start o script)
                SendActivationNPC(msg, ip);
                break;
            case "IgnoreCollisionBetweenObjects":
                SendIgnoreCollisionBetweenObjects(message, ip);
                break;
            case "BubbleInstantiatorData":
                SendBubbleInstantiatorDaTa(message, ip);
                break;
            case "CoordinateRotators":
                SendRotatorsData(message, ip);
                break;
            case "CoordinateInstantiators":
                SyncPlatformInstantiators(message, ip);
                break;
            case "CoordinateMovingObject":
                SyncMovingObjects(message, ip);
                break;
            case "MustInstantiateAndDestroy":
                SyncMovableTriggers(message, ip);
                break;
            case "EnterPOI":
                HandleEnterPOI(msg, ip);
                break;
            case "EnterButDontCare":
                HandleEnterButDontCare(msg, ip);
                break;
            case "ReadyPoi":
                HandleReadyPoi(msg, ip);
                break;
            case "IsThisExpEnough":
                HandleExpQuestion(msg, ip);
                break;
            case "WhichMusicShloudIPlay":
                SendSceneNameForMusic(message, ip);
                break;
            case "CoordinatePlayerTeleporter":
                SendTeleporterCoordination(message, ip);
                break;
            case "CoordinatePlayerId":
                SendPlayerIdCoordination(message, ip);
                break;
            default:
                break;
        }
    }


    private void SendPlayerIdCoordination(string msg, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayers(msg, true);
    }

    private void HandleReadyPoi(string[] msg, string ip)
    {
        string poiID = msg[1];
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        RoomLogger log = player.room.log;
        log.WritePoiIsReady(player.id, poiID);

        room.poisHandler.AddPoiReady(poiID);
        room.SendMessageToAllPlayersExceptOne("ReadyPoi" + "/" + poiID, ip, true);

    }

    private void HandleEnterPOI(string[] msg, string ip)
    {
        string poiID = msg[1].ToString();
        string incomingPlayer = msg[2].ToString();

        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        RoomLogger log = player.room.log;

        log.WriteEnterPOI(player.id, poiID);

        room.SendMessageToAllPlayersExceptOne("PoiReached" + "/" + poiID + "/" + incomingPlayer, ip, true);
    }

    private void HandleEnterButDontCare(string[] msg, string ip)
    {
        string poiID = msg[1].ToString();
        string incomingPlayer = msg[2].ToString();

        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        RoomLogger log = player.room.log;

        log.WriteEnterPOIButDontCare(player.id, poiID);
    }

    private void HandleChangeScene(string[] msg, string ip)
    {
        string scence = msg[1];

        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        int totalExp = room.hpManager.currentExp;
        RoomLogger log = room.log;
        log.WriteTotalExp(totalExp);
        log.WriteChangeScene(scence);

        SendChangeScene(scence, room);
    }


    private void EnemiesStartPatrolling(string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.EnemiesStartPatrolling();
    }

    //Usado para sincronizar estado del servidor con un cliente que se está reconectando
    public void SendAllData(string ip, Room room)
    {
        SendPlayerIdAndControl(ip);
        foreach (NetworkPlayer player in room.players)
        {
            room.SendMessageToPlayer(player.GetReconnectData(), ip, true);
        }

        foreach (RoomSwitch switchi in room.switchs)
        {
            room.SendMessageToPlayer(switchi.GetReconnectData(), ip, true);
        }

        foreach (string doorMessage in room.systemsManager.GetSystemsMessages())
        {
            room.SendMessageToPlayer(doorMessage, ip, true);
        }

        foreach (string obstacleMessage in room.obstacleManager.GetObstaclesMessages())
        {
            room.SendMessageToPlayer(obstacleMessage, ip, true);
        }

        foreach (string objectMessage in room.objectManager.GetObjectMessages())
        {
            room.SendMessageToPlayer(objectMessage, ip, true);
        }

        foreach (string poiMessages in room.poisHandler.GetPoiMessages())
        {
            room.SendMessageToPlayer(poiMessages, ip, true);
        }

    }

    private void SendIgnoreCollisionBetweenObjects(string message, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayers(message, true);
    }

    public void SendActivationNPC(string[] msg, string ip) // Manda un mensaje a un solo jugador
    {
        string message = msg[1];
        int playerId = int.Parse(msg[2]);
        string newIpAddress = "";

        Room room = server.GetPlayer(ip).room;

        foreach (NetworkPlayer jugador in room.players)
        {
            if (playerId == jugador.id)
            {
                newIpAddress = jugador.ipAddress;
                break;
            }
        }

        if (!message.Contains("ActivateNPCLog"))
        {
            message = "ActivateNPCLog/" + message;
        }

        server.NPCsLastMessage = message;
        room.SendMessageToPlayer(message, newIpAddress, true); // Message es el texto a mostrar en el NPC Log
        room.WriteFeedbackRecord(message + "/" + playerId);
    }

    private void SendActivateSystem(string message, string ip, string[] msg)
    {
        string systemName = msg[1];

        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;

        room.SendMessageToAllPlayersExceptOne(message, ip, true);
        room.systemsManager.AddSystem(systemName);
    }

    private void HandleObstacleDestroyed(string[] msg, string ip)
    {
        string obstacleName = msg[1];

        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;

        room.obstacleManager.AddObstacle(obstacleName);
    }

    private void SendSwitchGroupAction(string message, string[] msg, string ip)
    {
        // OBSOLETO <- por qué?
        int groupId = Int32.Parse(msg[1]);

        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;

        if (!room.activatedSwitchGroups.Contains(groupId))
        {
            room.activatedSwitchGroups.Add(groupId);
        }
    }

    private void SendChangeSwitchStatus(string message, string[] msg, string ip)
    {
        int groupId = Int32.Parse(msg[1]);
        int individualId = Int32.Parse(msg[2]);
        bool on = bool.Parse(msg[3]);

        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;

        room.SetSwitchOn(on, groupId, individualId);
        room.SendMessageToAllPlayers(message, true);
    }

    private void EnemyChangePosition(string message, string[] msg, string ip)
    {
        int enemyId = Int32.Parse(msg[1]);
        int directionX = Int32.Parse(msg[2]);
        float posX = float.Parse(msg[3]);
        float posY = float.Parse(msg[4]);

        NetworkPlayer player = server.GetPlayer(ip);
        NetworkEnemy enemy = player.room.GetEnemy(enemyId);

        if (enemy != null)
        {
            enemy.SetPosition(directionX, posX, posY);
            player.room.SendMessageToAllPlayersExceptOne(message, ip, false);
        }
    }

    private void SendEnemyPatrollingPoint(string message, string[] msg, string ip)
    {
        int enemyId = Int32.Parse(msg[1]);
        int directionX = Int32.Parse(msg[2]);
        float posX = float.Parse(msg[3]);
        float posY = float.Parse(msg[4]);
        float patrolX = float.Parse(msg[5]);
        float patrolY = float.Parse(msg[6]);

        NetworkPlayer player = server.GetPlayer(ip);
        NetworkEnemy enemy = player.room.GetEnemy(enemyId);

        if (enemy != null)
        {
            enemy.SetPatrollingPoint(directionX, posX, posY, patrolX, patrolY);
            player.room.SendMessageToAllPlayersExceptOne(message, ip, true);
        }
    }

    private void EnemyDied(string message, string[] msg, string ip)
    {
        int enemyId = Int32.Parse(msg[1]);

        NetworkPlayer player = server.GetPlayer(ip);
        NetworkEnemy enemy = player.room.GetEnemy(enemyId);

        if (enemy != null)
        {
            enemy.Die();
        }
    }

    private void NewEnemy(string[] msg, string ip)
    {
        int instanceId = Int32.Parse(msg[1]);
        int id = Int32.Parse(msg[2]);
        float hp = float.Parse(msg[3]);

        int directionX = Int32.Parse(msg[4]);
        float posX = float.Parse(msg[5]);
        float posY = float.Parse(msg[6]);

        string message = "EnemyRegistered/" + instanceId + "/" + id + "/" + directionX + "/" + posX + "/" + posY;

        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        NetworkEnemy enemy = room.AddEnemy(instanceId, id, hp); ;

        enemy.SetPosition(directionX, posX, posY);

        if (msg.Length >= 9)
        {
            float patrolX = float.Parse(msg[7]);
            float patrolY = float.Parse(msg[8]);

            message += ("/" + patrolX + "/" + patrolY);
            enemy.SetPatrollingPoint(directionX, posX, posY, patrolX, patrolY);
        }

        room.SendMessageToAllPlayers(message, true);
    }

    private void SendNewGameObject(string message, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayers(message + "/" + player.id, true);
    }

    private void SendInventoryUpdate(string message, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        player.InventoryUpdate(message);
        player.room.log.WriteInventory(player.id, message);
    }

    private void SendTeleporterCoordination(string message, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayers(message, true);
    }

    private void SendDestroyObject(string message, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayers(message, true);
    }

    private void SendOthersDestroyObject(string message, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayersExceptOne(message, ip, true);
    }

    private void SendPlayerVoted(string message, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayersExceptOne(message, ip, true);
    }

    private void SendPlayerPreVoted(string message, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayersExceptOne(message, ip, true);
    }

    private void SendHpHUDToRoom(string[] msg, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.hpManager.ChangeHP(msg[1], ip);
    }

    private void SendMpHUDToRoom(string[] msg, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.hpManager.ChangeMP(msg[1], ip);
    }


    private void StopChangeHPMpHUDToRoom(string[] msg, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.hpManager.StopChangeHpAndMpHUD(ip);
    }

    private void SendHpHAndMpHUDToRoom(string[] msg, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.hpManager.RecieveHpAndMpHUD(msg[1], ip);
    }

    private void PlayerEnteredChatZone(string msg, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        RoomLogger log = room.log;
        log.WritePlayerIsCharging(player.id);
    }

    private void SendPlayerLeftChatZoneSignal(string msg, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        RoomLogger log = room.log;
        log.WritePlayernotCharging(player.id);
    }

    private void SendExpToRoom(string[] msg, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.hpManager.ChangeExp(msg[1]);
    }

    private void HandleExpQuestion(string[] msg, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        int exp = room.hpManager.currentExp;
        room.SendMessageToAllPlayers("ExpAnswer/" + exp + "/" + msg[1], true);
    }

    private void SendNewFireball(string message, string ip, string[] data)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayersExceptOne(message, ip, false);
    }

    private void SendNewProjectile(string message, string ip, string[] data)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayersExceptOne(message, ip, false);
    }

    private void SendNewChatMessage(string chatMessage, string ip)
    {

        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayers(chatMessage, false);
    }

    private void SendPlayerTookDamage(string message, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayersExceptOne(message, ip, false);
    }

    private void SendUpdatedPosition(string message, string ip, string[] data)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;

        int charId = Int32.Parse(data[1]);
        float positionX = float.Parse(data[2]);
        float positionY = float.Parse(data[3]);
        int directionX = Int32.Parse(data[4]);
        int directionY = Int32.Parse(data[5]);
        float speedX = float.Parse(data[6]);
        bool isGrounded = bool.Parse(data[7]);
        bool pressingJump = bool.Parse(data[8]);
        bool pressingLeft = bool.Parse(data[9]);
        bool pressingRight = bool.Parse(data[10]);

        player.positionX = positionX;
        player.positionY = positionY;
        player.directionX = directionX;
        player.directionY = directionY;
        player.speedX = speedX;
        player.isGrounded = isGrounded;
        player.pressingJump = pressingJump;
        player.pressingLeft = pressingLeft;
        player.pressingRight = pressingRight;

        room.SendMessageToAllPlayersExceptOne(message, ip, false);
        room.log.WriteNewPosition(player.id, positionX, positionY, pressingJump, pressingLeft, pressingRight);
    }

    private void UpdatePositionForNewScene(string message, string ip, string[] data)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        float [] coordenadas = player.room.GetStartPosition();

        int charId = Int32.Parse(data[1]);
        float positionX = coordenadas[0]; //Cambiar Aquí 
        float positionY = coordenadas[1]; 
        int directionX = Int32.Parse(data[2]);
        int directionY = Int32.Parse(data[3]);
        float speedX = float.Parse(data[4]);
        bool isGrounded = bool.Parse(data[5]);
        bool pressingJump = bool.Parse(data[6]);
        bool pressingLeft = bool.Parse(data[7]);
        bool pressingRight = bool.Parse(data[8]);

        player.positionX = positionX;
        player.positionY = positionY;
        player.directionX = directionX;
        player.directionY = directionY;
        player.speedX = speedX;
        player.isGrounded = isGrounded;
        player.pressingJump = pressingJump;
        player.pressingLeft = pressingLeft;
        player.pressingRight = pressingRight;

        room.SendMessageToAllPlayers(message, false);
        room.log.WriteNewPosition(player.id, positionX, positionY, pressingJump, pressingLeft, pressingRight);
    }

    private void SendObjectMoved(string message, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayersExceptOne(message, ip, true);
    }

    private void SendObjectDestroyed(string theMessage, string[] message, string ip)
    {
        string objectName = message[1];
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.objectManager.AddObjectDestroyed(objectName);
        room.SendMessageToAllPlayersExceptOne(theMessage, ip, true);
    }

    private void SendUpdatedObjectPosition(string message, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayersExceptOne(message, ip, false);
    }

    private void SendInstantiation(string message, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayers(message, true);
    }

    private void SendPlayerIdAndControl(string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);

        string message = "PlayerSetId/" + player.id + "/" + player.controlOverEnemies;

        server.SendMessageToClient(player.connectionId, message, true);
    }

    private void SendBubbleInstantiatorDaTa(string message, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayers(message, true);
    }

    private void SendRotatorsData(string message, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayers(message, true);
    }

    private void SyncMovableTriggers(string message, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayers(message, true);
    }

    private void SyncPlatformInstantiators(string message, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayers(message, true);
    }

    private void SyncMovingObjects(string message, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayers(message, true);
    }

    public void SendChangeScene(string sceneName, Room room)
    {
        string message = "ChangeScene/" + sceneName;
        room.sceneToLoad = sceneName;
        room.SendMessageToAllPlayers(message, true);
        room.Reset();
    }

    public void SendSceneNameForMusic(string message, string ip)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        string sceneName = room.sceneToLoad;
        room.SendMessageToPlayer("SceneNameAnswerForMusic" + "/" + sceneName , ip, true);
    }

    public void SendAttackState(string message, string ip, string[] data)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        room.SendMessageToAllPlayersExceptOne(message, ip, false);
        room.log.WriteAttack(player.id);
    }

    public void SendPowerState(string message, string ip, string[] data)
    {
        NetworkPlayer player = server.GetPlayer(ip);
        Room room = player.room;
        player.power = bool.Parse(data[2]);
        room.SendMessageToAllPlayersExceptOne(message, ip, false);
        room.log.WritePower(player.id, player.power);
    }
}
