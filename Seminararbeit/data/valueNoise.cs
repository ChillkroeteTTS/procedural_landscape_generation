public float ValueNoise(float x, float y) {
    int floorX = Mathf.FloorToInt(x),
        ceilX = Mathf.CeilToInt(x),
        floorY = Mathf.FloorToInt(y),
        ceilY = Mathf.CeilToInt(y);
    float tx = x - floorX,
          ty = y - floorY;

    return FadeFunction(1 - ty) *
           (LatticeFunc(floorX, floorY) * FadeFunction(1 - tx) + FadeFunction(tx) * LatticeFunc(ceilX, floorY))
           +
           FadeFunction(ty) *
           (LatticeFunc(floorX, ceilY)*FadeFunction(1 - tx) + FadeFunction(tx)*LatticeFunc(ceilX, ceilY));
}