using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class MainMenuUpdateGem : MonoBehaviour
{
    public TextMeshProUGUI coins;

    // Update is called once per frame
    void Update()
    {
        coins.text = GlobalValue.SavedGems.ToString();
    }
}