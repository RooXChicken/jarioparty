shader_type canvas_item;

uniform int acc;

void fragment()
{
	COLOR = texture(TEXTURE, UV);
	//if(acc % 2 == 0)
		COLOR.a = 0;
}
