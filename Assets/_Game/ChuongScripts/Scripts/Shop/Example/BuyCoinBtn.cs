using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChuongCustom
{
    public class BuyCoinBtn : AShopBtn
    {
        [SerializeField] private int _amount;
        [SerializeField] private int _price;

        protected override void ShowNotEnoughMoney()
        {
            ToastManager.Instance.ShowMessageToast("Not enough gems!!");
        }

        protected override bool IsEnoughResource()
        {
            return GlobalValue.SavedGems >= _price;
        }

        protected override void OnBuySuccess()
        {
            ToastManager.Instance.ShowMessageToast("Buy Success!!");
            GlobalValue.SavedGems -= _price;
            GlobalValue.SavedCoins += _amount;
        }

        protected override void OnStart()
        {
        }
    }
}