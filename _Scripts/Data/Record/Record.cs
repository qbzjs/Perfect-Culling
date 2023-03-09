using System;
using Colyseus;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct Record
{
}

#region Camera Shaking
[System.Serializable]
public struct CameraShakingInfo
{
    public float shake_duration;
    public float shake_amount;
    public float decrease_factor;
}
#endregion

#region Unit info
[System.Serializable]
public struct RecordUnitInfo
{
    public float name;
    public float move_speed;
    public float sprint_speed;
    public float drive_move_speed;
    public float drive_sprint_speed;
    public JumpInfo jump_info;
    public string current_mission_id;

    [System.Serializable]
    public struct JumpInfo
    {
        public float jump_power;
        public float gravity;
        public float grounded_offset;
        public float grounded_radius;
    }
}
#endregion

#region Response interaction
[System.Serializable]
public struct RecordResponseInteractionInfo
{
    public string interaction_type;
}

[System.Serializable]
public struct RecordSeatInteractionInfo
{
    public int[] seat_positions;
}

[System.Serializable]
public struct RecordMissionInteractionInfo
{
    public int mission_id;

    public int next_mission_id;

    public string mission_type_quest;

    public string mission_type_name;

    public string mission_place;

    public string mission_detail;

    public string mission_name;

    public string mission_type;

    public int time_limit;

    public string video_name;

    public float[] camera_rotation;

    public float[] camera_position;

    public float[] target_position;

    public float[] finish_position;

    public string target_object_name;

    public string[] conversation_string;

    public Reward reward;

    public int current_progress;
    [System.Serializable]
    public struct Reward
    {
        public string type;
        public int id;
        public string name;
    }
}

[System.Serializable]
public struct RecordMissionDailyInfo
{
    public int mission_id;

    public int next_mission_id;

    public string mission_type_quest;

    public string mission_type_name;

    public string mission_place;

    public string mission_detail;

    public string mission_name;

    public string mission_type;

    public int time_limit;

    public string video_name;

    public float[] camera_rotation;

    public float[] camera_position;

    public float[] target_position;

    public float[] finish_position;

    public string target_object_name;

    public string[] conversation_string;

    public int current_progress;

    public bool is_complete;

    public bool is_reward_received;

    public Reward reward;

    [System.Serializable]
    public struct Reward
    {
        public string type;
        public int id;
        public string name;
    }
}
[System.Serializable]
public struct RecordQuizInteractionInfo
{
    public string quiz_id;

    public string question;

    public string[] answer;

    public int correct_answer;
}
[System.Serializable]
public struct RecordQuizConversationInfo
{
    public string target_object_name;

    public string[] conversation;

    public int quiz_position;
}
[System.Serializable]
public struct RecordTrashTalkInteractionInfo
{
    public int trash_talk_id;
    public string target_object_name;
}
#endregion

#region Mission
[System.Serializable]
public struct RecordInteractionTypeObserver
{
    public ResponseInteractionType interaction_type;
    public string object_name;
    public int mission_id;
    public string mission_name;
    public string mission_type;
    public string mission_type_quest;
    public string mission_type_name;
}
#endregion




#region User Data
[System.Serializable]
public struct RecordUserData
{
    public Info info;
    public List<RecordItemInventory> inventory_hat;
    //public List<RecordItemInventory> inventory_shirt;
    //public List<RecordItemInventory> inventory_pant;
    public List<RecordItemInventory> inventory_shoes;
    public List<RecordItemInventory> inventory_mission;
    public List<RecordItemInventory> inventory_consumable;
    public List<RecordItemInventory> inventory_vehicle;
    public RecordCharacter[] user_characters;
    public List<RecordItemInventory> inventory_hair;
    public List<RecordItemInventory> inventory_head;
    public List<RecordItemInventory> inventory_middle;
    public List<RecordItemInventory> inventory_bottom;


    //public List<RecordItemInventory> user_character_part_hat;
    //public List<RecordItemInventory> user_character_part_head;


    //public List<RecordItemInventory> user_character_part_chest;
    //public List<RecordItemInventory> user_character_part_elbow;
    //public List<RecordItemInventory> user_character_part_arm;
    //public List<RecordItemInventory> user_character_part_hand;
    //public List<RecordItemInventory> user_character_part_lowback;
    //public List<RecordItemInventory> user_character_part_hip;


    //public List<RecordItemInventory> user_character_part_femoral;
    //public List<RecordItemInventory> user_character_part_knee;


    //public List<RecordItemInventory> user_character_part_foot;
    [System.Serializable]
    public struct Info
    {
        public bool is_tutorial_done;
        public int current_id_main_mission;
        public bool open_mission_daily;
        public string _id;
        public string address;
        public string username;
        public Wallet ps;
        public Wallet prl;
        public int current_selected_character;
    }

}
#endregion

#region Wallet
[System.Serializable]
public struct Wallet
{
    public string contract_address;
    public decimal balance_dec;
    public string balance;
    public string symbol;
}
#endregion

[System.Serializable]
public struct Position
{
    public float x;
    public float y;
    public float z;
}


[System.Serializable]
public struct RecordItemInventory
{
    public string id;
    public string name;
    public string type;
    public string icon;
    public Prefab prefab;
    public int amount;
    public bool is_equip;
    public bool is_hide_player;
    public int number_delete;
    public bool is_hide_item;
    [System.Serializable]
    public struct Prefab
    {
        public string name;
        public string parent;
        public float[] position;
    }
}
[System.Serializable]
public struct RecordItemEquip
{
    public string itemID;
    public InventoryItemType itemType;
    public bool is_equip;
}
[System.Serializable]
public struct ClassQuestionRecord
{
    public string urlAvatarImage;
    public string name;
    public int point;
}

[System.Serializable]
public struct RecordCharacter
{
    public int id;
    public string name;
    public string icon;
}
public struct RecordNetworkCharacter
{
    public bool isOurs;
    public int id;
}
[System.Serializable]
public struct RecordMainmenuCharacter3D
{
    public string name_model;
    public float[] position;
    public float scale;
    public string animator_controller;
}

[System.Serializable]
public struct NPCConversation
{
    public string[] conversation;
}


[System.Serializable]
public struct Paragons
{
    public Paragon[] paragon;
    [System.Serializable]
    public struct Paragon
    {
        public string paragon_id;
        public string paragon_name;
        public string paragon_power;
        public string paragon_price;

    }
}
[System.Serializable]
public struct RecordLinkBillBoard
{
    public string urlImage;
    public string urlLink;
}

[System.Serializable]
public struct RecordLinkAds
{
    public string urlImage;
}

[System.Serializable]
public struct RecordKeyCodeActionName
{
    public KeyCode key_code;
    public string action_name;
}

[System.Serializable]
public struct RecordKeyCodeAction
{
    public KeyCode key_code;
    public UnityAction action;
}

[System.Serializable]
public struct ColorSkin
{
    public string color_code;
}


public struct RecordMissionBoard
{
    public int mission_id;
    public string mission_type_name;
    public string mission_type;
    public string mission_name;
    public RecordReward reward;
    public bool is_complete;
    public int current_progress;
    public int distance;
    public string mission_place;
    public string mission_detail;
    public float[] target_position;
}
[System.Serializable]
public struct RecordItemReward
{
    public int id_item;
    public string name;
    public string itemType;
    public string icon;
    public int quantity;
}

