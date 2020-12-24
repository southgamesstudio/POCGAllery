
using UnityEngine;

public class MultiplayerSettings : MonoBehaviour
{
    public static MultiplayerSettings multiplayerSettings;

    //sync all players 
    public bool delayStart;
    public int maxPlayers;

    public int menuScene;
    public int multiplayerScene;

    private void Awake()
    {
        if (MultiplayerSettings.multiplayerSettings == null)
        {
            MultiplayerSettings.multiplayerSettings = this;
        }
        else
        {
            if (MultiplayerSettings.multiplayerSettings != this)
                {
                    Destroy(this.gameObject);
                }
        }

        DontDestroyOnLoad(this.gameObject);
    }


}
