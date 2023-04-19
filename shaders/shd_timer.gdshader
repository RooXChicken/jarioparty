shader_type canvas_item;

uniform float multiplier;

void fragment()
{
	COLOR = texture(TEXTURE, UV);
	COLOR.r += multiplier;
}
