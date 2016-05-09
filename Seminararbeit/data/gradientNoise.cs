public float S(float x, float y) {
    int floorX = Mathf.FloorToInt(x),
        ceilX = Mathf.CeilToInt(x),
        floorY = Mathf.FloorToInt(y),
        ceilY = Mathf.CeilToInt(y);
    float tx = x - floorX,
          ty = y - floorY,
          //Calc slopes
          n00 = Vector2.Dot(LatticeFunc(floorX, floorY), new Vector2(tx, ty)), 
          n10 = Vector2.Dot(LatticeFunc(ceilX, floorY), new Vector2(tx-1, ty)),
          n01 = Vector2.Dot(LatticeFunc(floorX, ceilY), new Vector2(tx, ty-1)), 
          n11 = Vector2.Dot(LatticeFunc(ceilX, ceilY), new Vector2(tx - 1, ty - 1));

    return FadeFunction(1 - ty) *
           (n00 * FadeFunction(1 - tx) + n10 * FadeFunction(tx))
           +
           FadeFunction(ty) *
           (n01 * FadeFunction(1 - tx) + n11 * FadeFunction(tx));
}