shader_type canvas_item;

uniform float alpha;

void fragment(){
  COLOR = texture(TEXTURE, UV);
  COLOR.a *= alpha;
  if(COLOR.rgb != vec3(0, 0, 0))
  {
    COLOR.r -= 0.4;
    COLOR.bg += 0.4;
  }

  COLOR = vec4(1.0 - COLOR.rgb, COLOR.a);
}