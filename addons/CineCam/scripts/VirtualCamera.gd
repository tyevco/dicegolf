extends Node
class_name VirtualCamera

const SOM = preload("res://addons/CineCam/scripts/preload/SecondOrderMotion.gd")

@export_group("Location Settings")
## How the virtual camera determines its location.
@export var location_type: FollowType = FollowType.Self
## Which Node3D's position the camera will be placed on. Only works if 
## a FollowNode type is selected.
@export var location_follow_node: Array[Node3D]

@export_subgroup("Location Damping")
## Whether the location following is damped or not.
@export var location_damp: bool = true
## Frequency, in Hz. Makes the movement bounce as it settles in place.
@export_range(0, 5) var location_f: float = 1
## Damping Coefficient, describes how the system settles on target.
@export_range(0, 2) var location_z: float = 1
## Initial response of the system. At 1, the system reacts immediately to input.
## Above 1, the sstem overshoots the target. Below 0, the motion is anticipated.
@export_range(-5, 5) var location_r: float = 0

@export_subgroup("Orbiting Settings")
## Vertical rotation.
@export_range(-3, 3) var tilt: float = 1
# TODO: pan (Horizontal rotation)
## Horizontal displacement.
@export_range(-3, 3) var track: float = 0
## Vertical displacement.
@export_range(-1, 3) var pedestal: float = 1
## Horizontal pivoting around location.
@export_range(-TAU, TAU) var yaw: float
## Vertical pivoting around location.
@export_range(-TAU/4 + 0.1, TAU/4 - 0.1) var pitch: float = .3
## Distance from location.
@export_range(0, 20) var radius: float = 3

@export_group("Target Settings")
## How the virtual camera determines its target.
@export var target_type: FollowType = FollowType.Self
## Which Node3D's position the camera will target. Only works if
## a FollowNode type is selected.
@export var target_follow_node: Array[Node3D]

@export_subgroup("Target Damping")
## Whether the target following is damped or not.
@export var target_damp: bool = true
## Frequency, in Hz. Makes the movement bounce as it settles in place.
@export_range(0, 5) var target_f: float = 1
## Damping Coefficient, describes how the system settles on target.
@export_range(0, 2) var target_z: float = 1
## Initial response of the system. At 1, the system reacts immediately to input.
## Above 1, the system overshoots the target. Below 0, the motion is anticipated.
@export_range(-5, 5) var target_r: float = 0

@onready var location: Node3D = $Location
@onready var target: Node3D = $Target

enum FollowType {
	FollowNode, ## Follows the first Node3D in the follow_node array.
	LerpNodes, ## Lerps between the coordinates of all the nodes in the follow_node array.
	Self ## Follows the Location/Target node inside this virtual camera.
}

var loc_dampener: SOM.SOMEngine
var target_dampener: SOM.SOMEngine

func _ready():
	match location_type:
		FollowType.FollowNode:
			loc_dampener = SOM.SOMEngine.new(
				location_follow_node[0].position, location_f, location_z, location_r)
		FollowType.Self:
			loc_dampener = SOM.SOMEngine.new(
				location.position, location_f, location_z, location_r)
		FollowType.LerpNodes: pass # TODO

	match target_type:
		FollowType.FollowNode:
			target_dampener = SOM.SOMEngine.new(
				target_follow_node[0].position, target_f, target_z, target_r)
		FollowType.Self:
			target_dampener = SOM.SOMEngine.new(
				target.position, target_f, target_z, target_r)
		FollowType.LerpNodes: pass # TODO

func _physics_process(delta):
	match location_type:
		FollowType.Self: if location_damp:
			location.position = loc_dampener.update_motion(delta, location.position)
		FollowType.FollowNode: if location_damp:
			location.position = loc_dampener.update_motion(delta, location_follow_node[0].position)
		else:
			location.position = location_follow_node[0].position
			
		FollowType.LerpNodes: pass # TODO

	match target_type:
		FollowType.Self: if target_damp:
			target.position = loc_dampener.update_motion(delta, target.position)
		FollowType.FollowNode: if target_damp:
			target.position = loc_dampener.update_motion(delta, target_follow_node[0].position)
		else:
			target.position = target_follow_node[0].position
			
		FollowType.LerpNodes: pass # TODO
