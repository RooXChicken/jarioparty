shader_type canvas_item;

//uniform float line_thickness : hint_range(0, 20) = 1.0;    // thickness of the line
uniform float sin_frequency : hint_range(0.1, 2.0) = 0.2;  // frequency of the rainbow
//uniform float sin_offset : hint_range(0.0, 360.0) = 0.0;   // offset of the rainbow, useful to differentiate objects using the same shader
uniform float light_offset : hint_range(0.0, 1.0) = 0.5;   // this offsets all color channels; if set to 0 only red green and blue colors will be shown.

void fragment()
{
		vec4 animated_line_color = vec4(light_offset + sin(2.0*3.14*sin_frequency*TIME),
		   light_offset + sin(2.0*3.14*sin_frequency*TIME + radians(120.0)),
		   light_offset + sin(2.0*3.14*sin_frequency*TIME + radians(240.0)),
		   1.0);
							
	//vec4 color = texture(TEXTURE, UV);
	COLOR.rgb = animated_line_color.rgb;
	//COLOR.r = animated_line_color.r;
	//COLOR.g = animated_line_color.g;
	//COLOR.b = animated_line_color.b;
	//if(outline == 1.0)
	{
		//COLOR.r = 0.0;
		//COLOR.g = 0.0;
		//COLOR.b = 0.0;
		//COLOR.a = 1.0;
	}
}
