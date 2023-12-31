#include "FractalNoiseBase.cginc"


inline float NoiseFuncPlain(sampler2D latticeArray, float latticeSize, float x, float y, int kmax, float lacunarity, float h, bool derive = false, bool deriveAfterX = true) {

	float currHeight = 1,
		o = 0.2;
	//currHeight = NoiseFuncPlain(latticeArray, latticeSize, x, y, --k, lacunarity, h);

	for (int k = 0; k < kmax; k++) {
		float val = (1-abs((S1FValueNoise(latticeArray, latticeSize, x * pow(lacunarity, k), y * pow(lacunarity, k), derive, deriveAfterX) + o) / (!derive ? pow(lacunarity, k * h) : 1)));
		currHeight *= val;
	}

	return currHeight;
}



inline float GetFractalNoiseHeight(sampler2D latticeArray, float latticeSize, float x, float y, int kmax, float lacunarity, float h) {
	return NoiseFuncPlain(latticeArray, latticeSize, x, y, kmax, lacunarity, h);
}


inline float GetFractalNoiseDerivative(sampler2D latticeArray, float latticeSize, float x, float y, int kmax, float lacunarity, float h, bool deriveAfterX) {
	return NoiseFuncPlain(latticeArray, latticeSize, x, y, kmax, lacunarity, h, true, deriveAfterX);
}