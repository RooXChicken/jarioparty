extends AnimatedSprite2D

var thread
# Called when the node enters the scene tree for the first time.
func _ready():
	thread = Thread.new()
	thread.start(self, "_play_animation")
	
func _play_animation():
	play();
