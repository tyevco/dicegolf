using System.Linq;
using Godot;
using Godot.Collections;

public partial class Dice : RigidBody3D
{
	[Signal] public delegate void DiceRolledEventHandler(int result, Dice dice);

	[Export] public Dictionary<int, Vector3> Sides = new Dictionary<int, Vector3>();
	[Export] public float SettleDuration = 1.5f;

	private bool Rolled { get; set; }
	private float SettlingDurationSpent { get; set; }
	private bool Settling { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Rolled)
		{
			SettlingDurationSpent = 0;
			Settling = false;

			if (Sleeping)
			{
				Rolled = false;
				Settling = true;
			}
		}
		else
		{
			if (!Sleeping)
			{
				Settling = false;
				Rolled = true;
				SettlingDurationSpent = 0;
			}
		}

		if (Settling)
		{
			SettlingDurationSpent += (float)delta;

			if (SettlingDurationSpent >= SettleDuration)
			{
				// dice have settled.
				DetectRoll();

				Settling = false;
				SettlingDurationSpent = 0;
			}
		}
	}


	private void DetectRoll()
	{
		// do the side detection.
		Dictionary<int, float> dotProducts = new Dictionary<int, float>();

		var localUp = ToLocal(Position + Vector3.Up);

		foreach (var side in Sides)
		{
			dotProducts[side.Key] = side.Value.Dot(localUp);

			//GD.Print($"{this.Name} | {side.Key}");
		}

		//GD.Print(			$"{this.Name} | Rolling finished, determining results... {string.Join("; ", dotProducts.Where(x => x.Value > 0.001).OrderByDescending(x => x.Value).Take(3).Select(x => $"Side {x.Key} ({x.Value})"))}");

		var result = dotProducts.Where(x => x.Value > 0.001).OrderByDescending(x => x.Value).Select(x => x.Key)
			.FirstOrDefault();

		EmitSignal(SignalName.DiceRolled, result, this);
	}
}
