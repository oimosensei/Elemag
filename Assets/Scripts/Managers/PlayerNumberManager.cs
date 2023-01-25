using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNumberManager : MonoBehaviour
{
    private static PlayerNumberManager instance;

    public static PlayerNumberManager Instance
    {
        get
        {
            //nullチェック
            if (null == instance)
            {
                instance = (PlayerNumberManager)FindObjectOfType(typeof(PlayerNumberManager));
                if (null == instance)
                {
                    Debug.Log(" PlayerNumberManager Instance Error ");
                }
            }
            return instance;
        }
    }
    public int playerNumber = 0;
    void Awake()
    {
        int a = PlayerNumberManager.Instance.playerNumber;
        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        playerNumber = PlayerPrefs.GetInt("PlayerNumber") + 1;
        PlayerPrefs.SetInt("PlayerNumber", playerNumber);
        DontDestroyOnLoad(gameObject);
        Debug.Log("PlayerNumber: " + playerNumber);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetPlayerNumber()
    {
        PlayerPrefs.SetInt("PlayerNumber", 0);
        playerNumber = 0;
    }
}
