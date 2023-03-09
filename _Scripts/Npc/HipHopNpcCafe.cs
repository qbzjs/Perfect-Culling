using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HipHopNpcCafe : MonoBehaviour
{
    [SerializeField] private GameObject[] obs_Audience;
    [SerializeField] private GameObject obs_Battler1;
    [SerializeField] private GameObject obs_Battler2;

    private int turn_Dance = 1;
    private bool is_Dance = false;
    private float time_BreakAnim;
    private float time_Break = 0;

    // Start is called before the first frame update
    void Start()
    {
        AudienceAnim("AudienceClap", obs_Audience);
    }


    private void Update()
    {
        if (!is_Dance)
        {
            BattleTime();
        }
        else
        {
            BattlerCompleteAnim(time_BreakAnim);
        }
    }
    private void BattlerCompleteAnim(float time)
    {
        if (time_Break >= time)
        {
            is_Dance = false;
            turn_Dance++;
            time_Break = 0;
        }
        else
        {
            time_Break += Time.deltaTime;
        }
    }
    private void BattleTime()
    {
        int index = 0;
        if (turn_Dance == 1)
        {
            BattlerAnim("Hiphopdance", obs_Battler1);
            BattlerAnim("AudienceClap", obs_Battler2);
            is_Dance = true;
            index = (int)AnimHipHop.Hiphopdance;
            time_BreakAnim = TimeToPlay(obs_Battler1, index);
            return;
        }
        if (turn_Dance == 2)
        {
            BattlerAnim("AudienceClap", obs_Battler1);
            BattlerAnim("Hiphopdance", obs_Battler2);
            index = (int)AnimHipHop.Hiphopdance;
            time_BreakAnim = TimeToPlay(obs_Battler2, index);
            is_Dance = true;
            return;
        }
        if (turn_Dance == 3)
        {
            BattlerAnim("Hiphopdance", obs_Battler1);
            BattlerAnim("Hiphopdance", obs_Battler2);
            index = (int)AnimHipHop.Hiphopdance;
            time_BreakAnim = TimeToPlay(obs_Battler1, index);
            is_Dance = true;
            return;
        }
        if (turn_Dance == 4)
        {
            BattlerAnim("HappyIdle", obs_Battler1);
            BattlerAnim("HappyIdle", obs_Battler2);
            index = (int)AnimHipHop.HappyIdle;
            time_BreakAnim = TimeToPlay(obs_Battler1, index);
            is_Dance = true;
            return;
        }
        if (turn_Dance == 5)
        {
            BattlerAnim("VictoryIdleHiphop", obs_Battler1);
            BattlerAnim("VictoryIdleHiphop", obs_Battler2);
            index = (int)AnimHipHop.Victory;
            time_BreakAnim = TimeToPlay(obs_Battler1, index);
            is_Dance = true;
            return;
        }
        if (turn_Dance == 6)
        {
            turn_Dance = 1;
            return;
        }
    }
    private void AudienceAnim(string name_anim, GameObject[] obs)
    {
        if (obs != null && obs.Length > 0)
        {
            foreach (var item in obs)
            {
                if (item != null)
                {
                    item.GetComponent<Animator>().Play(name_anim);
                }
            }
        }
    }

    private void BattlerAnim(string name_anim, GameObject ob_battler)
    {
        if (ob_battler != null)
            ob_battler.GetComponent<Animator>().Play(name_anim);
    }

    private float TimeToPlay(GameObject ob, int index)
    {
        float time_anim = ob.GetComponent<Animator>().runtimeAnimatorController.animationClips[index].length;
        return time_anim;
    }
}
