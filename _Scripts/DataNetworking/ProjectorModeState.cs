// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.32
// 

using Colyseus.Schema;

public partial class ProjectorModeState : Schema {
	[Type(0, "string")]
	public string mode = default(string);

	[Type(1, "string")]
	public string resource_link = default(string);

	[Type(2, "int32")]
	public int total_page = default(int);

	[Type(3, "int32")]
	public int page_id = default(int);

	[Type(4, "int8")]
	public sbyte page_state = default(sbyte);

	[Type(5, "number")]
	public float page_value = default(float);

	[Type(6, "number")]
	public float timestamp = default(float);
}

