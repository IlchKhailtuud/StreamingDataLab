using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.UI;

public class NetworkedServer : MonoBehaviour
{
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    
    private LinkedList<SharingRoom> sharingRooms;

    // Start is called before the first frame update
    void Start()
    {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        reliableChannelID = config.AddChannel(QosType.Reliable);
        unreliableChannelID = config.AddChannel(QosType.Unreliable);
        HostTopology topology = new HostTopology(config, maxConnections);
        hostID = NetworkTransport.AddHost(topology, socketPort, null);
        sharingRooms = new LinkedList<SharingRoom>();
    }

    // Update is called once per frame
    void Update()
    {

        int recHostID;
        int recConnectionID;
        int recChannelID;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error = 0;

        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

        switch (recNetworkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log("Connection, " + recConnectionID);
                break;
            case NetworkEventType.DataEvent:
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                ProcessReceivedMsg(msg, recConnectionID);
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("Disconnection, " + recConnectionID);
                PlayerDisconnected(recConnectionID);
                break;
        }

    }
  
    public void SendMessageToClient(string msg, int id)
    {
        byte error = 0;
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, id, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }
    
    private void ProcessReceivedMsg(string msg, int id)
    {
        Debug.Log("msg received = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split();

        int signifier = int.Parse(csv[0]);

        if (signifier == ClientToServerSignifiers.JoinSharingRoom)
        {
            string roomToJoinName = csv[1];

            SharingRoom foundSharingRoom = null;
            
            foreach (var sr in sharingRooms )
            {
                if (sr.name == roomToJoinName)
                {
                    foundSharingRoom = sr;
                    sr.connectionIDs.AddLast(id);
                    Debug.Log("Added to sharing room");
                    break;
                }
            }

            if (foundSharingRoom == null)
            {
                foundSharingRoom = new SharingRoom();
                foundSharingRoom.name = roomToJoinName;
                foundSharingRoom.connectionIDs.AddLast(id);
                sharingRooms.AddLast(foundSharingRoom);
                Debug.Log("Created sharing room");
            }
            
            //what for?
            if (!foundSharingRoom.connectionIDs.Contains(id))
                foundSharingRoom.connectionIDs.AddLast(id);
            else
                Debug.Log("Prevent duplicate in sharing room");
        }
        
        else if (signifier == ClientToServerSignifiers.PartyDataTransferStart)
        {
            SharingRoom sr = FindSharingRoomWithConnectionID(id);
            sr.transferData = new LinkedList<string>();
            
        }
        else if (signifier == ClientToServerSignifiers.PartyDataTransfer)
        {
            SharingRoom sr = FindSharingRoomWithConnectionID(id);
            sr.transferData.AddLast(msg);
        }
        else if (signifier == ClientToServerSignifiers.PartyDataTransferEnd)
        {
            SharingRoom sr = FindSharingRoomWithConnectionID(id);

            foreach (var pID in sr.connectionIDs)
            {
                if (pID == id)
                    continue;
                
                SendMessageToClient(ServerToClientSignifiers.PartyDataTransferStart + "", pID);
                
                foreach (string d in sr.transferData )
                {
                    SendMessageToClient(d, pID);
                }
                
                SendMessageToClient(ServerToClientSignifiers.PartyDataTransferEnd + "", pID);
            }
        }
    }

    public void PlayerDisconnected(int id)
    {
        SharingRoom foundSR = FindSharingRoomWithConnectionID(id);
        
        if (foundSR != null) 
        {
            foundSR.connectionIDs.Remove(id);
            Debug.Log("Removing player from room");
            
            if (foundSR.connectionIDs.Count == 0)
            {
                sharingRooms.Remove(foundSR);
                Debug.Log("Removing room from list");
            }
        }
    }

    public SharingRoom FindSharingRoomWithConnectionID(int id)
    {
        foreach (var sr in sharingRooms)
        {
            foreach (var pID in sr.connectionIDs)
            {
                if (pID == id)
                {
                    return sr;
                }
            }
        }
        return null;
    }

    public class SharingRoom
    {
        public string name;
        public LinkedList<int> connectionIDs;
        public LinkedList<string> transferData;

        public SharingRoom()
        {
            connectionIDs = new LinkedList<int>();
        }
    }

    public static class ClientToServerSignifiers
    {
        public const int JoinSharingRoom = 1;
        
        public const int PartyDataTransferStart = 101;
        public const int PartyDataTransfer = 102;
        public const int PartyDataTransferEnd = 103;
    }

    public static class ServerToClientSignifiers
    {
        public const int PartyDataTransferStart = 101;
        public const int PartyDataTransfer = 102;
        public const int PartyDataTransferEnd = 103;
    }
}