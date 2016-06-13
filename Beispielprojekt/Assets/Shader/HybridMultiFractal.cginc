#include "FractalNoiseBase.cginc"

inline float NoiseFuncPlain(sampler2D latticeArray, float latticeSize, float x, float y, int kmax, float lacunarity, float h, bool derive = false, bool deriveAfterX = true) {

	float offset = 0.5, 
		currHeight = S1F(latticeArray, latticeSize, x, y, derive, deriveAfterX) + offset,
		weight = currHeight*currHeight;
	//currHeight = NoiseFuncPlain(latticeArray, latticeSize, x, y, --k, lacunarity, h);


	for (int k = 1; k < kmax; k++) {
		// Clamp weight 
		weight = min(1, weight);

		// Get local value for current frequency
		float val = (S1F(latticeArray, latticeSize, x * pow(lacunarity, k), y * pow(lacunarity, k), derive, deriveAfterX) + offset) / (!derive ? pow(lacunarity, k * h) : 1);

		currHeight += weight * val;

		weight *= val * 1.15;
	}

	return currHeight;
}



inline float GetFractalNoiseHeight(sampler2D latticeArray, float latticeSize, float x, float y, int kmax, float lacunarity, float h) {
	return NoiseFuncPlain(latticeArray, latticeSize, x, y, kmax, lacunarity, h);
}


inline float GetFractalNoiseDerivative(sampler2D latticeArray, float latticeSize, float x, float y, int kmax, float lacunarity, float h, bool deriveAfterX) {
	return NoiseFuncPlain(latticeArray, latticeSize, x, y, kmax, lacunarity, h, true, deriveAfterX);
}