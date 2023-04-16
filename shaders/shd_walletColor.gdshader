shader_type canvas_item;

uniform vec3 walletColor;
uniform float alpha;

void fragment()
{
	COLOR = texture(TEXTURE, UV);
	COLOR.a = alpha;
	COLOR.rgb = walletColor;
}
