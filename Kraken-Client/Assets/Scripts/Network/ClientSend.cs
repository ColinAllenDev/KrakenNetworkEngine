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
    public static void PlayerMovement(bool[] _inputs) {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement)) {
            _packet.Write(_inputs.Length);
            foreach(bool _input in _inputs) {
                _packet.Write(_input);
            }
            _packet.Write(GameManager.players[Client.instance.myId].transform.rotation);

            SendUDPData(_packet);
        }
    }
    
    /// <summary> Sends player forward facing direction to the server </summary>
    /// <param name="_facing"> The player's forward facing direction</param>
    public static void PlayerShoot(Vector3 _facing) {
        using(Packet _packet = new Packet((int)ClientPackets.playerShoot)) {
            _packet.Write(_facing);

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
