// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.32
// 

using Colyseus.Schema;

public partial class ClassRoomState : Schema
{
	[Type(0, "map", typeof(MapSchema<ClassEntityState>))]
	public MapSchema<ClassEntityState> users = new MapSchema<ClassEntityState>();

	[Type(1, "string")]
	public string project_current_mode = default(string);

	[Type(2, "map", typeof(MapSchema<ProjectorModeState>))]
	public MapSchema<ProjectorModeState> projector_modes = new MapSchema<ProjectorModeState>();
}

