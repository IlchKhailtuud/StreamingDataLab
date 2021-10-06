
/*
This RPG data streaming assignment was created by Fernando Restituto.
Pixel RPG characters created by Sean Browning.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


#region Assignment Instructions
public partial class PartyCharacter
{
    public int classID;

    public int health;
    public int mana;

    public int strength;
    public int agility;
    public int wisdom;

    public LinkedList<int> equipment;
}

#endregion

#region Assignment Part 1

static public class AssignmentPart1
{
    const int PartyCharacterSaveDataSignifier = 0;
    const int EquipmentSaveDataSignifier = 1; 
    
    static public void SavePartyButtonPressed()
    {
        Debug.Log("SavePartyBtnPressed");
        StreamWriter sw = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + "Our BelovedSaveFile.txt");

        foreach (PartyCharacter pc in GameContent.partyCharacters) 
        {
            sw.WriteLine(PartyCharacterSaveDataSignifier + "," + pc.classID + "," + pc.health + "," + pc.mana + "," + pc.strength + "," + pc.agility + "," + pc.wisdom);

            foreach (int equipID in pc.equipment)
            {
                sw.WriteLine(EquipmentSaveDataSignifier + " , " + equipID);
            }
        }
        
        sw.Close();
    }

    static public void LoadPartyButtonPressed()
    {
        if (File.Exists(Application.dataPath + Path.DirectorySeparatorChar + "Our BelovedSaveFile.txt"))
        {
            GameContent.partyCharacters.Clear();
            
            string line = "";
            StreamReader sr = new StreamReader(Application.dataPath + Path.DirectorySeparatorChar + "Our BelovedSaveFile.txt");
            
            
            while ((line = sr.ReadLine()) != null)
            {
                string[] csv = line.Split(',');

                int saveDataSignifier = int.Parse(csv[0]);

                if (saveDataSignifier == PartyCharacterSaveDataSignifier)
                {
                    PartyCharacter pc = new PartyCharacter(int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3]), int.Parse(csv[4]), int.Parse(csv[5]), int.Parse(csv[6]));
                    GameContent.partyCharacters.AddLast(pc);
                }
                else if (saveDataSignifier == EquipmentSaveDataSignifier)
                {
                    GameContent.partyCharacters.Last.Value.equipment.AddLast(int.Parse(csv[1]));
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
    const int PartyCharacterSaveDataSignifier = 0;
    const int PartyCharacterEquipmentSaveDataSignifier = 1;
    
    const int LastUsedIndexSignifier = 1;
    const int IndexAndNameSignifier = 2;
    
    static int lastIndexUsed;
    
    static List<string> partyNames;

    private static LinkedList<NameAndIndex> nameAndIndices;

    const string IndexFilePath = "indices.txt";
    
    
    static public void GameStart()
    {
        Debug.Log("Game Start");

        nameAndIndices = new LinkedList<NameAndIndex>();

        if (File.Exists(Application.dataPath + Path.DirectorySeparatorChar + IndexFilePath))
        {
            StreamReader sr = new StreamReader(Application.dataPath + Path.DirectorySeparatorChar + IndexFilePath);

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                Debug.Log(line);
                string[] csv = line.Split(',');
                int signifier = int.Parse(csv[0]);

                if (signifier == LastUsedIndexSignifier)
                {
                    lastIndexUsed = int.Parse(csv[1]);
                }
                else if (signifier == IndexAndNameSignifier)
                {
                    nameAndIndices.AddLast(new NameAndIndex(int.Parse(csv[1]), csv[2]));
                }
            }
        }

        partyNames = new List<string>();


        foreach(NameAndIndex nameAndIndex in nameAndIndices)
        {
            partyNames.Add(nameAndIndex.name);
        }

        GameContent.RefreshUI();

    }

    static public List<string> GetListOfPartyNames()
    {
        return partyNames;
    }

    static public void LoadPartyDropDownChanged(string selectedName)
    {
        GameContent.partyCharacters.Clear();

        int indexToLoad = -1;

        foreach(NameAndIndex nameAndIndex in nameAndIndices)
        {
            if(nameAndIndex.name == selectedName)
                indexToLoad = nameAndIndex.index;
        }

        StreamReader sr = new StreamReader(Application.dataPath + Path.DirectorySeparatorChar + indexToLoad + ".txt");

        string line;
        while ((line = sr.ReadLine()) != null)
        {
            Debug.Log(line);

            string[] csv = line.Split(',');

            int signifier = int.Parse(csv[0]);

            if (signifier == PartyCharacterSaveDataSignifier)
            {
                PartyCharacter pc = new PartyCharacter(int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3]), int.Parse(csv[4]), int.Parse(csv[5]), int.Parse(csv[6]));
                GameContent.partyCharacters.AddLast(pc);
            }
            else if (signifier == PartyCharacterEquipmentSaveDataSignifier)
            {
                GameContent.partyCharacters.Last.Value.equipment.AddLast(int.Parse(csv[1]));
            }
        }

        GameContent.RefreshUI();
    }

    static public void SavePartyButtonPressed()
    {
        bool isUniqueName = true;

        foreach (NameAndIndex nameAndIndex in nameAndIndices)
        {
            if (nameAndIndex.name == GameContent.GetPartyNameFromInput())
            {
                SaveParty(Application.dataPath + Path.DirectorySeparatorChar + nameAndIndex.index + ".txt");
                isUniqueName = false;
            }
        }

        if (isUniqueName)
        {
            lastIndexUsed++;
            SaveParty(Application.dataPath + Path.DirectorySeparatorChar + lastIndexUsed + ".txt");
            nameAndIndices.AddLast(new NameAndIndex(lastIndexUsed, GameContent.GetPartyNameFromInput()));
        }


        GameContent.RefreshUI();
        Debug.Log("saving");


        SaveIndexManagementFile();
    }

    static public void NewPartyButtonPressed()
    {
        Debug.Log("Create party created");
        SaveIndexManagementFile();
    }

    static public void DeletePartyButtonPressed()
    {
        Debug.Log("Party deleted");
    }
    
    static public void SaveIndexManagementFile()
    {

        StreamWriter sw = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + IndexFilePath);

        sw.WriteLine(LastUsedIndexSignifier + "," + lastIndexUsed);

        foreach (NameAndIndex nameAndIndex in nameAndIndices)
        {
            sw.WriteLine(IndexAndNameSignifier + "," + nameAndIndex.index + "," + nameAndIndex.name);
        }

        sw.Close();

    }

    static public void SaveParty(string fileName)
    {
        StreamWriter sw = new StreamWriter(fileName);

        foreach (PartyCharacter pc in GameContent.partyCharacters)
        {
            sw.WriteLine(PartyCharacterSaveDataSignifier + "," + pc.classID + "," + pc.health
                         + "," + pc.mana + "," + pc.strength + "," + pc.agility + "," + pc.wisdom);

            foreach (int equip in pc.equipment)
            {
                sw.WriteLine(PartyCharacterEquipmentSaveDataSignifier + "," + equip);
            }
        }
        sw.Close();
    }
}

public class NameAndIndex
{
    public string name;
    public int index;

    public NameAndIndex(int Index, string Name)
    {
        name = Name;
        index = Index;
    }
}

#endregion