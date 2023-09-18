extends CharacterBody3D


const SPEED = 5.0
const JUMP_VELOCITY = 4.5

@export var camera_rig: Node3D;

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = ProjectSettings.get_setting("physics/3d/default_gravity")
@onready var animation_player = $visuals/character/AnimationPlayer
@onready var visuals = $visuals


func _physics_process(delta):
	# Add the gravity.
	if not is_on_floor():
		if (animation_player.current_animation != "Jump_Idle"):
			animation_player.play("Jump_Full_Short")
		velocity.y -= gravity * delta

	# Handle Jump.
	if Input.is_action_just_pressed("ui_accept") and is_on_floor():
		if (animation_player.current_animation != "Jump_Start"):
			animation_player.play("Jump_Start")
		velocity.y = JUMP_VELOCITY

	# Get the input direction and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.
	var input_dir = Input.get_vector("move_left", "move_right", "move_forward", "move_backward")
	var direction = (transform.basis * Vector3(input_dir.x, 0, input_dir.y)).normalized()
	if direction:
		if (camera_rig != null):
			visuals.look_at(position - camera_rig.transform.basis.z)
		
		if (animation_player.current_animation != "Walking_A"):
			animation_player.play("Walking_A")
		velocity.x = direction.x * SPEED
		velocity.z = direction.z * SPEED
		velocity = velocity.rotated(Vector3.UP, camera_rig.rotation.y)
	else:
		velocity.x = move_toward(velocity.x, 0, SPEED)
		velocity.z = move_toward(velocity.z, 0, SPEED)
		if (animation_player.current_animation != "Idle"):
			animation_player.play("Idle")

	print(camera_rig.get_global_transform().basis.z)	

	move_and_slide()
