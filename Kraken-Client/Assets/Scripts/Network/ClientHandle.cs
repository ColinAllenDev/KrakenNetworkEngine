using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    /// <summary> Handles welcome message packet from the server and prints it to console.
    /// It assigns the client's instance ID and then connects to thhe server. </summary>
    /// <param name="_packet"> The received message packet</param>
    public static void Welcome(Packet _packet) {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    /// <summary> Handles a message packet from the server and prints it to the console </summary>
    /// <param name="_packet"> The received message packet </param>
    public static void Message(Packet _packet) {
        string _msg = _packet.ReadString();

        Debug.Log($"Server: {_msg}");
    }

    /// <summary> Handles the ping packet recieved and updates the client tickrate </summary>
    /// <param name="_packet"> The received ping packet</param>
    public static void Ping(Packet _packet) {
        float RTT = Time.time - _packet.ReadFloat();
        int _tick = (int)(_packet.ReadInt() + (RTT / 2));
        GameLogic.instance.RTT = RTT;
        GameLogic.instance.tick = _tick;
    }

    /// <summary> Handles the player packet received from the server and spawns the player </summary>
    /// <param name="_packet"> The received id, username, position and rotation packet</param>
    public static void SpawnPlayer(Packet _packet) {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
    }

    /// <summary> Handles the disconnection from the server</summary>
    /// <param name="_packet"> The player id packet</param>
    public static void PlayerDisconnected(Packet _packet) {
        int _id = _packet.ReadInt();
        Destroy(GameManager.players[_id].gameObject);
        GameManager.players.Remove(_id);
    }

    /// <summary> Handles the player position packet and updates the it's position </summary>
    /// <param name="_packet"> The server position packet</param>
    public static void PlayerPosition(Packet _packet) {
        int _id = _packet.ReadInt();
        Vector3 _serverPosition = _packet.ReadVector3();

        if(GameManager.players.TryGetValue(_id, out PlayerManager _player)) {
            // Player movement logic
            _player.GetComponent<Interpolator>().NewUpdate(GameLogic.instance.tick, _serverPosition);   
        }
    }

    /// <summary> Handles the player rotation packet and updates the it's rotation </summary>
    /// <param name="_packet"> The server rotation packet</param>
    public static void PlayerRotation(Packet _packet) {
        int _id = _packet.ReadInt();
        Quaternion _playerRotation = _packet.ReadQuaternion();

        if(GameManager.players.TryGetValue(_id, out PlayerManager _player)) {
            // Player rotation logic - TODO: Interpolate and handle chest rotation
            _player.transform.rotation = _playerRotation;
        }
    }

    /// <summary> Handles the player velocity packet</summary>
    /// <param name="_packet"> The server velocity packet</param>
    public static void PlayerVelocity(Packet _packet) {
        int _id = _packet.ReadInt();
        Vector3 _serverVelocity = _packet.ReadVector3();
        
        GameManager.players[_id].SetVelocity(_serverVelocity);
    }

    /// <summary> Handles the health of the player from the server</summary>
    /// <param name="_packet"> The player id and health packets</param>
    public static void PlayerHealth(Packet _packet) {
        int _id = _packet.ReadInt();
        float _health = _packet.ReadFloat();

        GameManager.players[_id].SetHealth(_health);
    }

    /// <summary> Handles the respawn event sent by the server</summary>
    /// <param name="_packet"> The player id packet</param>
    public static void PlayerRespawned(Packet _packet) {
        int _id = _packet.ReadInt();

        GameManager.players[_id].Respawn();
    }

    /// <summary> Handles the death of a player from the server </summary>
    /// <param name="_packet"> The received death packet </param>
    public static void PlayerDied(Packet _packet) {
        // Read the id and username of the player killed
        int _playerId = _packet.ReadInt();
        string _playerUsername = _packet.ReadString();

        // Read the id and username of the player's killer
        int _killerId = _packet.ReadInt();
        string _killerUsername = _packet.ReadString();

        // Log the kill
        Debug.Log($"Player {_playerUsername} was killed by {_killerUsername}");

        // Display death screen on killed player's client
        if(Client.instance.myId == _playerId) {
            UIManager.instance.DisplayDeathText(_killerUsername);
        }
    }

    public static void PlayerLoadout(Packet _packet) {
        int _playerId = _packet.ReadInt();
        string _primary = _packet.ReadString();
        string _secondary = _packet.ReadString();
        string _melee = _packet.ReadString();

        if(GameManager.players.TryGetValue(_playerId, out PlayerManager _player)) {
            Loadout _loadout = _player.GetComponent<Loadout>();
            _loadout.SetLoadout(_primary, _secondary, _melee);
        }
    } 

    /// <summary> Handles the item spawner(s) received from the server</summary>
    /// <param name="_packet"> The spawner id, position, and hasItem packets</param>
    public static void CreateItemSpawner(Packet _packet) {
        int _spawnerId = _packet.ReadInt();
        Vector3 _spawnerPosition = _packet.ReadVector3();
        bool _hasItem = _packet.ReadBool();

        GameManager.instance.CreateItemSpawner(_spawnerId, _spawnerPosition, _hasItem);
    }

    /// <summary> Handles the itemSpawned event sent by the server</summary>
    /// <param name="_packet"> The spawner id packet</param>
    public static void ItemSpawned(Packet _packet) {
        int _spawnerId = _packet.ReadInt();

        GameManager.itemSpawners[_spawnerId].ItemSpawned();
    }

    /// <summary> Handles the item picked up event sent by the server</summary>
    /// <param name="_packet"> The spawner id and player id packets</param>
    public static void ItemPickedUp(Packet _packet) {
        int _spawnerId = _packet.ReadInt();
        int _byPlayer = _packet.ReadInt();

        GameManager.itemSpawners[_spawnerId].ItemPickedUp();
        GameManager.players[_byPlayer].itemCount++;
    }

    /// <summary> Handles the projectile spawning event sent by the server</summary>
    /// <param name="_packet"> The projectile id, position, and player id packet</param>
    public static void SpawnProjectile(Packet _packet) {
        int _projectileId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        int _thrownByPlayer = _packet.ReadInt();

        GameManager.instance.SpawnProjectile(_projectileId, _position);
        GameManager.players[_thrownByPlayer].itemCount--;
    }
    
    /// <summary> Handles the projectile position sent by the server</summary>
    /// <param name="_packet"> The projectile id and position packets</param>
    public static void ProjectilePosition(Packet _packet) {
        int _projectileId = _packet.ReadInt();
        Vector3 _serverPosition = _packet.ReadVector3();;

        GameManager.projectiles[_projectileId].GetComponent<Interpolator>().NewUpdate(GameLogic.instance.tick, _serverPosition);   
    }

    /// <summary> Handles the projectile explosion event sent by the server</summary>
    /// <param name="_packet"> The projectile id and position packets</param>
    public static void ProjectileExploded(Packet _packet) {
        int _projectileId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();;

        GameManager.projectiles[_projectileId].Explode(_position);
    }


}
