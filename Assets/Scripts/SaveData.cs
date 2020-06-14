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

        //load old player data
        PlayerData playerDataOld = LoadPlayerData();

        using (FileStream fileStream = new FileStream(path, FileMode.Create))
        {
            //create player intance
            PlayerData playerData = new PlayerData(highScore, selectedWeaponIndex);

            //check unlocked weapon and save just like old data

            if (playerDataOld != null)
            {
                playerData.missileLauncherAvailable = playerDataOld.missileLauncherAvailable;
                playerData.lightningGunAvailable = playerDataOld.lightningGunAvailable;
                playerData.grenadeLauncherAvailable = playerDataOld.grenadeLauncherAvailable;
            }

            //create serialize
            binaryFormatter.Serialize(fileStream, playerData);
            fileStream.Close();
        }
    }

    public static void ResetSave()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path, FileMode.Create);

        //create player intance
        PlayerData playerData = new PlayerData(0, 0);

        playerData.missileLauncherAvailable = false;
        playerData.lightningGunAvailable = false;
        playerData.grenadeLauncherAvailable = false;

        //create serialize
        binaryFormatter.Serialize(fileStream, playerData);
        fileStream.Close();
    }

    public static void SaveWeaponUnlocked(int weaponIndex, bool isAvailable)
    {
        PlayerData playerData = LoadPlayerData();
        if(playerData != null)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(path, FileMode.Create);

            //check whiich weapon
            if (weaponIndex == WeaponIndex.MISSILE_LAUNCHER)
            {
                playerData.missileLauncherAvailable = isAvailable;
            }
            else if (weaponIndex == WeaponIndex.LIGHTNING_GUN)
            {
                playerData.lightningGunAvailable = isAvailable;
            }
            else if (weaponIndex == WeaponIndex.GRENADE_LAUNCHER)
            {
                playerData.grenadeLauncherAvailable = isAvailable;
            }

            //create serialize
            binaryFormatter.Serialize(fileStream, playerData);
            fileStream.Close();
        }
    }

    public static bool LoadWeaponAvailabilityByWeaponIndex(int weaponIndex)
    {
        bool isAvailable = false;

        //check whiich weapon
        PlayerData playerData = LoadPlayerData();
        if(playerData != null)
        {
            if (weaponIndex == WeaponIndex.MISSILE_LAUNCHER)
            {
                isAvailable = playerData.missileLauncherAvailable;
            }
            else if (weaponIndex == WeaponIndex.LIGHTNING_GUN)
            {
                isAvailable = playerData.lightningGunAvailable;
            }
            else if (weaponIndex == WeaponIndex.GRENADE_LAUNCHER)
            {
                isAvailable = playerData.grenadeLauncherAvailable;
            }
        }

        return isAvailable;
    }

    public static PlayerData LoadPlayerData()
    {
        if (File.Exists(path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                PlayerData playerData = binaryFormatter.Deserialize(fileStream) as PlayerData;

                fileStream.Close();

                return playerData;
            }
        }
        else
        {
            //Debug.LogError("Save file not found: "+path);
            return null;
        }
    }
}
