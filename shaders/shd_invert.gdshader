shader_type canvas_item;

void fragment()
{
	COLOR = texture(TEXTURE, UV);
	COLOR.r = 1.0 - COLOR.r;
	COLOR.g = 1.0 - COLOR.g;
	COLOR.b = 1.0 - COLOR.b;
}
