using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CodeStage.AntiCheat.Time;
using System.Linq;

[UIPanelPrefabAttr("PopupSlotmachine", "PopupCanvas")]
public class PopupSlotmachine : BasePanel
{
    private ObscuredBool isSpining = false;
    private ObscuredDecimal psFee;
    private ObscuredDecimal pRLFee;
    private CurrencyType currentCurreny = CurrencyType.Ps;
    [SerializeField]
    private TextMeshProUGUI txCurrency, txWinAmount, txHoldToSpin;
    [SerializeField]
    private Image imgBtSpin, imgBtCancel, imgBtTopup, imgFilledWhenHoldBtSpin;
    [SerializeField]
    private Button btSpin, btCancel, btTopup, btInfo;
    [SerializeField]
    private Sprite sprSpin, sprSpinPressed, sprCancel, sprCancelPressed, sprTopup, sprTopupPressed;
    private ObscuredBool isAutoSpin = false;
    private ObscuredBool isPressedSpin = false;
    private ObscuredBool isPressedCancel = false;
    private ObscuredBool isPressedTopup = false;
    private ObscuredString _address = "";
    private ObscuredString address
    {
        get
        {
            if (string.IsNullOrEmpty(_address))
                _address = UserDatas.user_Data.info.address;
            return _address;
        }
    }
    private ObscuredDecimal previousWinAmount = 0;
    public Action actionClose = null;
    [SerializeField]
    private GameObject rollItemPrefab;
    [SerializeField]
    private SlotmachineColumn[] columSlotmachines;
    private Coroutine corSpin = null;
    private Coroutine corShowResult = null;
    [SerializeField] private List<ParticleSystem> winEffects;
    [SerializeField] private GameObject lineWin;
    [SerializeField] private List<Animator> flyMoneyAnimators;
    [SerializeField] private List<TMP_Text> flyMoneyTexts;
    private ObscuredBool isPointerDown = false;
    private ObscuredFloat timePointerDown = 0;
    private ObscuredFloat timeToTriggerLongPointerDown = 2;
    [SerializeField]
    private GameObject obInfo;
    [SerializeField]
    private TextMeshProUGUI txSpinPsFeeInfo;
    [SerializeField] Image arowLeft, arowRight;
    [SerializeField] private Animator animator;
    [Header("EffectWinLose")]
    [SerializeField] private GameObject obWinEffect, obUnluckyEffect;
    [SerializeField]
    private TextMeshProUGUI txTotal, txWin;




    private void SetActiveLoading(bool active)
    {
        if (active)
            PanelManager.Show<PopupLoading>();
        else
            PanelManager.Hide<PopupLoading>();
    }

    private void OnEnable()
    {
        
        TPRLSoundManager.Instance.MuteSFX("SoundSlotMachineBG", true);
        TPRLSoundManager.Instance.PlayMusic("SlotMachineBackground1", true);
        CloseWinEffect();
        // SetActiveInfo(false);
        if (obInfo != null) obInfo.gameObject.SetActive(false);
        SetActiveWinEffect(false);
        SetActiveBtCancel(false);
        SetActiveLoading(true);
        GetBalance();
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
            GetBalance();
    }

    private void Start()
    {
        if (animator == null)
        {
            animator = gameObject.GetComponent<Animator>();
        }
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
        InitEvents();
        CreateRollItems();
        GetPlayFees();
    }

    private void InitEvents()
    {
        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((e) => OnBtSpinPointerDown());
        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((e) => OnBtSpinPointerUp());
        EventTrigger eventTriggerBtSpin = btSpin.gameObject.AddComponent<EventTrigger>();

        eventTriggerBtSpin.triggers.Add(pointerDown);
        eventTriggerBtSpin.triggers.Add(pointerUp);
        if (btCancel != null)
            btCancel.onClick.AddListener(ClickCancel);
        if (btTopup != null)
        {
            btTopup.onClick.AddListener(ClickTopup);
        }
        if (btInfo != null)
            btInfo.onClick.AddListener(ClickInfo);
    }

    private void Update()
    {
        if (isPointerDown == false || isSpining) return;
        timePointerDown += SpeedHackProofTime.deltaTime;
        SetImgFilledWhenHold();
        if (timePointerDown >= timeToTriggerLongPointerDown)
        {
            SetAutoSpinMode();
        }
    }

    private void SetImgFilledWhenHold()
    {
        if (imgFilledWhenHoldBtSpin == null || isAutoSpin) return;
        float amount = timePointerDown / timeToTriggerLongPointerDown;
        imgFilledWhenHoldBtSpin.fillAmount = amount;
    }

    private void CreateRollItems()
    {
        if (rollItemPrefab == null)
        {
            Debug.LogError("roll item null");
            return;
        }
        ServerConfig.Slotmachine.RollItem[] rollItems = EnvironmentConfig.serverConfig.slot_machine.roll_item;
        if (rollItems == null || rollItems.Length == 0)
        {
            Debug.LogError("roll items server null");
            return;
        }
        int column_length = columSlotmachines.Length;
        int roll_items_length = rollItems.Length + 2;
        float spacing_position_y = -248.4f;
        float position_y = spacing_position_y;
        for (int i = 0; i < column_length; i++)
        {
            SlotmachineColumn parent = columSlotmachines[i];
            position_y = spacing_position_y;
            System.Random random = new System.Random();
            rollItems = rollItems.OrderBy(x => random.Next()).ToArray();
            for (int j = 0; j < roll_items_length; j++)
            {
                ServerConfig.Slotmachine.RollItem rollItem = default;
                if (j == roll_items_length - 1)
                    rollItem = rollItems[1];
                else if (j == roll_items_length - 2)
                {
                    rollItem = rollItems[0];
                    parent.SetEndPositionSpin(-position_y);
                }
                else
                    rollItem = rollItems[j];
                Image item = CreateController.instance.CreateObjectGetComponent<Image>(rollItemPrefab, Vector3.zero, parent.transform);
                if (item == null) continue;
                item.name = $"{rollItem.sticker}_{j}";
                item.transform.localPosition = new Vector3(0, position_y, 0);
                parent.AddPosition(item.name, position_y);
                position_y -= spacing_position_y;
                item.sprite = TexturesManager.Instance.GetSprites(rollItem.sticker);
                item.gameObject.SetActive(true);
            }
        }
    }

    private void GetPlayFees()
    {
        int length = EnvironmentConfig.serverConfig.slot_machine.spin_fee.Length;
        for (int i = 0; i < length; i++)
        {
            if (EnvironmentConfig.serverConfig.slot_machine.spin_fee[i].symbol == "PS")
            {
                psFee = Decimal.Divide((decimal)EnvironmentConfig.serverConfig.slot_machine.spin_fee[i].fee, (decimal)Math.Pow(10, 18));
                //ps_support = EnvironmentConfig.serverConfig.slot_machine.spin_fee[i].is_support;
                if (txSpinPsFeeInfo != null)
                    txSpinPsFeeInfo.text = $"Spin = {psFee}";
            }
            else if (EnvironmentConfig.serverConfig.slot_machine.spin_fee[i].symbol == "PRL")
            {
                pRLFee = Decimal.Divide((decimal)EnvironmentConfig.serverConfig.slot_machine.spin_fee[i].fee, (decimal)Math.Pow(10, 18));
                //prl_support = EnvironmentConfig.serverConfig.slot_machine.spin_fee[i].is_support;
            }
        }
    }

    private void SetActiveBtCancel(bool active)
    {
        SetSpriteButton(imgBtSpin, sprSpin);
        SetSpriteButton(imgBtCancel, sprCancel);
        if (btSpin != null)
            btSpin.gameObject.SetActive(!active);
        if (btCancel != null)
            btCancel.gameObject.SetActive(active);
        if (btTopup != null)
            btTopup.gameObject.SetActive(false);
    }

    private void SetActiveBtTopup(bool active)
    {
        if (active)
        {
            SetSpriteButton(imgBtTopup, sprTopup);
            if (btTopup != null)
                btTopup.gameObject.SetActive(true);
            if (btSpin != null)
                btSpin.gameObject.SetActive(false);
            if (btCancel != null)
                btCancel.gameObject.SetActive(false);
        }
        else
        {
            SetActiveBtCancel(isAutoSpin);
        }
    }

    private void GetBalance()
    {
        TPRLAPI.instance.GetUserBalance(UserDatas.token, UserDatas.user_Data.info.address, (data) =>
        {
            UserBalanceResponse myDeserializedClass = JsonUtility.FromJson<UserBalanceResponse>(data);

            foreach (var item in myDeserializedClass.output.wallets)
            {
                if (item.symbol.ToLower().Equals("ps"))
                {
                    UserDatas.user_Data.info.ps.balance_dec = Decimal.Divide(decimal.Parse(item.balance), (decimal)Math.Pow(10, 18));
                }
                if (item.symbol.ToLower().Equals("prl"))
                {
                    UserDatas.user_Data.info.prl.balance_dec = Decimal.Divide(decimal.Parse(item.balance), (decimal)Math.Pow(10, 18));
                }
            }
            SetBalanceText();
            SetActiveLoading(false);
            if (isSpining == false)
            {
                CheckBalance();
            }
        });
    }

    private void CheckBalance()
    {
        ObscuredBool is_enough_balance = currentCurreny == CurrencyType.Ps ? UserDatas.user_Data.info.ps.balance_dec > psFee : UserDatas.user_Data.info.prl.balance_dec > pRLFee;
        Sprite spr = null;
        if (is_enough_balance)
        {
            spr = isAutoSpin ? sprCancel : sprSpin;
            isPressedSpin = false;
            SetActiveBtCancel(isAutoSpin);
            return;
        }

        isPressedTopup = false;
        SetActiveBtTopup(true);
        PopUpNotice popUpNotice = PanelManager.Show<PopUpNotice>();
        if (popUpNotice != null)
            popUpNotice.OnSetTextOneButton(Constant.NOTICE, Constant.BALANCE_NOT_ENOUGH, null, "OK");
    }

    private void OnBtSpinPointerDown()
    {
        timePointerDown = 0;
        isPointerDown = true;
    }

    private void OnBtSpinPointerUp()
    {
        timePointerDown = 0;
        SetImgFilledWhenHold();
        isPointerDown = false;
        if (isSpining) return;
        bool is_long_pointer_down = timePointerDown > timeToTriggerLongPointerDown;
        if (is_long_pointer_down)
        {
            SetAutoSpinMode();
            return;
        }
        ClickSpin();
    }

    private void SetBalanceText()
    {
        if (txCurrency != null)
            txCurrency.text = currentCurreny == CurrencyType.Ps ? $"{UserDatas.user_Data.info.ps.balance_dec}" : $"{UserDatas.user_Data.info.prl.balance_dec}";
    }

    private void ClickSpin()
    {
        if (isPressedSpin) return;
        isPressedSpin = true;
        isSpining = true;
        if (corSpin != null)
            StopCoroutine(corSpin);
        corSpin = StartCoroutine(IESpin());
        SetSpriteButton(imgBtSpin, sprSpinPressed);
        SetActiveInfo(false);
        CloseWinEffect();
        RequestSpin();
    }

    private void SetAutoSpinMode()
    {
        timePointerDown = 0;
        isPointerDown = false;
        SetImgFilledWhenHold();

        isAutoSpin = true;
        SetTextHoldToSpin();
        SetActiveBtCancel(isAutoSpin);
        AutoSpin();
    }

    private void AutoSpin()
    {
        isSpining = true;
        if (corSpin != null)
            StopCoroutine(corSpin);
        corSpin = StartCoroutine(IESpin());
        RequestSpin();
    }

    private IEnumerator IESpin()
    {
        if (columSlotmachines == null || columSlotmachines.Length == 0) yield break;
        int length = columSlotmachines.Length;
        for (int i = 0; i < length; i++)
        {
            columSlotmachines[i].SetSpin(true);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void ClickCancel()
    {
        if (isPressedCancel) return;
        isPressedCancel = true;
        SetSpriteButton(imgBtCancel, sprCancelPressed);
    }

    private void ClickTopup()
    {
        if (isPressedTopup) return;
        isPressedTopup = true;
        SetSpriteButton(imgBtTopup, sprTopupPressed);
    }

    private void RequestSpin()
    {
        SetActiveWinEffect(false);
        ObscuredString token_address = currentCurreny == CurrencyType.Ps ? UserDatas.user_Data.info.ps.contract_address : UserDatas.user_Data.info.prl.contract_address;
        TPRLAPI.instance.RequestSpinSlotmachine(UserDatas.token, address, token_address, (data) =>
        {
            if (data.error)
            {
                PopUpNotice popUpNotice = PanelManager.Show<PopUpNotice>();
                if (data.message.Equals(SMErrorCode.INSUFFICIENT_FUND))
                {
                    if (popUpNotice != null)
                        popUpNotice.OnSetTextOneButton(Constant.NOTICE, Constant.BALANCE_NOT_ENOUGH, null, "OK");
                }
                else
                {
                    if (popUpNotice != null)
                        popUpNotice.OnSetTextOneButton(Constant.NOTICE, Constant.COMMON_ERROR, null, "OK");
                }
                isSpining = false;
                isPressedSpin = false;
                SetActiveBtCancel(isAutoSpin);
                return;
            }
            if (data.output == null || data.output.values == null || data.output.values.Length != columSlotmachines.Length)
            {
                PopUpNotice popUpNotice = PanelManager.Show<PopUpNotice>();
                if (popUpNotice != null)
                    popUpNotice.OnSetTextOneButton(Constant.NOTICE, Constant.COMMON_ERROR, null, "OK");
                return;
            }
            previousWinAmount = currentCurreny == CurrencyType.Ps ? Decimal.Multiply((decimal)data.output.multi, psFee) : Decimal.Multiply((decimal)data.output.multi, pRLFee);
            ShowResult(data);
        });
    }

    private void ShowResult(PlaySlotMachineResponse response)
    {
        if (corShowResult != null)
            StopCoroutine(corShowResult);
        corShowResult = StartCoroutine(IEShowResult(response));
    }

    private IEnumerator IEShowResult(PlaySlotMachineResponse response)
    {
        yield return new WaitForSeconds(1);
        int[] result = response.output.values;
        int length_result = result.Length;
        for (int i = 0; i < length_result; i++)
        {
            bool is_done = false;
            columSlotmachines[i].ShowResult(result[i], delegate
            {
                is_done = true;
            });
            yield return new WaitUntil(() => is_done);
        }
        isSpining = false;
        bool is_win = response.output.multi > 0;
        string sound = GetWinSoundFx(result);
        TPRLSoundManager.Instance.PlaySoundFx(sound);
        ShowWinLose(is_win);
        if (isPressedCancel)
        {
            isPressedCancel = false;
            isAutoSpin = false;
            SetActiveBtCancel(isAutoSpin);
            SetTextHoldToSpin();
        }
        GetBalance();
        if (is_win)
        {
            OpenWinEffect();
        }
        else
        {
            OpenUnluckyEffect();
        }
        if (isAutoSpin)
        {
            yield return new WaitForSeconds(is_win ? 2 : 1);
            AutoSpin();
        }
    }

    private void ShowWinLose(bool is_win)
    {
        SetTextWinAmount();
        SetActiveWinEffect(is_win);
    }

    private void SetActiveWinEffect(bool active)
    {
        foreach (ParticleSystem particle in winEffects)
        {
            particle.gameObject.SetActive(active);
            if (active)
                particle.Play();
            else
                particle.Stop();
        }
        if (lineWin != null)
            lineWin.SetActive(active);
        if (active)
        {
            
            foreach (Animator animator in flyMoneyAnimators)
            {
                animator.Play("FlyMoney");
            }
        }

    }

    private void StopAll()
    {
        StopAllCoroutines();
        isSpining = false;
        isPressedSpin = false;
        isPressedCancel = false;
        isPressedTopup = false;
        isAutoSpin = false;
        int length = columSlotmachines.Length;
        for (int i = 0; i < length; i++)
        {
            columSlotmachines[i].SetSpin(false);
        }
        SetActiveBtCancel(isAutoSpin);
    }

    private void SetSpriteButton(Image img, Sprite sprite)
    {
        if (img != null)
            img.sprite = sprite;
    }

    private void SetTextHoldToSpin()
    {
        if (txHoldToSpin != null)
            txHoldToSpin.text = isAutoSpin ? "Click to cancel Auto spin" : "HOLD to Auto spin";
    }

    private void SetTextWinAmount()
    {
        if (txWinAmount != null)
            txWinAmount.text = $"+{previousWinAmount}";
        foreach (TMP_Text flyMoney in flyMoneyTexts)
        {
            if (flyMoney != null)
            {
                flyMoney.text = $"+{previousWinAmount}";
            }
        }
    }

    public override void HidePanel()
    {
        StopAll();
        actionClose?.Invoke();
        base.HidePanel();
    }

    public void Close()
    {
        if (obInfo != null && obInfo.activeSelf)
        {
            SetActiveInfo(false);
            return;
        }
        if (isSpining)
        {
            PopUpNotice popUpNotice = PanelManager.Show<PopUpNotice>();
            popUpNotice.OnSetTextTwoButtonCustom(Constant.NOTICE, SMErrorCode.CONFIRM_QUIT_SM, delegate
            {
                HidePanel();
            }, null, "Confirm", "Cancel");
            return;
        }
        else
            HidePanel();
    }

    private void OnApplicationQuit()
    {
        StopAll();
    }

    private string GetWinSoundFx(int[] result)
    {
        if (result == null || result.Length < 3) return "";
        if (result[0] != result[1] || result[0] != result[2] || result[1] != result[2]) return "";
        string sound = "";

        switch (result[0])
        {
            case 0:
            case 1:
                sound = "Melon&CherryWinner";
                break;
            case 2:
                sound = "BarWinner";
                break;
            case 3:
                sound = "JackpotWinner";
                break;
            default:
                break;
        }
        return sound;
    }

    private void ClickInfo()
    {
        if (obInfo == null) return;
        SetActiveInfo(!obInfo.activeSelf);
    }

    private void SetActiveInfo(bool active)
    {
        if (arowLeft == null || arowRight == null||obInfo==null) return;
        if (active)
        {
            animator.SetBool("statusInfor", true);
        }
        else
        {
            animator.SetBool("statusInfor", false);
        }

    }
    private void EnableInfor()
    {
        if (obInfo == null) return;
        obInfo.gameObject.SetActive(true);
        arowLeft.gameObject.SetActive(false);
        arowRight.gameObject.SetActive(false);
    }
    private void DisableInfor()
    {
        if (obInfo == null) return;
        obInfo.gameObject.SetActive(false);
        arowLeft.gameObject.SetActive(true);
        arowRight.gameObject.SetActive(true);
    }
    #region WinLoseEffect
    private void OpenWinEffect()
    {
        obWinEffect.SetActive(true);
        animator.SetTrigger("winEffect");
        SetTextTotalAndWinEffect();
        StartCoroutine(AutoCloseWinEffect());
    }
    private void SetTextTotalAndWinEffect()
    {
        txTotal.text  = currentCurreny == CurrencyType.Ps ? $"{UserDatas.user_Data.info.ps.balance_dec}" : $"{UserDatas.user_Data.info.prl.balance_dec}"; ;
        txWin.text = txWinAmount.text;
    }
    private IEnumerator AutoCloseWinEffect()
    {
        yield return new WaitForSeconds(2f);
        CloseWinEffect();
    }
    private void CloseWinEffect()
    {
        if(obWinEffect!=null&&obWinEffect.activeSelf)
        obWinEffect.SetActive(false);
    }
    private void OpenUnluckyEffect()
    {
        obUnluckyEffect.SetActive(true);
        animator.SetTrigger("unluckyEffect");
        StartCoroutine(CloseUnluckyEffect());
    }
    private IEnumerator CloseUnluckyEffect()
    {
        yield return new WaitForSeconds(1f);
        obUnluckyEffect.SetActive(false);
    }
    #endregion
    private void OnDisable()
    {
        TPRLSoundManager.Instance.PlayMusic("CityBackground", true);
        TPRLSoundManager.Instance.PlaySFX("SoundSlotMachineBG", "SlotBackgroundMusic");
    }

}