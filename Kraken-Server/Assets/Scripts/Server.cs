using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server
{
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }

    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
    public static Dictionary<int, PacketHandler> packetHandlers;
    public delegate void PacketHandler(int _fromClient, Packet _packet);

    private static TcpListener tcpListener;
    private static UdpClient udpListenter;


    public static void Start(int _maxPlayers, int _port) {
        MaxPlayers = _maxPlayers;
        Port = _port;

        Debug.Log("Starting server...");
        InitializeServerData();

        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        udpListenter = new UdpClient(Port);
        udpListenter.BeginReceive(UDPReceiveCallback, null);

        Debug.Log($"Server started on {Port}.");
    }

    private static void TCPConnectCallback(IAsyncResult _result) {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
        Debug.Log($"Incoming connection from {_client.Client.RemoteEndPoint}...");

        for (int i = 1; i <= MaxPlayers; i++) {
            if (clients[i].tcp.socket == null) {
                clients[i].tcp.Connect(_client);
                return;
            }
        }
        Debug.Log($"{_client.Client.RemoteEndPoint} failed to connect: Server is full!");
    }

    private static void UDPReceiveCallback(IAsyncResult _result) {
        try {
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListenter.EndReceive(_result, ref _clientEndPoint);
            udpListenter.BeginReceive(UDPReceiveCallback, null);

            if (_data.Length < 4) return;
            
            using (Packet _packet = new Packet(_data)) {
                int _clientId = _packet.ReadInt();
                if (_clientId == 0) return;

                if (clients[_clientId].udp.endPoint == null) {
                    clients[_clientId].udp.Connect(_clientEndPoint);
                    return;
                }

                if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString()) {
                    clients[_clientId].udp.HandleData(_packet);
                }
            }
        } catch (Exception _ex) {
            Debug.Log($"Error receiving UDP data: {_ex}");
        }
    }

    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet) {
        try {
            if (_clientEndPoint != null) {
                udpListenter.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
        } catch (Exception _ex) {
            Debug.Log($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
        }
    }

    private static void InitializeServerData() {
        for (int i = 1; i <= MaxPlayers; i++)
        {
            clients.Add(i, new Client(i));
        }

        packetHandlers = new Dictionary<int, PacketHandler>() {
            {(int) ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
            {(int) ClientPackets.ping, ServerHandle.Ping},
            {(int) ClientPackets.playerMovement, ServerHandle.PlayerMovement },
            {(int) ClientPackets.playerShoot, ServerHandle.PlayerShoot},
            {(int) ClientPackets.playerThrowItem, ServerHandle.PlayerThrowItem},
        };

        Debug.Log("Initialized packets.");
    }

    public static void Stop() {
        tcpListener.Stop();
        udpListenter.Close();
    }
}
