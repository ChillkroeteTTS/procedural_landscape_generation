#include "FractalNoiseBase.cginc"

inline float NoiseFuncPlain(sampler2D latticeArray, float latticeSize, float x, float y, int kmax, float lacunarity, float h, bool derive = false, bool deriveAfterX = true) {

	float currHeight = 0,
		derX = 0,
		derZ = 0;
	//currHeight = NoiseFuncPlain(latticeArray, latticeSize, x, y, --k, lacunarity, h);
	for (int k = 0; k < kmax; k++) {
		/*if (currHeight >= 0 || k <=3) {
			float val = S1F(latticeArray, latticeSize, x*pow(lacunarity, k), y * pow(lacunarity, k)) / (!derive ? pow(lacunarity, k * h) : 1);
			der += S1F(latticeArray, latticeSize, x*pow(lacunarity, k), y * pow(lacunarity, k), derive, deriveAfterX);
			currHeight += (currHeight + 1) / 2 * val;
		}*/

		float minSlopeForAllOctaves = 0.6;
				
		/*if (abs(derX) > minSlopeForAllOctaves || abs(derZ) > minSlopeForAllOctaves
			|| k <= min(1, pow(FadeFunction(((currHeight + 1) / 2) / 1.1), 1.5)) * (kmax-1)) {*/
		if (k <= min(1, pow(FadeFunction(((currHeight + 1) / 2) / 1.1), 1.5)) * (kmax-1) * max(1, abs(derZ) / minSlopeForAllOctaves) * max(1, abs(derZ) / minSlopeForAllOctaves)) {/**/
			float val = S1F(latticeArray, latticeSize, x*pow(lacunarity, k), y * pow(lacunarity, k)) / (!derive ? pow(lacunarity, k * h) : 1);
			derX += S1F(latticeArray, latticeSize, x*pow(lacunarity, k), y * pow(lacunarity, k), derive, true);
			derZ += S1F(latticeArray, latticeSize, x*pow(lacunarity, k), y * pow(lacunarity, k), derive, false);
			currHeight += FadeFunction((currHeight +1) / 2) * val;
		}
	}

	return (!derive ? currHeight : (deriveAfterX ? derX : derZ));
}



inline float GetFractalNoiseHeight(sampler2D latticeArray, float latticeSize, float x, float y, int kmax, float lacunarity, float h) {
	return NoiseFuncPlain(latticeArray, latticeSize, x, y, kmax, lacunarity, h);
}


inline float GetFractalNoiseDerivative(sampler2D latticeArray, float latticeSize, float x, float y, int kmax, float lacunarity, float h, bool deriveAfterX) {
	return NoiseFuncPlain(latticeArray, latticeSize, x, y, kmax, lacunarity, h, true, deriveAfterX);
}