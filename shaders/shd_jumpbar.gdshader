shader_type canvas_item;

uniform float size;

void fragment()
{
	COLOR = texture(TEXTURE, UV);
	if(UV.y < 1.0 - size)
		COLOR.a = 0.0;
		
	COLOR.r = 1.0 - size;
	COLOR.g *= 1.1 * size;
}
