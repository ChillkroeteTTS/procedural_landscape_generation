public float FractalValueNoise(float x, float y, float lacunarity, float H, int k) {
  if (k <= 0) {
    return 0;
  }

  return FractalValueNoise(x, y, lacunarity, H, --k)
         + ValueNoise(x * Mathf.pow(lacunarity, k), y * Mathf.pow(lacunarity, k)) / Mathf.pow(lacunarity, k*H);
}