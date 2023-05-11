shader_type canvas_item;

uniform bool canDie;

void fragment()
{
	if(canDie)
	{
		COLOR = texture(TEXTURE, UV);
		COLOR.r *= 1.2;
	}
}
