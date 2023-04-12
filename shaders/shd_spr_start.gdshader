shader_type canvas_item;

uniform vec3 input;

void fragment()
{
	COLOR = texture(TEXTURE, UV);
	if(COLOR.r == 0.0 && COLOR.g == 1.0)
	{
		COLOR.rgb = input;
	}
}
