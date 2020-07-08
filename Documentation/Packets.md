# Packets
Packets are basically small amounts of data sent over a network (LAN or the internet). 
Each packet sent includes the source it was sent from, a destination, and the data being sent.
In the Kraken Engine, packets are sent using your choice of the TCP or UDP protocols.

## Adding a New Packet
The process of adding a new packet is relatively simple, but if you forget a certain step, you're gonna run into weird behavior.
Though the process is virtually the same for client and server, I will document the process for both.

### Sending Packets from the Client to the Server
The first step in creating a new packet is adding it to the relevant enum in the **Packet** class.
On the client-side, go into the **Packet** class and add your packet name to the **ClientPackets** enum.
When adding packets that are sent from the client to the server, you must add your packet to the **ClientPackets** enum.
When adding packets that are sent from the server to the client, you must add your packet to the **ServerPackets** enum.
**(NOTE: Make sure to note the index of the packet you added to the enum. These must be the _exact_ same on both the client and the server.)**
**(ANOTHER NOTE: As you'll notice, the first packet in the list (in our case, the "welcome" packet) is equal to 1. This means the enum index starts at 1 instead of zero. You can change this if you want.)**

```
/// <summary>Sent from client to server.</summary>
public enum ClientPackets
{
    welcomeReceived = 1,
    playerMovement,
    playerShoot,
    playerThrowItem,
    ping,
    //  <--- Your new packet goes here (or anywhere in the list)
}
```

The next step is to add our packet method to our **ClientSend** class. In the parameters field, add any data you want to send.
Inside our method, we define our Packet instance inside a `using` block. This will dispose of our Packet when we're done with it.
A common use-case for sending data from the client to the server is sending input, rotation, local events, etc. In this example we'll just 
send a simple message.
```
public static void ClientMessage() {
    using(Packet _packet = new Packet((int)ClientPackets.your_packet_name)) {
        _packet.Wrtie("Hello Server!);
        SendTCPData(_packet); // You can choose TCP or UDP here
    }	
}
```

Now go into your server project's code. Open the `Packet` class and the same packet you added to the **ClientPackets** enum on the client-side, to the server's **ClientPackets** enum.
**(NOTE: It is VERY IMPORTANT that you add the packet at the _exact same_ index that you did in on the client-side. Otherwise you'll run into all kinds of weird issues.)**

```
/// <summary>Sent from client to server.</summary>
public enum ClientPackets
{
    welcomeReceived = 1,
    playerMovement,
    playerShoot,
    playerThrowItem,
    ping,
    newClientPacket //<--- Your new packet goes in the exact same spot it did on the client!!
}
```

Now let's move over to our **ServerHandle** class. Add a static method with two parameters: `int _fromClient` and `Packet _packet`. 
These are pretty self explanitory; _fromClient is the client id that sent the packet, and _packet is, of course, the packet.
To read the data we sent, we can simply use `_packet.ReadString()`. In cases of other variable types, check the Packet class to see what can
be read by the server.
```
public static void ReadMessage(int _fromClient, Packet _packet) {
    float _clientMessage = _packet.ReadString();
    Debug.Log($"Client {_fromClient} Sent Message: {_clientMessage}); 
}
```

Lastly, we need to add the packet to our packetHandlers Dictionary on our server. In our server code, open your `Server` class and scroll down to the `InitializeServerData()` method.
Inside is a Dictionary called **packetHandlers**, here we add the client packet from our **ClientPackets** enum as well as it's corresponding **ServerHandle** method.
Add the following line at the same index it was in the **ClientPackets** enum.

```
{(int) ClientPackets.newClientPacket, ServerHandle.ReadMessage}
```

And that's it! You have succesfully sent a packet from the client to the server!

### Sending a Packets from the Server to the Client
I'm gonna work on a longer explanation of this one, but for now just do what you did on the client but on the server-side.

