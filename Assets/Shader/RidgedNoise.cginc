#include "FractalNoiseBase.cginc"

inline float NoiseFuncPlain(sampler2D latticeArray, float latticeSize, float x, float y, int kmax, float lacunarity, float h, bool derive = false, bool deriveAfterX = true) {

	if (!derive) {
		float currHeight = 0,
			derivativeX = 0,
			derivativeY = 0,
			S = 1,
			capitalH = 0,
			dx = 0, dy = 0,
			w = 1 / pow(lacunarity, (1 + 2 * h) / 2),
			b = 1.1,
			a = 0.01,
			p = 2;
		//currHeight = NoiseFuncPlain(latticeArray, latticeSize, x, y, --k, lacunarity, h);

		for (int k = 0; k < kmax; k++) {
			float val = S1F(latticeArray, latticeSize, x * pow(lacunarity, k), y * pow(lacunarity, k), derive, deriveAfterX);
			derivativeX += S1F(latticeArray, latticeSize, x * pow(lacunarity, k), y * pow(lacunarity, k), true, true);
			derivativeY += S1F(latticeArray, latticeSize, x * pow(lacunarity, k), y * pow(lacunarity, k), true, false);
			S += S * b*min(1, capitalH);
			dx += S * pow(w, k) * (-val)*derivativeX;
			dy += S * pow(w, k) * (-val)*derivativeY;

			capitalH += pow((val + 1 / 2), p)  * S * pow(w, k) * (S1F(latticeArray, latticeSize, x * pow(lacunarity, k) + a*dx, y * pow(lacunarity, k) + a*dy, derive, deriveAfterX));

			currHeight += val;
		}

		return capitalH;
	}
	else {
		float currHeight = 0;
		//currHeight = NoiseFuncPlain(latticeArray, latticeSize, x, y, --k, lacunarity, h);

		for (int k = 0; k < kmax; k++) {
			float val = S1F(latticeArray, latticeSize, x * pow(lacunarity, k), y * pow(lacunarity, k), derive, deriveAfterX) / (!derive ? pow(lacunarity, k * h) : 1);
			currHeight += val;
		}
		return currHeight;
	}
}


inline float GetFractalNoiseHeight(sampler2D latticeArray, float latticeSize, float x, float y, int kmax, float lacunarity, float h) {
	return NoiseFuncPlain(latticeArray, latticeSize, x, y, kmax, lacunarity, h);
}


inline float GetFractalNoiseDerivative(sampler2D latticeArray, float latticeSize, float x, float y, int kmax, float lacunarity, float h, bool deriveAfterX) {
	return -NoiseFuncPlain(latticeArray, latticeSize, x, y, kmax, lacunarity, h)*NoiseFuncPlain(latticeArray, latticeSize, x, y, kmax, lacunarity, h, true, deriveAfterX);
}