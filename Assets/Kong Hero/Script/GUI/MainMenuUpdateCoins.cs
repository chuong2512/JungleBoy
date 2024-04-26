using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class MainMenuUpdateCoins : MonoBehaviour
{
    public TextMeshProUGUI coins;

    // Update is called once per frame
    void Update()
    {
        coins.text = GlobalValue.SavedCoins.ToString();
    }
}