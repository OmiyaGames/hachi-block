using UnityEngine;
using System.Collections;

public class GameSettings : ISingletonScript
{
    public const int NumLevels = 7;
    public const int MenuLevel = 0;
    public const int CreditsLevel = NumLevels;
    public const string NumLevelsUnlockedKey = "numLevelsUnlocked";

	public bool simulateWebplayer = false;
    
    int mNumLevelsUnlocked = 1;

	public bool IsWebplayer
	{
		get
		{
			return (simulateWebplayer == true) || (Application.isWebPlayer == true);
		}
	}

    public int NumLevelsUnlocked
    {
        get
        {
            return mNumLevelsUnlocked;
        }
        set
        {
            mNumLevelsUnlocked = Mathf.Clamp(value, 1, NumLevels);
        }
    }
	
	public override void SingletonStart(Singleton instance)
	{
		RetrieveFromSettings();
	}
	
    public override void SceneStart(Singleton instance)
	{
	}

    public void RetrieveFromSettings()
    {
        NumLevelsUnlocked = PlayerPrefs.GetInt(NumLevelsUnlockedKey, 1);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt(NumLevelsUnlockedKey, NumLevelsUnlocked);
        PlayerPrefs.Save();
    }

    public void ClearSettings()
    {
        NumLevelsUnlocked = 1;
        PlayerPrefs.DeleteAll();
    }
}
