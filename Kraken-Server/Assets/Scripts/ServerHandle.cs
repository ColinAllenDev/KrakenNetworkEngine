using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet) {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"Player \"{_username}\" (ip: {Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint}) connected successfully and is now Player {_fromClient}");
        if (_fromClient != _clientIdCheck) {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }
        Server.clients[_fromClient].SendIntoGame(_username);
    }

    public static void PlayerMovement(int _fromClient, Packet _packet) {
        // Create input arrays
        bool[] _inputs = new bool[_packet.ReadInt()];
        float[] _axes = new float[_packet.ReadInt()];

        // Read keyboard inputs
        for (int i = 0; i < _inputs.Length; i++) {
            _inputs[i] = _packet.ReadBool();
        }

        // Read keyboard axes
        for(int i = 0; i < _axes.Length; i++) {
            _axes[i] = _packet.ReadFloat();
        }

        // Read rotation
        Quaternion _rotation = _packet.ReadQuaternion();
        
        Server.clients[_fromClient].player.SetInput(_inputs, _axes, _rotation);
    }

    public static void PlayerShoot(int _fromClient, Packet _packet) {
        Vector3 _shootDirection = _packet.ReadVector3();
        Server.clients[_fromClient].player.Shoot(_shootDirection);
    }

    public static void PlayerThrowItem(int _fromClient, Packet _packet) {
        Vector3 _throwDirection = _packet.ReadVector3();

        Server.clients[_fromClient].player.ThrowItem(_throwDirection);
    }

    public static void Ping(int _fromClient, Packet _packet) {
        float _clientTime = _packet.ReadFloat();
        Server.clients[_fromClient].SendPing(_clientTime, _fromClient);
    }

}
