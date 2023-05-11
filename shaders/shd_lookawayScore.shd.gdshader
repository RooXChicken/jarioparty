shader_type canvas_item;

uniform float remaining;

void fragment()
{
	COLOR = texture(TEXTURE, UV);
	if(UV.x > remaining && COLOR.g != 1.0)
	{
		COLOR.r *= 1.2;
		COLOR.g *= 1.0;
		COLOR.b = 0.2;
	}
	else if(COLOR.g != 1.0)
	{
		COLOR.r *= 0.08;
		COLOR.g *= 2.0;
		COLOR.b = 0.08;
	}
	else if(COLOR.g == 1.0)
	{
		COLOR.a = 0.0;
	}
}
