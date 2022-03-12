shader_type canvas_item;

void fragment() {
	vec4 px = texture(TEXTURE, UV);
	float total_alpha = 0.0;
	for (int x = 0; x < 3; x++) {
		for (int y = 0; y < 3; y++) {
			total_alpha += texture(TEXTURE, UV + vec2(TEXTURE_PIXEL_SIZE.x * float(x - 1), TEXTURE_PIXEL_SIZE.y * float(y - 1))).a;
		}
	}
	if (total_alpha >= 1.0) {
		COLOR = vec4(0, 0, 0, 1.0 - px.a) + (vec4(px.r, px.g, px.b, 1.0) * px.a);
	}
	else {
		COLOR = px;
	}
}
