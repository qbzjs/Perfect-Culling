using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    private string url = "https://chainsafe.github.io/game-web3wallet/";
    [SerializeField]
    private Button btMetamask, btWalletConnect, btLogout, btTapToPlay, btGuest, btReload;
    [SerializeField]
    private TextMeshProUGUI txVersion, txGettingInfo;
    [SerializeField]
    private TextMeshProUGUI obConnectingToServer, txSessionTime;
    [SerializeField] private GameObject notConnectedGameObject;
    [SerializeField] private GameObject connectedGameObject;
    [SerializeField] private GameObject imgConnectWallet, imgSessionExpired;

    private Image _imgBtMetamaskLogin;
    private Image imgBtMetamaskLogin
    {
        get
        {
            if (_imgBtMetamaskLogin == null && btMetamask != null)
                _imgBtMetamaskLogin = btMetamask.GetComponent<Image>();
            return _imgBtMetamaskLogin;
        }
    }

    private Image _imgBtWalletConnectLogin;
    private Image imgBtWalletConnectLogin
    {
        get
        {
            if (_imgBtWalletConnectLogin == null && btWalletConnect != null)
                _imgBtWalletConnectLogin = btWalletConnect.GetComponent<Image>();
            return _imgBtWalletConnectLogin;
        }
    }

    private byte countPrepareStep = 0;
    private byte totalPrepareStep = 3;
    private string _lastTimeStamp = "";
    private string lastTimeStamp
    {
        get
        {
            if (string.IsNullOrEmpty(_lastTimeStamp))
                _lastTimeStamp = ObscuredPrefs.Get<string>(Constant.LastTimeStamp, "");
            return _lastTimeStamp;
        }
    }

    private void Awake()
    {
        SetText(txVersion, $"v{Application.version}");
        SetText(txGettingInfo, "");
        UserDatas.current_room_type = CurrentRoomType.None;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetButtonEvent(btMetamask, ClickMetamask);
        SetButtonEvent(btWalletConnect, ClickWalletConnect);
        SetButtonEvent(btTapToPlay, ClickTapToPlay);
        SetButtonEvent(btLogout, ClickLogout);
        SetButtonEvent(btGuest, ClickPlayGuest);
        SetButtonEvent(btReload, ClickReload);
        CheckExistAccount();
    }

    private void ActiveAllConnectWalletButton(bool active)
    {
        if (btMetamask != null)
        {
            btMetamask.interactable = active;
            imgBtMetamaskLogin.color = new Color32(255, 255, 255, (byte)(active ? 255 : 100));
        }
        if (btWalletConnect != null)
        {
            btWalletConnect.interactable = active;
            imgBtWalletConnectLogin.color = new Color32(255, 255, 255, (byte)(active ? 255 : 100));
        }
    }

    private void ClickReload()
    {
        ScenesManager.Instance.GetScene(BundleName.SCENES, AllSceneName.Login, false, null, true);
    }

    private void ClickPlayGuest()
    {
        UserDatas.is_guest = true;
    }

    private void ClickLogout()
    {
        SetText(txGettingInfo, "");
        ObscuredPrefs.DeleteAll();
        countPrepareStep = 0;
        StopAllCoroutines();
        SetActiveNotConnected(true);
    }

    private void ClickTapToPlay()
    {
        if (btTapToPlay != null)
            btTapToPlay.interactable = false;
        SetActiveLoading(true);
        long timestamp = long.Parse(ObscuredPrefs.Get<string>(Constant.LastTimeStamp, "0"));
        string lastSignature = ObscuredPrefs.Get<string>(Constant.SIGNATURE, "");
        string address = ObscuredPrefs.Get<string>(Constant.UserAddress, "");
        StartLogin(lastSignature, address, timestamp);
    }

    private void ClickMetamask()
    {
        ActiveAllConnectWalletButton(false);
        SetActiveLoading(true);
        long timestamp = GetCurrentTimeStampMiliSec();
        string message = "parallel_" + timestamp.ToString();
        StartCoroutine(IESign(message, (signature) =>
        {
            StartVerify(message, signature, timestamp);
        }));
    }

    private IEnumerator IESign(string message, Action<string> action)
    {
        GUIUtility.systemCopyBuffer = "";
        Application.OpenURL(url + "?action=sign&message=" + message);
        string clip_board = "";
        SetText(txGettingInfo, "Please copy metamask signature to login");
        while (string.IsNullOrEmpty(clip_board))
        {
            clip_board = GUIUtility.systemCopyBuffer;
            yield return new WaitForSeconds(0.1f);
        }
        if (clip_board.StartsWith("0x") && clip_board.Length == 132)
        {
            action?.Invoke(clip_board);
        }
        else
            action?.Invoke("");
    }

    private void StartVerify(string message, string signature, long timestamp)
    {
        if (string.IsNullOrEmpty(signature))
        {
            SetText(txGettingInfo, "Login metamask failed, please retry");
            SetActiveLoading(false);
            ActiveAllConnectWalletButton(true);
            return;
        }
        GameObject enm_ob = new GameObject();
        EVM evm = enm_ob.AddComponent<EVM>();
        evm.Verify(message, signature, (address) =>
        {
            if (!string.IsNullOrEmpty(address) && address.Length == 42)
            {
                StartLogin(signature, address, timestamp);
            }
            else
            {
                SetText(txGettingInfo, "Metamask address info is not valid, please retry");
                SetActiveLoading(false);
                ActiveAllConnectWalletButton(true);
            }
        }, delegate
        {
            SetText(txGettingInfo, "Verify metamask failed, please retry");
            SetActiveLoading(false);
            ActiveAllConnectWalletButton(true);
        });
    }

    private void StartLogin(string signature, string address, long timestamp)
    {
        TPRLAPI.instance.LogIn(signature, address, timestamp, (login_response) =>
        {
            if (login_response.error)
            {
                PopUpNotice popupNotice = PanelManager.Show<PopUpNotice>();
                popupNotice.OnSetTextOneButton(Constant.LoginFailed, login_response.message, delegate { ClickLogout(); }, "Close");
                return;
            }
            SetText(txGettingInfo, $"Login success! Setting up infomation {countPrepareStep}/{totalPrepareStep}");
            UserDatas.is_guest = false;
            string token = login_response.Equals((TPRLResponse)default) ? "" : "Bearer " + login_response.output;
            ObscuredPrefs.Set<string>(Constant.SIGNATURE, signature);
            ObscuredPrefs.Set<string>(Constant.LastTimeStamp, timestamp + "");
            UserDatas.token = token;
            ObscuredPrefs.Set<string>(Constant.UserAddress, address);
            ObscuredPrefs.Save();
            GetUserInfo(token, address);
            GetBalance(token, address);
            GetProgress(token, address);
        });

    }

    private Action action1;
    private void FakeDataCharacter()
    {
        RecordItemInventory[] recordUserInventories = new RecordItemInventory[15];
        //
        recordUserInventories[0].id = "1";
        recordUserInventories[0].name = "head_1";
        recordUserInventories[0].icon = "nhan-vat-avata_1";
        recordUserInventories[0].type = InventoryItemType.head.ToString();
        recordUserInventories[0].is_hide_item = false;
        //
        recordUserInventories[1].id = "2";
        recordUserInventories[1].name = "head_2";
        recordUserInventories[1].icon = "nhan-vat-avata_1";
        recordUserInventories[1].type = InventoryItemType.head.ToString();
        recordUserInventories[1].is_hide_item = false;
        //
        recordUserInventories[2].id = "1";
        recordUserInventories[2].name = "middle_charcter_1";
        recordUserInventories[2].icon = "nhan-vat-avata_1";
        recordUserInventories[2].type = InventoryItemType.middle.ToString();
        recordUserInventories[2].is_hide_item = false;
        //
        recordUserInventories[3].id = "2";
        recordUserInventories[3].name = "middle_charcter_2";
        recordUserInventories[3].icon = "nhan-vat-avata_1";
        recordUserInventories[3].type = InventoryItemType.middle.ToString();
        recordUserInventories[3].is_hide_item = false;
        //
        recordUserInventories[4].id = "1";
        recordUserInventories[4].name = "bottom_charcter_1";
        recordUserInventories[4].icon = "nhan-vat-avata_1";
        recordUserInventories[4].type = InventoryItemType.bottom.ToString();
        recordUserInventories[4].is_hide_item = false;
        //
        recordUserInventories[5].id = "2";
        recordUserInventories[5].name = "bottom_charcter_2";
        recordUserInventories[5].icon = "nhan-vat-avata_1";
        recordUserInventories[5].type = InventoryItemType.bottom.ToString();
        recordUserInventories[5].is_hide_item = false;
        //
        recordUserInventories[6].id = "1";
        recordUserInventories[6].name = "shoes_1";
        recordUserInventories[6].icon = "nhan-vat-avata_1";
        recordUserInventories[6].type = InventoryItemType.shoes.ToString();
        recordUserInventories[6].is_hide_item = false;
        //
        recordUserInventories[7].id = "2";
        recordUserInventories[7].name = "shoes_2";
        recordUserInventories[7].icon = "nhan-vat-avata_1";
        recordUserInventories[7].type = InventoryItemType.shoes.ToString();
        recordUserInventories[7].is_hide_item = false;
        //
        recordUserInventories[8].id = "1";
        recordUserInventories[8].name = "hair_1";
        recordUserInventories[8].icon = "nhan-vat-avata_1";
        recordUserInventories[8].type = InventoryItemType.hair.ToString();
        recordUserInventories[8].is_hide_item = false;
        //
        recordUserInventories[9].id = "2";
        recordUserInventories[9].name = "hair_2";
        recordUserInventories[9].icon = "nhan-vat-avata_1";
        recordUserInventories[9].type = InventoryItemType.hair.ToString();
        recordUserInventories[9].is_hide_item = false;
        #region Default id 0, array 10-14
        //
        recordUserInventories[10].id = "0";
        recordUserInventories[10].name = "head_0";
        recordUserInventories[10].icon = "nhan-vat-avata_1";
        recordUserInventories[10].type = InventoryItemType.head.ToString();
        recordUserInventories[10].is_hide_item = true;
        //
        recordUserInventories[11].id = "0";
        recordUserInventories[11].name = "middle_charcter_0";
        recordUserInventories[11].icon = "nhan-vat-avata_1";
        recordUserInventories[11].type = InventoryItemType.middle.ToString();
        recordUserInventories[11].is_hide_item = true;
        //
        recordUserInventories[12].id = "0";
        recordUserInventories[12].name = "bottom_charcter_0";
        recordUserInventories[12].icon = "nhan-vat-avata_1";
        recordUserInventories[12].type = InventoryItemType.bottom.ToString();
        recordUserInventories[12].is_hide_item = true;
        //
        recordUserInventories[13].id = "0";
        recordUserInventories[13].name = "shoes_0";
        recordUserInventories[13].icon = "nhan-vat-avata_1";
        recordUserInventories[13].type = InventoryItemType.shoes.ToString();
        recordUserInventories[13].is_hide_item = false;
        recordUserInventories[13].is_hide_item = true;
        //
        recordUserInventories[14].id = "0";
        recordUserInventories[14].name = "hair_0";
        recordUserInventories[14].icon = "nhan-vat-avata_1";
        recordUserInventories[14].type = InventoryItemType.hair.ToString();
        recordUserInventories[14].is_hide_item = true;
        //
        #endregion
        List<RecordItemInventory> listHair = new List<RecordItemInventory>();
        List<RecordItemInventory> listHead = new List<RecordItemInventory>();
        List<RecordItemInventory> listMiddle = new List<RecordItemInventory>();
        List<RecordItemInventory> listBottom = new List<RecordItemInventory>();
        List<RecordItemInventory> listShoes = new List<RecordItemInventory>();

        int inventory_length = recordUserInventories.Length;
        for (int i = 0; i < inventory_length; i++)
        {
            RecordItemInventory item = recordUserInventories[i];
            string item_type = item.type;
            switch (item_type)
            {
                case "hair":
                    listHair.Add(item);
                    break;
                case "head":
                    listHead.Add(item);
                    break;
                case "middle":
                    listMiddle.Add(item);
                    break;
                case "bottom":
                    listBottom.Add(item);
                    break;
                case "shoes":
                    listShoes.Add(item);
                    break;
                default:
                    break;
            }
        }
        UserDatas.user_Data.inventory_hair = listHair;
        UserDatas.user_Data.inventory_head = listHead;
        UserDatas.user_Data.inventory_middle = listMiddle;
        UserDatas.user_Data.inventory_bottom = listBottom;
        UserDatas.user_Data.inventory_shoes = listShoes;

    }
    private void GetUserInfo(string token, string address)
    {
        TPRLAPI.instance.GetUserInfo(token, address, (data) =>
        {
            //FakeVihecle.DataFake();
            UserInfoResponse myDeserializedClass = JsonUtility.FromJson<UserInfoResponse>(data);
            UserDatas.user_Data.info.address = myDeserializedClass.output.address;
            UserDatas.user_Data.info.username = myDeserializedClass.output.username;
            RecordItemInventory[] recordUserInventories = new RecordItemInventory[1];
            RecordItemInventory recordUserInventory0 = new RecordItemInventory();
            recordUserInventory0.id = "11";
            recordUserInventory0.type = "vehicle";
            recordUserInventory0.name = "Hummer";
            recordUserInventory0.icon = "hummer_0";
            recordUserInventory0.amount = 1;
            recordUserInventory0.is_equip = false;
            recordUserInventories[0] = recordUserInventory0;

            List<RecordItemInventory> listHair = new List<RecordItemInventory>();
            List<RecordItemInventory> listHead = new List<RecordItemInventory>();
            List<RecordItemInventory> listMiddle = new List<RecordItemInventory>();
            List<RecordItemInventory> listBottom = new List<RecordItemInventory>();
            List<RecordItemInventory> lstShoes = new List<RecordItemInventory>();
            List<RecordItemInventory> lstMission = new List<RecordItemInventory>();
            List<RecordItemInventory> lstConsumable = new List<RecordItemInventory>();
            List<RecordItemInventory> lstVehicle = new List<RecordItemInventory>();

            int inventory_length = recordUserInventories.Length;
            for (int i = 0; i < inventory_length; i++)
            {
                RecordItemInventory item = recordUserInventories[i];
                InventoryItemType item_type = Ultis.ParseEnum<InventoryItemType>(item.type);
                switch (item_type)
                {
                    case InventoryItemType.hair:
                        listHair.Add(item);
                        break;
                    case InventoryItemType.head:
                        listHead.Add(item);
                        break;
                    case InventoryItemType.middle:
                        listMiddle.Add(item);
                        break;
                    case InventoryItemType.bottom :
                        listBottom.Add(item);
                        break;
                    case InventoryItemType.shoes:
                        lstShoes.Add(item);
                        break;
                    case InventoryItemType.mission:
                        lstMission.Add(item);
                        break;
                    case InventoryItemType.consumable:
                        lstConsumable.Add(item);
                        break;
                    case InventoryItemType.vehicle:
                        lstVehicle.Add(item);
                        break;
                    default:
                        break;
                }
            }
            UserDatas.user_Data.inventory_hair = listHair;
            UserDatas.user_Data.inventory_head = listHead;
            UserDatas.user_Data.inventory_middle = listMiddle;
            UserDatas.user_Data.inventory_bottom = listBottom;
            UserDatas.user_Data.inventory_shoes = lstShoes;
            UserDatas.user_Data.inventory_mission = lstMission;
            UserDatas.user_Data.inventory_consumable = lstConsumable;
            UserDatas.user_Data.inventory_vehicle = lstVehicle;
            RecordCharacter[] recordCharacters = new RecordCharacter[1];
            RecordCharacter character = new RecordCharacter();
            character.id = 3;
            character.name = "Harry";
            character.icon = "character_3";
            //RecordCharacter character1 = new RecordCharacter();
            //character1.id = 0;
            //character1.name = "Horseman";
            //character1.icon = "character_0";
            recordCharacters[0] = character;
            //recordCharacters[1] = character1;
            UserDatas.user_Data.user_characters = recordCharacters;
            UserDatas.user_Data.info.current_selected_character = 9;

            RecordItemEquip recordItemEquip = new RecordItemEquip();
            recordItemEquip.itemID = "11";
            recordItemEquip.itemType = InventoryItemType.vehicle;
            recordItemEquip.is_equip = false;
            UserDatas.current_vehicle = recordItemEquip;
            CheckAllDone();
        });
    }

    private void GetBalance(string token, string address)
    {
        TPRLAPI.instance.GetUserBalance(token, address, (data) =>
        {
            UserBalanceResponse myDeserializedClass = JsonUtility.FromJson<UserBalanceResponse>(data);
            foreach (var item in myDeserializedClass.output.wallets)
            {
                if (item.symbol.ToLower().Equals("ps"))
                {
                    UserDatas.user_Data.info.ps = item;
                }
                if (item.symbol.ToLower().Equals("prl"))
                {
                    UserDatas.user_Data.info.prl = item;
                }
            }
            CheckAllDone();
        });
    }
    private void GetProgress(string token, string address)
    {
        TPRLAPI.instance.GetUserProgress(token, address, (data) =>
        {
            Debug.LogError("data" + data);
            UserProgressResponse myDeserializedClass = JsonUtility.FromJson<UserProgressResponse>(data);
            if (myDeserializedClass.output != null)
            {
                UserDatas.user_Data.info.is_tutorial_done = myDeserializedClass.output.is_tutorial_completed;
                JSONObject dataObj = JSON.Parse(data).AsObject;
                string main = "";
                if (dataObj["output"]["main"] != null)
                    main = dataObj["output"]["main"].ToString();
                if (string.IsNullOrEmpty(main)&&UserDatas.user_Data.info.is_tutorial_done)
                {
                    UserDatas.user_Data.info.open_mission_daily = true;
                    UserDatas.user_Data.info.current_id_main_mission = -1;
                }
                else
                {
                    UserDatas.user_Data.info.current_id_main_mission = myDeserializedClass.output.main.mission_id;
                }
                if (UserDatas.user_Data.info.current_id_main_mission>= QuestManager.numberMissionWelcomeEnd)
                {
                    UserDatas.user_Data.info.open_mission_daily = true;
                }
                if (myDeserializedClass.output.daily != null)
                {
                    int lengthDaily = myDeserializedClass.output.daily.Length;
                    UserDatas.missionDailyInfo = new RecordMissionDailyInfo[lengthDaily];
                    UserDatas.missionDailyInfoUsed = new RecordMissionDailyInfo[lengthDaily];
                    if (lengthDaily != 0)
                    {
                        for (int i = 0; i < lengthDaily; i++)
                        {
                            UserDatas.missionDailyInfo[i].mission_id = myDeserializedClass.output.daily[i].mission_id;
                            UserDatas.missionDailyInfo[i].mission_name = "Daily Mission";
                            UserDatas.missionDailyInfo[i].is_reward_received = myDeserializedClass.output.daily[i].is_reward_received;
                            if (UserDatas.missionDailyInfo[i].is_reward_received)
                            {
                                UserDatas.missionDailyInfoUsed[i].mission_id = myDeserializedClass.output.daily[i].mission_id;
                                UserDatas.missionDailyInfoUsed[i].mission_name = "Daily Mission";
                                UserDatas.missionDailyInfoUsed[i].is_reward_received = myDeserializedClass.output.daily[i].is_reward_received;
                            }
                        }
                    }
                }
            }

            CheckAllDone();
        });


    }

    private void ClickWalletConnect()
    {

    }

    private void CheckExistAccount()
    {
        if (!string.IsNullOrEmpty(lastTimeStamp))
        {
            int expiredTime = GetExpiredTime();
            if (GetCurrentTimeStamp() >= expiredTime)
            {
                SetActiveNotConnected(true);
                SetActiveExpiredUI(true);
            }
            else
            {
                SetActiveNotConnected(false);
                if (corIERunSessionTime != null)
                    StopCoroutine(corIERunSessionTime);
                corIERunSessionTime = StartCoroutine(IERunSessionTime());
            }
        }
        else
        {
            SetActiveNotConnected(true);
            SetActiveExpiredUI(false);
        }
    }

    private void CheckAllDone()
    {
        countPrepareStep++;
        SetText(txGettingInfo, $"Login success! Setting up infomation {countPrepareStep}/{totalPrepareStep}");
        if (countPrepareStep < totalPrepareStep) return;
        SetText(txGettingInfo, "Connecting to server...");
        NetworkingManager.Instance.OnJoinOpenworldSuccess = LoadSceneGame;
        NetworkingManager.Instance.OnJoinOpenworldFailed = OnJoinOpenworldFailed;
        NetworkingManager.Instance.JoinOpenworldRoom();
        VideoManager.instance.StartDownloadVideo();
        ImageManager.instance.StartDownloadImage();
    }

    private void LoadSceneGame()
    {
        SetText(txGettingInfo, $"Almost done!");
        PopupLoadingLogin popupLoadingLogin = PanelManager.Show<PopupLoadingLogin>();
        SetActiveLoading(false);
        FakeDataCharacter();
        PanelManager.Hide<PopupLoadingLogin>();
        ScenesManager.Instance.GetScene(BundleName.SCENES, AllSceneName.CreateCharacter, false, null, true);
        // ScenesManager.Instance.GetScene(BundleName.SCENES, AllSceneName.GamePlayMain, false, delegate
        // {
        //     popupLoadingLogin.EventSlider90Percent();
        //     ScenesManager.Instance.GetScene(BundleName.SCENES, AllSceneName.GamePlayCanvas, true, delegate
        //     {
        //         popupLoadingLogin.EventSlider100percent();
        //         popupLoadingLogin.PlayVideo();
        //         var builder = new UriBuilder(EnvironmentConfig.linkServerVoice);
        //         var uri = builder.Uri;
        //         Mirror.NetworkManager.singleton.StartClient(uri);
        //     }, true);
        // }, true);
    }

    private void OnJoinOpenworldFailed()
    {
        PopUpNotice popupNotice = PanelManager.Show<PopUpNotice>();
        popupNotice.OnSetTextTwoButtonCustom(Constant.NOTICE, "Connection error, please try again", delegate { NetworkingManager.Instance.JoinOpenworldRoom(); }, ClickLogout, "Retry", "Close");
    }

    private int GetCurrentTimeStamp()
    {
        return (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
    private long GetCurrentTimeStampMiliSec()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private int GetExpiredTime()
    {
        long last_timestamp = long.Parse(lastTimeStamp);
        int expiredTime = (int)(Constant.EXPIRED_TIME + (last_timestamp / 1000));
        return expiredTime;
    }

    private void SetActiveNotConnected(bool active)
    {
        StartCoroutine(IESetActiveNotConnected(active));
    }

    private IEnumerator IESetActiveNotConnected(bool active)
    {

        if (notConnectedGameObject != null)
            notConnectedGameObject.SetActive(active);
        if (connectedGameObject != null)
            connectedGameObject.SetActive(!active);
#if UNITY_IOS
        if (active)
        {
            ActiveAllConnectWalletButton(false);
            SetActiveConnectingToServer(true);
            yield return new WaitUntil(() => WalletConnect.ActiveSession.ReadyForUserPrompt);
            //yield return new WaitForSeconds(15);
            SetActiveConnectingToServer(false);
            ActiveAllConnectWalletButton(true);
        }
#endif
        yield return null;
    }

    private Coroutine corSetTextConnecting;
    private void SetActiveConnectingToServer(bool active)
    {
        if (corSetTextConnecting != null)
            StopCoroutine(corSetTextConnecting);
        if (obConnectingToServer != null)
            obConnectingToServer.gameObject.SetActive(active);
        if (active)
            corSetTextConnecting = StartCoroutine(IESetTextConnecting());
    }

    private IEnumerator IESetTextConnecting()
    {
        string text = "Connecting to wallet server";
        string[] dots = { ".", "..", "..." };
        int index = 0;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (obConnectingToServer != null)
                obConnectingToServer.text = $"{text}{dots[index]}";
            index++;
            if (index >= 3)
                index = 0;
        }
    }
    private void SetActiveExpiredUI(bool is_expired)
    {
        if (imgSessionExpired != null)
            imgSessionExpired.SetActive(is_expired);
        if (imgConnectWallet != null)
            imgConnectWallet.SetActive(!is_expired);
    }

    private Coroutine corIERunSessionTime = null;
    IEnumerator IERunSessionTime()
    {
        int expiredTime = GetExpiredTime();
        while (true)
        {
            yield return new WaitForSeconds(1);
            int currentSessionTime = expiredTime - GetCurrentTimeStamp();
            if (currentSessionTime >= 0)
            {
                SetSessionTimeText(currentSessionTime);
            }
            else
            {
                SetActiveNotConnected(true);
                SetActiveExpiredUI(true);
                yield break;
            }
        }
    }

    private void SetSessionTimeText(int time)
    {
        int hh = (int)time / 3600;
        int mm = (int)(time - hh * 3600) / 60;
        int ss = time - 3600 * hh - 60 * mm;

        string sessionTime = (hh >= 10 ? hh.ToString() : "0" + hh) + ":" +
            (mm >= 10 ? mm.ToString() : "0" + mm) + ":" +
            (ss >= 10 ? ss.ToString() : "0" + ss);
        if (txSessionTime)
            txSessionTime.text = sessionTime;
    }

    private void SetActiveLoading(bool active)
    {
        if (active)
            PanelManager.Show<PopupLoading>();
        else
            PanelManager.Hide<PopupLoading>();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnApplicationQuit()
    {
        StopAllCoroutines();
    }

    private void SetText(TextMeshProUGUI text_mesh, string content)
    {
        if (text_mesh != null)
            text_mesh.text = content;
    }

    private void SetButtonEvent(Button button, UnityAction action)
    {
        if (button != null)
            button.onClick.AddListener(action);
    }
}
