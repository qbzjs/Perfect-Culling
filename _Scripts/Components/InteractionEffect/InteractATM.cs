using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractATM : MonoBehaviour, IInteractionEffect
{
    Action action;
    public void Init(GameObject ob1, ResponseInteraction ob2, Action on_done)
    {
        string name = ob2.gameObject.name;
        if (name.Equals("PanCake"))
            Application.OpenURL("https://pancakeswap.finance/swap?outputCurrency=0xd07e82440A395f3F3551b42dA9210CD1Ef4f8B24&inputCurrency=0xe9e7cea3dedca5984780bafc599bd69add087d56");
        if (name.Equals("Kyber"))
            Application.OpenURL("https://kyberswap.com/#/swap?inputCurrency=0xe9e7CEA3DedcA5984780Bafc599bD69ADd087D56&outputCurrency=0xd07e82440A395f3F3551b42dA9210CD1Ef4f8B24&networkId=56");
        if (name.Equals("PRLOffChain"))
            Application.OpenURL("https://theparallel.io/user/offchain");
        action = on_done;
        OnDone();
    }

    public void OnDone()
    {
        action?.Invoke();
    }


}
