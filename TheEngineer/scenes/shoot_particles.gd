extends Node3D

@onready var shotgun_particles: Array[GPUParticles3D] = _get_shotgun_particles()

func _get_shotgun_particles() -> Array[GPUParticles3D]:
	var result: Array[GPUParticles3D] = []

	for node in find_children("*", "GPUParticles3D", true, false):
		result.append(node as GPUParticles3D)

	return result


func fx_shotgun():
	for particles in shotgun_particles:
		particles.restart()
