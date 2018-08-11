#ifndef SHADERLIB_INCLUDED
#define SHADERLIB_INCLUDED

float inverselerp(float a, float b, float x) {
	x = clamp(a, b, x);
	return (x - a) / (b - a);
}

float gain(float x, float k) {
	float a = 0.5 * pow(2*((x < 0.5) ? x : 1.0 - x), k);
	return (x < 0.5) ? a : 1.0 - a;
}

float impulse(float k, float x) {
	const float h = k*x;
	return h*exp(1.0 - h);
}

float expstep(float x, float k, float n) {
	return exp(-k*pow(x, n));
}

float diffuse_directional(float3 worldLightDir, float3 objectNormal) {
	return saturate(dot(UnityObjectToWorldNormal(objectNormal), worldLightDir));
}

float diffuse_point(float4 vertex, float3 worldLightPos, float3 objectNormal) {
	float3 lightDir = normalize(float4(worldLightPos, 1) - mul(unity_ObjectToWorld, vertex));
	return saturate(dot(UnityObjectToWorldNormal(objectNormal), lightDir));
}

float diffuse(float4 vertex, float4 worldLight, float3 objectNormal) {
	if (worldLight.w == 0) {
		return diffuse_directional(worldLight.xyz, objectNormal);
	} else {
		return diffuse_point(vertex, worldLight.xyz, objectNormal);
	}
}

float specular_directional(float4 vertex, float3 lightDir, float3 worldNormal, float exponent) {
	// Vertex should be passed in as v.vertex.
	float3 viewDir = normalize(WorldSpaceViewDir(vertex));
	float3 reflection = normalize(reflect(-lightDir, worldNormal));
	return pow(saturate(dot(reflection, viewDir)), exponent);
}

float specular_point(float4 vertex, float3 lightPos, float3 worldNormal, float exponent) {
	float3 viewDir = normalize(WorldSpaceViewDir(vertex));
	float3 lightDir = lightPos - mul(unity_ObjectToWorld, vertex);
	float3 reflection = normalize(reflect(-lightDir, worldNormal));
	return pow(saturate(dot(reflection, viewDir)), exponent);
}

#endif