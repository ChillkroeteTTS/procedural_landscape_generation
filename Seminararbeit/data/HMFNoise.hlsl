#include "FractalNoiseBase.cginc"

/**
 * Hybrid Noise Function
 * @param  float x            X Coordinate
 * @param  float y            Y Coordinate
 * @param  int   kmax         max Frequency
 * @param  float lacunarity   Lacunarity
 * @param  float h            H, fractal dimension
 */
inline float HybridMultiNoise(float x, float y, int kmax, float lacunarity, float h, float o, float w) {

  // calc first octave
  float currHeight = GradientNoise(x * pow(lacunarity, k),y * pow(lacunarity, k)) + o,
        weight = currHeight;

  for (int k = 1; k < kmax; k++) {
    // clamp weight
    weight = min(1, weight);

    float val = (GradientNoise(x * pow(lacunarity, k),y * pow(lacunarity, k)) + o) / pow(lacunarity, k * h);
    currHeight += weight * val;

    // update weight
    weight *= val * w;
  }

  return currHeight;
}