#if TOOLS
using Godot;
using System;
using PleaseKissMyElbow.addons.components.scripts;

[Tool]
public partial class ComponentToolPlugin : EditorPlugin
{
	private static readonly Texture2D Texture = GD.Load<Texture2D>("res://addons/components/component_white.png");

	public override void _EnterTree()
	{
		AddCustomType<FollowableComponent>();
		AddCustomType<SelectableComponent>();
	}

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
		// Always remember to remove it from the engine when deactivated.
		RemoveCustomType<FollowableComponent>();
		RemoveCustomType<SelectableComponent>();
	}

	private void AddCustomType<T>()
		where T : Node
	{
		var typeName = typeof(T).Name;
        
		AddCustomType(typeName, typeof(T).BaseType?.Name, GD.Load<Script>($"res://addons/components/scripts/{typeName}.cs"), Texture);
	}

	private void RemoveCustomType<T>()
	{
		RemoveCustomType(typeof(T).Name);
	}
	
}
#endif
