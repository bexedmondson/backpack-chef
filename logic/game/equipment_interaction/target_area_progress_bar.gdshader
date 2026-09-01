shader_type canvas_item;
render_mode unshaded;

uniform vec4 default_colour : source_color = vec4(0.6, 0, 0, 1.0);
uniform vec4 target_area_colour : source_color = vec4(0, 0.9, 0, 1.0);
instance uniform float target_area_min_value = 15;
instance uniform float target_area_max_value = 25;
instance uniform float bar_max_value = 40;


void fragment() {
    vec4 original_colour = COLOR;
	
	float target_area_min_proportion = target_area_min_value / bar_max_value;
	float target_area_max_proportion = target_area_max_value / bar_max_value;
	
	vec3 final_colour = mix(default_colour.rgb, target_area_colour.rgb, float(UV.x >= target_area_min_proportion && UV.x <= target_area_max_proportion));
	
    COLOR = vec4(final_colour, original_colour.a);
}
