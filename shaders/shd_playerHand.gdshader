shader_type canvas_item;

uniform float progress;

void fragment()
{
	COLOR =  texture(TEXTURE, UV);
	if(COLOR.a > 0.1 && UV.y < 1.0-progress)
	{
		COLOR.r *= 0.2;
		COLOR.g *= 0.2;
		COLOR.b *= 0.2;
		//float grey = 0.21 * COLOR.r + 0.71 * COLOR.g + 0.07 * COLOR.b;
		//COLOR.rgba = vec4(COLOR.r * u_colorFactor + grey * (1.0 - u_colorFactor), COLOR.g * u_colorFactor + grey * (1.0 - u_colorFactor), COLOR.b * u_colorFactor + grey * (1.0 - u_colorFactor), 1.0);
	}
}
