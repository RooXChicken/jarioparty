shader_type canvas_item;

uniform vec2 position;

void fragment()
{
	COLOR = texture(TEXTURE, UV);
	vec4 bounds = vec4(position.x / 1280.0, position.x + 64.0 / 1280.0, position.y / 720.0, position.y + 64.0 / 720.0);
	
	if(UV.x > bounds.x && UV.y > bounds.z && UV.x < bounds.y && UV.y < bounds.w)
	{
		COLOR.a = 0.0;
	}
}