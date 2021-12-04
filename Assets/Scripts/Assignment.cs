/*
This RPG data streaming assignment was created by Fernando Restituto.
Pixel RPG characters created by Sean Browning.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System.IO;

#region Assignment Instructions

/*  Hello!  Welcome to your first lab :)

Wax on, wax off.

    The development of saving and loading systems shares much in common with that of networked gameplay development.  
    Both involve developing around data which is packaged and passed into (or gotten from) a stream.  
    Thus, prior to attacking the problems of development for networked games, you will strengthen your abilities to develop solutions using the easier to work with HD saving/loading frameworks.

    Try to understand not just the framework tools, but also, 
    seek to familiarize yourself with how we are able to break data down, pass it into a stream and then rebuild it from another stream.


Lab Part 1

    Begin by exploring the UI elements that you are presented with upon hitting play.
    You can roll a new party, view party stats and hit a save and load button, both of which do nothing.
    You are challenged to create the functions that will save and load the party data which is being displayed on screen for you.

    Below, a SavePartyButtonPressed and a LoadPartyButtonPressed function are provided for you.
    Both are being called by the internal systems when the respective button is hit.
    You must code the save/load functionality.
    Access to Party Character data is provided via demo usage in the save and load functions.

    The PartyCharacter class members are defined as follows.  */

public partial class PartyCharacter
{
    public int classID;

    public int health;
    public int mana;

    public int strength;
    public int agility;
    public int wisdom;

    public LinkedList<int> equipment;

    public LinkedList<int> affliction;

    public LinkedList<int> otherThing;

    //public bool isAntiMasker = true;

}


/*
    Access to the on screen party data can be achieved via …..

    Once you have loaded party data from the HD, you can have it loaded on screen via …...

    These are the stream reader/writer that I want you to use.
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamwriter
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamreader

    Alright, that’s all you need to get started on the first part of this assignment, here are your functions, good luck and journey well!
*/


#endregion


#region Assignment Part 1

static public class AssignmentPart1
{

    const int PartyCharacterSaveDataSignifier = 0;
    const int EquipmentSaveDataSignifier = 1;
    const int AfflictionSaveDataSignifier = 2;


    static public void SavePartyButtonPressed()
    {

        //StreamWriter sw = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + "OurBelovedSaveFile.txt");

        //Debug.Log("start of loop");

        //foreach (PartyCharacter pc in GameContent.partyCharacters)
        //{

        //    sw.WriteLine(PartyCharacterSaveDataSignifier + "," + pc.classID + "," + pc.health 
        //    + "," + pc.mana + "," + pc.strength
        //    + "," + pc.agility + "," + pc.wisdom);

        //    //pc.equipment

        //    foreach(int equipID in pc.equipment)
        //    {
        //        sw.WriteLine(EquipmentSaveDataSignifier + "," + equipID);
        //    }


        //}

        //sw.Close();

        //Debug.Log("end of loop");
    }

    static public void LoadPartyButtonPressed()
    {


        string path = Application.dataPath + Path.DirectorySeparatorChar + "OurBelovedSaveFile.txt";

        if (File.Exists(path))
        {
            GameContent.partyCharacters.Clear();

            string line = "";
            StreamReader sr = new StreamReader(path);

            while ((line = sr.ReadLine()) != null)
            {
                string[] csv = line.Split(',');

                // foreach (string i in csv)
                //     Debug.Log(i);

                // Debug.Log(line);

                int saveDataSignifier = int.Parse(csv[0]);

                if (saveDataSignifier == PartyCharacterSaveDataSignifier)
                {
                    PartyCharacter pc = new PartyCharacter(int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3]),
                        int.Parse(csv[4]), int.Parse(csv[5]), int.Parse(csv[6]));

                    GameContent.partyCharacters.AddLast(pc);
                }
                else if (saveDataSignifier == EquipmentSaveDataSignifier)
                {
                    GameContent.partyCharacters.Last.Value.equipment.AddLast(int.Parse(csv[1]));
                    //GameContent.partyCharacters.equipment.Last.Value.AddLast(int.Parse(csv[1]))
                }
                else if (saveDataSignifier == AfflictionSaveDataSignifier)
                {
                    //load affliction data
                }



            }
        }
        GameContent.RefreshUI();
    }
}


#endregion


#region Assignment Part 2

static public class AssignmentConfiguration
{
    public const int PartOfAssignmentThatIsInDevelopment = 2;
}

static public class AssignmentPart2
{
    //const int PartyCharacterSaveDataSignifier = 0;
    //const int EquipmentSaveDataSignifier = 1;


    public const string PartyMetaFile = "PartyIndicesAndNames.txt"; // okay

    private static LinkedList<PartySaveData> parties;
    private static uint lastUsedIndex;

    static public void GameStart()
    {

        GameContent.RefreshUI();

        LoadPartyMetaData();
        
        Debug.Log("start");

    }

    static public List<string> GetListOfPartyNames()
    {
        if (parties == null)
            return new List<string>();

        List<string> pNames = new List<string>();

        foreach (PartySaveData psd in parties)
        {
            pNames.Add(psd.name);
        }

        return pNames;
    }

    static public void LoadPartyDropDownChanged(string selectedName)
    {

        foreach (PartySaveData psd in parties)
        {
            if(selectedName == psd.name)
                psd.LoadParty();
        }

        GameContent.RefreshUI();
        Debug.Log("l " + selectedName);

    }

    static public void SavePartyButtonPressed()
    {
        lastUsedIndex++;
        PartySaveData p = new PartySaveData(lastUsedIndex, GameContent.GetPartyNameFromInput());
        parties.AddLast(p);

        SavePartyMetaData();

        p.SaveParty();

        GameContent.RefreshUI();
        Debug.Log("s");

    }

    static public void NewPartyButtonPressed()
    {
        Debug.Log("n");
    }

    static public void DeletePartyButtonPressed()
    {
        Debug.Log("d");
    }

    static public void SavePartyMetaData()
    {
        StreamWriter sw = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + PartyMetaFile);


        sw.WriteLine("1," + lastUsedIndex);


        foreach (PartySaveData pData in parties)
        {
            sw.WriteLine("2," + pData.index + "," + pData.name);
        }

        sw.Close();

    }

    static public void LoadPartyMetaData()
    {
        parties = new LinkedList<PartySaveData>();

        string path = Application.dataPath + Path.DirectorySeparatorChar + PartyMetaFile;

        if (File.Exists(path))
        {
            string line = "";
            StreamReader sr = new StreamReader(path);

            while ((line = sr.ReadLine()) != null)
            {
                string[] csv = line.Split(',');

                //if(int.Parse(csv[0]))

                int saveDataSignifier = int.Parse(csv[0]);

                if (saveDataSignifier == 1)
                    lastUsedIndex = uint.Parse(csv[1]);
                else if (saveDataSignifier == 2)
                    parties.AddLast(new PartySaveData(uint.Parse(csv[1]), csv[2]));
            }
            sr.Close();
        }
    }
    
    static public void SendPartyDataToServer(NetworkedClient networkedClient)
    {
        // const int PartyCharacterSaveDataSignifier = 0;
        // const int EquipmentSaveDataSignifier = 1;

        LinkedList<string> data = DataManager.SerializedParty(GameContent.partyCharacters);
        
        // foreach (PartyCharacter pc in GameContent.partyCharacters)
        // {
        //     data.AddLast(PartyCharacterSaveDataSignifier + "," + pc.classID + "," + pc.health
        //                  + "," + pc.mana + "," + pc.strength
        //                  + "," + pc.agility + "," + pc.wisdom);
        //
        //     foreach (var equipID in pc.equipment)
        //     {
        //         data.AddLast(EquipmentSaveDataSignifier + "," + equipID);
        //     }
        // }
        
        networkedClient.SendMessageToHost(ClientToServerSignifiers.PartyDataTransferStart + "");

        foreach (string d in data)
            networkedClient.SendMessageToHost(ClientToServerSignifiers.PartyDataTransferStart + "," + d);

        networkedClient.SendMessageToHost(ClientToServerSignifiers.PartyDataTransferEnd + "");
    }

    static public void LoadPartyFromReceivedData(LinkedList<string> data)
    {
        GameContent.partyCharacters.Clear();
        GameContent.partyCharacters = DataManager.DeserializeParty(data);
        // const int PartyCharacterSaveDataSignifier = 0;
        // const int EquipmentSaveDataSignifier = 1;

        // foreach (string line in data)
        // {
        //     string[] csv = line.Split(',');
        //
        //     int saveDataSignifier = int.Parse(csv[0]);
        //
        //     if (saveDataSignifier == PartyCharacterSaveDataSignifier)
        //     {
        //         PartyCharacter pc = new PartyCharacter(int.Parse(csv[2]), int.Parse(csv[3]), int.Parse(csv[4]),
        //             int.Parse(csv[5]), int.Parse(csv[6]), int.Parse(csv[7]));
        //
        //         GameContent.partyCharacters.AddLast(pc);
        //     }
        //     else if (saveDataSignifier == EquipmentSaveDataSignifier)
        //     {
        //         GameContent.partyCharacters.Last.Value.equipment.AddLast(int.Parse(csv[2]));
        //         //GameContent.partyCharacters.equipment.Last.Value.AddLast(int.Parse(csv[1]))
        //     }
        // }
        GameContent.RefreshUI();
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
        
}
#endregion

class PartySaveData
{
    // const int PartyCharacterSaveDataSignifier = 0;
    // const int EquipmentSaveDataSignifier = 1;

    public uint index; // we need the extra numbers!
    public string name;

    public PartySaveData(uint index, string name)
    {
        this.index = index;
        this.name = name;
    }

    public void SaveParty()
    {
        StreamWriter sw = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + index + ".txt");

        Debug.Log("start of loop");

        LinkedList<string> data = DataManager.SerializedParty(GameContent.partyCharacters);

        foreach (string line in data)
        {
            sw.WriteLine(line);
        }
        
        // foreach (PartyCharacter pc in GameContent.partyCharacters)
        // {
        //     sw.WriteLine(PartyCharacterSaveDataSignifier + "," + pc.classID + "," + pc.health
        //     + "," + pc.mana + "," + pc.strength
        //     + "," + pc.agility + "," + pc.wisdom);
        //
        //     foreach (int equipID in pc.equipment)
        //     {
        //         sw.WriteLine(EquipmentSaveDataSignifier + "," + equipID);
        //     }
        // }
        
        sw.Close();
        Debug.Log("end of loop");
    }

    public void LoadParty()
    {
        string path = Application.dataPath + Path.DirectorySeparatorChar + index + ".txt";

        if (File.Exists(path))
        {
            GameContent.partyCharacters.Clear();

            string line = "";
            StreamReader sr = new StreamReader(path);
            LinkedList<string> data = new LinkedList<string>();
            
            while ((line = sr.ReadLine()) != null) 
                data.AddLast(line);
            
            sr.Close();
            
            GameContent.partyCharacters = DataManager.DeserializeParty(data);

            // string line = "";
            // StreamReader sr = new StreamReader(path);

            // while ((line = sr.ReadLine()) != null)
            // {
            //     string[] csv = line.Split(',');
            //
            //     int saveDataSignifier = int.Parse(csv[0]);
            //
            //     if (saveDataSignifier == PartyCharacterSaveDataSignifier)
            //     {
            //         PartyCharacter pc = new PartyCharacter(int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3]),
            //             int.Parse(csv[4]), int.Parse(csv[5]), int.Parse(csv[6]));
            //
            //         GameContent.partyCharacters.AddLast(pc);
            //     }
            //     else if (saveDataSignifier == EquipmentSaveDataSignifier)
            //     {
            //         GameContent.partyCharacters.Last.Value.equipment.AddLast(int.Parse(csv[1]));
            //         //GameContent.partyCharacters.equipment.Last.Value.AddLast(int.Parse(csv[1]))
            //     }
            // }
            //sr.Close();
        }
        GameContent.RefreshUI();
    }
}

static public class DataManager
{
    const int PartyCharacterSaveDataSignifier = 0;
    const int EquipmentSaveDataSignifier = 1;
    
    static public LinkedList<string> SerializedParty(LinkedList<PartyCharacter> party)
    {
        LinkedList<string> data = new LinkedList<string>();
        foreach (PartyCharacter pc in GameContent.partyCharacters)
        {
            data.AddLast(PartyCharacterSaveDataSignifier + "," + pc.classID + "," + pc.health
                          + "," + pc.mana + "," + pc.strength
                          + "," + pc.agility + "," + pc.wisdom);

            foreach (int equipID in pc.equipment)
            {
                data.AddLast(EquipmentSaveDataSignifier + "," + equipID);
            }
        }
        
        return data;
    }

    static public LinkedList<PartyCharacter> DeserializeParty(LinkedList<string> data)
    {
        LinkedList<PartyCharacter> party = new LinkedList<PartyCharacter>();

        foreach (string line in data)
        {
            string[] csv = line.Split(',');

            int saveDataSignifier = int.Parse(csv[0]);

            if (saveDataSignifier == PartyCharacterSaveDataSignifier)
            {
                PartyCharacter pc = new PartyCharacter(int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3]),
                    int.Parse(csv[4]), int.Parse(csv[5]), int.Parse(csv[6]));

               party.AddLast(pc);
            }
            else if (saveDataSignifier == EquipmentSaveDataSignifier)
            {
                party.Last.Value.equipment.AddLast(int.Parse(csv[1]));
                //GameContent.partyCharacters.equipment.Last.Value.AddLast(int.Parse(csv[1]))
            }
        }
        
        return party;
    }
}