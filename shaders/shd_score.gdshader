shader_type canvas_item;

uniform vec3 newColor;

void fragment()
{
	COLOR = texture(TEXTURE, UV);
	if(COLOR.g < 0.5)
	{
		COLOR.r *= newColor.x;
		if(COLOR.g != 0.0)
			COLOR.g = 0.0;
		else
			COLOR.g = newColor.y;
		COLOR.b *= newColor.z;
	}
}
