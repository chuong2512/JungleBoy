using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuHomeScene : MonoBehaviour
{
    public static MainMenuHomeScene Instance;

    public GameObject StartMenu;
    public GameObject WorldsChoose;
    public GameObject LoadingScreen;
    public GameObject LevelsChoose;
    public GameObject CharacterChoose;
    public GameObject[] WorldLevel;

    SoundManager soundManager;
    public Button[] rewardedAdButton;


    void Awake()
    {
        Instance = this;
        soundManager = FindObjectOfType<SoundManager>();
    }

    // Use this for initialization
    void Start()
    {
        StartMenu.SetActive(true);
        WorldsChoose.SetActive(false);
        LevelsChoose.SetActive(false);
        LoadingScreen.SetActive(true);
        CharacterChoose.SetActive(false);
    }

    private void Update()
    {
        foreach (var but in rewardedAdButton)
        {
/*#if UNITY_ANDROID || UNITY_IOS
            but.interactable = (AdsManager.Instance && AdsManager.Instance.isRewardedAdReady());
#else
            but.gameObject.SetActive(false);
#endif*/
        }
    }

    public void WatchVideoAd()
    {
/*#if UNITY_ANDROID || UNITY_IOS
        AdsManager.AdResult += AdsManager_AdResult;
        AdsManager.Instance.ShowRewardedAds();
#endif*/
    }

    private void AdsManager_AdResult(bool isSuccess, int rewarded)
    {
#if UNITY_ANDROID || UNITY_IOS
        /*AdsManager.AdResult -= AdsManager_AdResult;*/
        GlobalValue.SavedCoins += rewarded;
        SoundManager.PlaySfx(SoundManager.Instance.soundReward);
#endif
    }

    public void OpenWorld(int world)
    {
        //WorldsChoose.SetActive (false);
        LevelsChoose.SetActive(true);

        for (int i = 0; i < WorldLevel.Length; i++)
        {
            if (i == (world - 1))
            {
                WorldLevel[i].SetActive(true);
            }
            else
                WorldLevel[i].SetActive(false);
        }

        SoundManager.PlaySfx(soundManager.soundClick);
    }

    public void OpenWorldChoose()
    {
        StartMenu.SetActive(false);
        WorldsChoose.SetActive(true);
        LevelsChoose.SetActive(false);

        SoundManager.PlaySfx(soundManager.soundClick);
    }

    public void OpenStartMenu()
    {
        StartMenu.SetActive(true);
        WorldsChoose.SetActive(false);
        CharacterChoose.SetActive(false);

        SoundManager.PlaySfx(soundManager.soundClick);
    }

    public void OpenCharacterChoose()
    {
        StartMenu.SetActive(false);
        CharacterChoose.SetActive(true);


        SoundManager.PlaySfx(soundManager.soundClick);
    }
}