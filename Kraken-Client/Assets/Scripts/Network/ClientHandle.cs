using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet) {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void Ping(Packet _packet) {
        float RTT = Time.time - _packet.ReadFloat();
        int _tick = (int)(_packet.ReadInt() + (RTT / 2));
        GameLogic.instance.RTT = RTT;
        GameLogic.instance.tick = _tick;
        UIManager.instance.DisplayDebug();
        Debug.Log($"Ping: {(RTT * 1000).ToString("f0")}ms");
    }

    public static void SpawnPlayer(Packet _packet) {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
    }

    public static void PlayerPosition(Packet _packet) {
        int _id = _packet.ReadInt();
        Vector3 _serverPosition = _packet.ReadVector3();

        if(GameManager.players.TryGetValue(_id, out PlayerManager _player)) {
            // Player movement logic
            GameManager.players[_id].GetComponent<Interpolator>().NewUpdate(GameLogic.instance.tick, _serverPosition);   
        }
    }

    public static void PlayerRotation(Packet _packet) {
        int _id = _packet.ReadInt();
        Quaternion _serverRotation = _packet.ReadQuaternion();

        if(GameManager.players.TryGetValue(_id, out PlayerManager _player)) {
            // Player rotation logic
            GameManager.players[_id].GetComponent<Interpolator>().NewUpdate(GameLogic.instance.tick, _serverRotation);
        }
    }

    public static void PlayerDisconnected(Packet _packet) {
        int _id = _packet.ReadInt();
        Destroy(GameManager.players[_id].gameObject);
        GameManager.players.Remove(_id);
    }

    public static void PlayerHealth(Packet _packet) {
        int _id = _packet.ReadInt();
        float _health = _packet.ReadFloat();

        GameManager.players[_id].SetHealth(_health);
    }

    public static void PlayerRespawned(Packet _packet) {
        int _id = _packet.ReadInt();

        GameManager.players[_id].Respawn();
    }

    public static void CreateItemSpawner(Packet _packet) {
        int _spawnerId = _packet.ReadInt();
        Vector3 _spawnerPosition = _packet.ReadVector3();
        bool _hasItem = _packet.ReadBool();

        GameManager.instance.CreateItemSpawner(_spawnerId, _spawnerPosition, _hasItem);
    }

    public static void ItemSpawned(Packet _packet) {
        int _spawnerId = _packet.ReadInt();

        GameManager.itemSpawners[_spawnerId].ItemSpawned();
    }

    public static void ItemPickedUp(Packet _packet) {
        int _spawnerId = _packet.ReadInt();
        int _byPlayer = _packet.ReadInt();

        GameManager.itemSpawners[_spawnerId].ItemPickedUp();
        GameManager.players[_byPlayer].itemCount++;
    }
}
