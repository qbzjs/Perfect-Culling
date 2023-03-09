// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.32
// 

using Colyseus.Schema;

public partial class ClassEntityState : Schema {
    [Type(0, "string")]
    public string id = default(string);

    [Type(1, "string")]
    public string ow_ss_id = default(string);

    [Type(2, "string")]
    public string role = default(string);

    [Type(3, "number")]
    public float timestamp = default(float);

    [Type(4, "uint8")]
    public byte character_id = default(byte);

    [Type(5, "string")]
    public string address = default(string);

    [Type(6, "string")]
    public string username = default(string);
}

