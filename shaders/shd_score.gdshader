shader_type canvas_item;

uniform vec3 newColor;

void fragment()
{
	COLOR = texture(TEXTURE, UV);
	
	COLOR.r *= newColor.x;
	COLOR.g *= newColor.y;
	COLOR.b *= newColor.z;
}
