using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    private int highScore;
    private int selectedWeaponIndex;
    public bool missileLauncherAvailable = false;
    public bool lightningGunAvailable = false;
    public bool grenadeLauncherAvailable = false;

    public PlayerData(
        int highScore, 
        int selectedWeaponIndex
        )
    {
        this.highScore = highScore;
        this.selectedWeaponIndex = selectedWeaponIndex;
    }

    public void SetHighScore(int newHighScore)
    {
        this.highScore = newHighScore;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    public int GetSelectedWeaponIndex()
    {
        return selectedWeaponIndex;
    }
}
