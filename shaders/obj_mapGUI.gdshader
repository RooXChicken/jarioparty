shader_type canvas_item;

uniform float alpha;

void fragment()
{
	COLOR = texture(TEXTURE, UV);
	COLOR.a *= alpha;
}
