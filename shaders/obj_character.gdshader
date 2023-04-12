shader_type canvas_item;

uniform float mult;

void fragment()
{
	COLOR = texture(TEXTURE, UV);
	COLOR = COLOR.rbga;
	highp int index = int(UV.x * mult);
    if(index % 2 == 0)
	{
        //COLOR.a = 0.0;
	}
}
