shader_type canvas_item;

uniform vec4 dark_color : source_color =
vec4(0.067, 0.004, 0.11, 1.0);

uniform vec4 mid_color : source_color =
vec4(0.741, 0.275, 0.631, 1.0);

uniform vec4 bright_color : source_color =
vec4(0.918, 0.216, 0.984, 1.0);

uniform float noise_size = 45.0;
uniform float noise_speed = 3.25;
uniform float noise_strength = 0.48;

uniform float bubble_cell_size = 12.0;
uniform float bubble_speed = 8.0;
uniform float bubble_thickness = 0.65;
uniform float bubble_brightness = 0.55;

uniform float bubble_keep_chance = 0.72;

varying vec2 local_pixel_pos;

void vertex()
{
    local_pixel_pos = VERTEX;
}

vec2 hash22(vec2 p)
{
    p = vec2(
        dot(p, vec2(127.1, 311.7)),
        dot(p, vec2(269.5, 183.3))
    );

    return -1.0 + 2.0 * fract(
        sin(p) * 43758.5453123
    );
}

float perlin(vec2 p)
{
    vec2 i = floor(p);
    vec2 f = fract(p);

    vec2 u =
    f * f * f *
            (f * (f * 6.0 - 15.0) + 10.0);

    float a = dot(
        hash22(i + vec2(0.0, 0.0)),
        f - vec2(0.0, 0.0)
    );

    float b = dot(
        hash22(i + vec2(1.0, 0.0)),
        f - vec2(1.0, 0.0)
    );

    float c = dot(
        hash22(i + vec2(0.0, 1.0)),
        f - vec2(0.0, 1.0)
    );

    float d = dot(
        hash22(i + vec2(1.0, 1.0)),
        f - vec2(1.0, 1.0)
    );

    return mix(
        mix(a, b, u.x),
        mix(c, d, u.x),
        u.y
    ) * 0.5 + 0.5;
}

float fbm(vec2 p)
{
    float value = 0.0;
    float weight = 0.0;
    float amplitude = 0.55;

    for (int i = 0; i < 4; i++)
    {
        value +=
                perlin(p) * amplitude;

        weight += amplitude;

        p =
                p * 2.03 +
                        vec2(13.7, 8.9);

        amplitude *= 0.48;
    }

    return value / weight;
}

float hash21(vec2 p)
{
    return fract(
        sin(
            dot(
                p,
                vec2(127.1, 311.7)
            )
        ) * 43758.5453
    );
}

float bubble_field(vec2 pixel_pos)
{
    vec2 p =
    pixel_pos / bubble_cell_size;
    
    p.y +=
            TIME *
                    bubble_speed /
                    bubble_cell_size;

    vec2 base_cell =
    floor(p);

    vec2 local =
    fract(p) *
            bubble_cell_size;

    float result = 0.0;
    
    for (int y = -1; y <= 1; y++)
    {
        for (int x = -1; x <= 1; x++)
        {
            vec2 offset =
            vec2(
                float(x),
                float(y)
            );

            vec2 cell =
            base_cell + offset;
            
            float eligible =
            step(
                0.5,
                mod(
                    cell.x + cell.y,
                    2.0
                )
            );

            if (eligible <= 0.0)
                continue;
            
            float keep =
            step(
                1.0 - bubble_keep_chance,
                hash21(cell + 41.3)
            );

            if (keep <= 0.0)
                continue;
            
            vec2 center =
            vec2(
                mix(
                    0.18,
                    0.82,
                    hash21(cell + 3.7)
                ),
                mix(
                    0.18,
                    0.82,
                    hash21(cell + 9.1)
                )
            ) *
                    bubble_cell_size;

            center +=
                    offset *
                            bubble_cell_size;
            
            float radius =
            mix(
                3.5,
                9.0,
                hash21(cell + 17.6)
            );
            
            float d =
            length(
                local - center
            );
            
            float ring =
            1.0 -
                    smoothstep(
                        bubble_thickness,
                        bubble_thickness + 0.75,
                        abs(d - radius)
                    );
            
            result =
                    max(
                        result,
                        ring
                    );
        }
    }

    return result;
}

void fragment()
{
    vec4 tex =
    texture(TEXTURE, UV);
    
    vec2 pixel_pos =
    local_pixel_pos;
    
    vec2 noise_pos =
    pixel_pos /
            noise_size;

    noise_pos +=
            vec2(
                TIME * noise_speed * 0.08,
                -TIME * noise_speed * 0.045
            );
    
    float warp_x =
    perlin(
        noise_pos * 0.55 +
                vec2(
                    TIME * 0.015,
                    0.0
                )
    );

    float warp_y =
    perlin(
        noise_pos * 0.55 +
                vec2(
                    17.0,
                    -TIME * 0.012
                )
    );
    
    vec2 warped =
    noise_pos +
            (
            vec2(
                warp_x,
                warp_y
            ) - 0.5
            ) * 0.9;
    
    float n =
    fbm(warped);
    
    float noise_mask =
    smoothstep(
        0.42,
        0.72,
        n
    ) *
            noise_strength;
    
    vec3 color =
    mix(
        dark_color.rgb,
        mid_color.rgb,
        noise_mask
    );
    
    float bubbles =
    bubble_field(pixel_pos);
    
    color =
            mix(
                color,
                bright_color.rgb,
                bubbles * bubble_brightness
            );
    
    COLOR =
            vec4(
                color,
                tex.a
            );
}