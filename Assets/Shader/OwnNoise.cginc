#include "FractalNoiseBase.cginc"

inline float NoiseFuncPlain(sampler2D latticeArray, float latticeSize, float x, float y, int kmax, float lacunarity, float h, bool derive = false, bool deriveAfterX = true) {

	float currHeight = 0,
		 der = 0,
		c = 0.0;
	//currHeight = NoiseFuncPlain(latticeArray, latticeSize, x, y, --k, lacunarity, h);
	for (int k = 0; k < kmax; k++) {
		/*if (currHeight >= 0 || k <=3) {
			float val = S1F(latticeArray, latticeSize, x*pow(lacunarity, k), y * pow(lacunarity, k)) / (!derive ? pow(lacunarity, k * h) : 1);
			der += S1F(latticeArray, latticeSize, x*pow(lacunarity, k), y * pow(lacunarity, k), derive, deriveAfterX);
			currHeight += (currHeight + 1) / 2 * val;
		}*/

		if (k <= min(1, min(1, ((currHeight + 1) + c) / 2) / 0.7) * (kmax-1)) {
			float val = S1F(latticeArray, latticeSize, x*pow(lacunarity, k), y * pow(lacunarity, k)) / (!derive ? pow(lacunarity, k * h) : 1);
			der += S1F(latticeArray, latticeSize, x*pow(lacunarity, k), y * pow(lacunarity, k), derive, deriveAfterX);
			currHeight += FadeFunction((currHeight +1) / 2) * val;
		}
	}

	return (!derive ? currHeight : der);
}



inline float GetFractalNoiseHeight(sampler2D latticeArray, float latticeSize, float x, float y, int kmax, float lacunarity, float h) {
	return NoiseFuncPlain(latticeArray, latticeSize, x, y, kmax, lacunarity, h);
}


inline float GetFractalNoiseDerivative(sampler2D latticeArray, float latticeSize, float x, float y, int kmax, float lacunarity, float h, bool deriveAfterX) {
	return NoiseFuncPlain(latticeArray, latticeSize, x, y, kmax, lacunarity, h, true, deriveAfterX);
}