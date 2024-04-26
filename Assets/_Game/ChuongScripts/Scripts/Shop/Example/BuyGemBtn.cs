using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChuongCustom
{
    public class BuyGemBtn : BaseIAPButton
    {
        [SerializeField] private int _amount;

        protected override void OnBuySuccess()
        {
            ToastManager.Instance.ShowMessageToast("Buy Success!!");
            GlobalValue.SavedGems += _amount;
        }

        protected override void OnStart()
        {
        }
    }
}