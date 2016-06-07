#include "FractalNoiseBase.cginc"

/**
 * Ridged Noise Function
 * @param  float x            X Coordinate
 * @param  float y            Y Coordinate
 * @param  int   kmax         max Frequency
 * @param  float lacunarity   Lacunarity
 * @param  float h            H, fractal dimension
 */
inline float RidgedNoise(float x, float y, int kmax, float lacunarity, float h) {

  float currHeight = 0;

  for (int k = 0; k < kmax; k++) {
    currHeight += (1-abs(GradientNoise(x * pow(lacunarity, k),y * pow(lacunarity, k)))) / pow(lacunarity, k * h);
  }

  return currHeight;
}