using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySharingManger : MonoBehaviour
{
    public GameObject sharePartyButton, sharingRoomNameInputField, joinSharingRoomButton;

    private NetworkedClient networkedClinet;
    
    // Start is called before the first frame update
    void Start()
    {
        sharePartyButton.GetComponent<Button>().onClick.AddListener(SharePartyButtonPressed);
        joinSharingRoomButton.GetComponent<Button>().onClick.AddListener(JoinSharingRoomButtonPressed);

        networkedClinet = GetComponent<NetworkedClient>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SharePartyButtonPressed()
    {
        //AssignmentPart2.SendPartyDataToServer();
    }

    public void JoinSharingRoomButtonPressed()
    {
        string name = sharingRoomNameInputField.GetComponent<InputField>().text;
        networkedClinet.SendMessageToHost(ClientToServerSignifiers.JoinSharingRoom + name);
    }
    
    public static class ClientToServerSignifiers
    {
        public const int JoinSharingRoom = 1;
    }

    public static class ServerToClientSignifiers
    {
        
    }
}
