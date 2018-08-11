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

#endif