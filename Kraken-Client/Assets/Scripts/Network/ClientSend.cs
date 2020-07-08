using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet) {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet) {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets

    /// <summary> Lets the server know we received the welcome message </summary>
    public static void WelcomeReceived() {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived)) {
            _packet.Write(Client.instance.myId);
            _packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(_packet);
        }
    }

    /// <summary> Sends player movement data to the server </summary>
    /// <param name="_inputs"> The array of inputs from our InputManager</param>
<<<<<<< HEAD
    public static void PlayerMovement(bool[] _inputs) {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement)) {
            _packet.Write(_inputs.Length);
            foreach(bool _input in _inputs) {
                _packet.Write(_input);
            }
=======
    public static void PlayerMovement(bool[] _inputs, float[] _axes) {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement)) {
            // Write packet lengths
            _packet.Write(_inputs.Length);
            _packet.Write(_axes.Length);

            // Write keyboard inputs
            foreach(bool _input in _inputs) {
                _packet.Write(_input);
            }

            // Write keyboard axes
            foreach(float _axe in _axes) {
                _packet.Write(_axe);
            }

            // Write player rotation
>>>>>>> develop
            _packet.Write(GameManager.players[Client.instance.myId].transform.rotation);

            SendUDPData(_packet);
        }
    }
    
    /// <summary> Sends player forward facing direction to the server </summary>
    /// <param name="_facing"> The player's forward facing direction</param>
<<<<<<< HEAD
    public static void PlayerShoot(Vector3 _facing) {
        using(Packet _packet = new Packet((int)ClientPackets.playerShoot)) {
            _packet.Write(_facing);
=======
    public static void PlayerShoot(Vector3 _facing, float _damage) {
        using(Packet _packet = new Packet((int)ClientPackets.playerShoot)) {
            _packet.Write(_facing);
            _packet.Write(_damage);

            SendTCPData(_packet);
        }
    }

    /// <summary> Sends player forward facing direction to the server </summary>
    /// <param name="_facing"> The player's forward facing direction</param>
    public static void PlayerThrowItem(Vector3 _facing) {
        using(Packet _packet = new Packet((int)ClientPackets.playerThrowItem)) {
            _packet.Write(_facing);
>>>>>>> develop

            SendTCPData(_packet);
        }
    }

    /// <summary> Sends ping packet to server (RTT) </summary>
    public static void Ping() {
        using (Packet _packet = new Packet((int)ClientPackets.ping)) {
            _packet.Write(Time.time);
            SendTCPData(_packet);
        }
    }

    #endregion

}
