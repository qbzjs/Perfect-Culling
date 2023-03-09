// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.32
// 

using Colyseus.Schema;

public partial class NetworkedEntityState : Schema {
	[Type(0, "string")]
	public string id = default(string);

	[Type(1, "number")]
	public float xPos = default(float);

	[Type(2, "number")]
	public float yPos = default(float);

	[Type(3, "number")]
	public float zPos = default(float);

	[Type(4, "number")]
	public float xRot = default(float);

	[Type(5, "number")]
	public float yRot = default(float);

	[Type(6, "number")]
	public float zRot = default(float);

	[Type(7, "number")]
	public float wRot = default(float);

	[Type(8, "number")]
	public float timestamp = default(float);

	[Type(9, "uint8")]
	public byte status = default(byte);

	[Type(10, "uint8")]
	public byte character_id = default(byte);

	[Type(11, "string")]
	public string address = default(string);

	[Type(12, "string")]
	public string username = default(string);

	[Type(13, "boolean")]
	public bool visible_ow = default(bool);
}

