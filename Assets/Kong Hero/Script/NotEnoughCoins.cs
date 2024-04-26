using UnityEngine;
using System.Collections;

public class NotEnoughCoins : MonoBehaviour
{
    public static NotEnoughCoins Instance;
    public GameObject Panel;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        Panel.SetActive(false);
    }

    public void ShowUp()
    {
#if UNITY_ANDROID || UNITY_IOS
        Panel.SetActive(true);
#endif
    }

    public void Close()
    {
        Panel.SetActive(false);
    }
}