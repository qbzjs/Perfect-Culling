using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CodeStage.AntiCheat.Storage;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.Events;
using System;
using System.Text;
using System.Net;
using Unity.VisualScripting.Antlr3.Runtime;

public class AccountUserInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text tx_UserName;
    [SerializeField] private TMP_Text tx_WalletAddress;
    [SerializeField] private TMP_Text tx_PsBalance;
    [SerializeField] private TMP_Text tx_PrlBalance;
    [SerializeField] private Button btAddPrl;
    [SerializeField] private Button btAddPs;
    [SerializeField] private Button btRename;
    private StringBuilder sb_WalletAddress;
    private string str_WalletAddress;
    private bool _checkClickRename = false;
    private bool _checkClickAddCoin = false;
    
    private void SetText(TMP_Text tmp_text, string text)
    {
        if (tmp_text != null)
            tmp_text.text = text;
    }
    private void ClickButtonOn (Button button,UnityAction actionunity)
    {
        if (button != null)
        {
            button.onClick.AddListener(actionunity);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        sb_WalletAddress = new StringBuilder();
        str_WalletAddress = UserDatas.user_Data.info.address;
        sb_WalletAddress.Append(str_WalletAddress.Substring(0, 6));
        sb_WalletAddress.Append("...");
        sb_WalletAddress.Append(str_WalletAddress.Substring(str_WalletAddress.Length-6));
        SetText(tx_UserName, UserDatas.user_Data.info.username);
        SetText(tx_WalletAddress, sb_WalletAddress.ToString());
        ClickButtonOn(btAddPrl, ClickToLink);
        ClickButtonOn(btAddPs, ClickToLink);
        ClickButtonOn(btRename, ClickButtonRename);
       
    }
    private void ClickButtonRename()
    {
        _checkClickRename = true;
        Application.OpenURL(EnvironmentConfig.linkRename);
    }
    private void ClickToLink()
    {
        _checkClickAddCoin = true;
        Application.OpenURL(EnvironmentConfig.linkMarket);
    }

    private void OnEnable()
    {
        if (btRename != null)
        {
            btRename.gameObject.SetActive(string.IsNullOrEmpty(UserDatas.user_Data.info.username.Trim()));
        }
        GetBalance();
        
    }
    private void GetBalance()
    {
        string token = ObscuredPrefs.Get<string>(Constant.Token);
        string address = ObscuredPrefs.Get<string>(Constant.UserAddress);
        TPRLAPI.instance.GetUserBalance(token, address, (data) =>
        {
            UserBalanceResponse myDeserializedClass = JsonUtility.FromJson<UserBalanceResponse>(data);
            foreach (var item in myDeserializedClass.output.wallets)
            {
                if (item.symbol.ToLower().Equals("ps"))
                {
                    UserDatas.user_Data.info.ps = item;
                    UserDatas.user_Data.info.ps.balance_dec = Decimal.Divide(decimal.Parse(item.balance), (decimal)Math.Pow(10, 18));
                    SetText(tx_PsBalance, UserDatas.user_Data.info.ps.balance_dec.ToString());
                }
                if (item.symbol.ToLower().Equals("prl"))
                {
                    UserDatas.user_Data.info.prl = item;
                    UserDatas.user_Data.info.prl.balance_dec = Decimal.Divide(decimal.Parse(item.balance), (decimal)Math.Pow(10, 18));
                    SetText(tx_PrlBalance, UserDatas.user_Data.info.prl.balance_dec.ToString());
                }
            }
        });
    }
    void OnApplicationFocus(bool hasFocus)
    {
        if (_checkClickRename&& hasFocus)
        {
            GetUsernameInfor();
            _checkClickRename = false;
        }
        if (_checkClickAddCoin && hasFocus)
        {
            GetBalance();
            _checkClickAddCoin = false;
        }
    }

    void GetUsernameInfor()
    {
        string token = ObscuredPrefs.Get<string>(Constant.Token);
        string address = ObscuredPrefs.Get<string>(Constant.UserAddress);
        TPRLAPI.instance.GetUserInfo(token, address, (data) =>
        {
            UserInfoResponse myDeserializedClass = JsonUtility.FromJson<UserInfoResponse>(data);
            UserDatas.user_Data.info.username = myDeserializedClass.output.username;
            SetText(tx_UserName, UserDatas.user_Data.info.username);
            if (btRename != null)
            {
                btRename.gameObject.SetActive(string.IsNullOrEmpty(UserDatas.user_Data.info.username.Trim()));
            }
        }
        );


    }


}