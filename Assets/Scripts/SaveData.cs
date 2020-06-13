using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveData
{
    //save path
    public static string path = Application.persistentDataPath + "/player.rem";

    public static void Save(int highScore, int selectedWeaponIndex)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path, FileMode.Create);

        //create player intance
        PlayerData playerData = new PlayerData(highScore, selectedWeaponIndex);

        //create serialize
        binaryFormatter.Serialize(fileStream, playerData);
        fileStream.Close();
    }

    public static PlayerData LoadPlayerData()
    {
        if (File.Exists(path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(path, FileMode.Open);

            //create serialize
            PlayerData playerData = binaryFormatter.Deserialize(fileStream) as PlayerData;
            fileStream.Close();

            return playerData;
        }
        else
        {
            //Debug.LogError("Save file not found: "+path);
            return null;
        }
    }
}
