#version 430

layout(location = 0) out vec4 outColor;

layout(std430, binding = 2) buffer shader_data
{
    float[] pixels;
};

void main()
{
    // vec2 loc = gl_FragCoord.xy;
    // int index = int(loc);
    // outColor = (pixels[index] > 0) ? vec4(1.0f, 1.0f, 1.0f, 1.0f) : vec4(0.0f, 0.0f, 0.0f, 1.0f);
    // outColor = vec4(loc, 1.0f, 1.0f);

    // vec2 p = gl_FragCoord.xy;

    vec2 pos = mod(gl_FragCoord.xy, vec2(50.0)) - vec2(25.0);
    float dist_squared = dot(pos, pos);
 
     outColor = (dist_squared < 400.0) 
         ? vec4(.90, .90, .90, 1.0)
         : vec4(.20, .20, .40, 1.0);
}