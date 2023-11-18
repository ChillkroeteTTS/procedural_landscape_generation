/**
 * 
 * @param int                  left    left coordinate of current rectangle
 * @param int                  top     top coordinate of current rectangle
 * @param int                  right   right coordinate of current rectangle
 * @param int                  bottom  bottom coordinate of current rectangle
 * @param PseudoRandomFunction rnd     PNRG with values between -1 and 1
 * @param float                r       Lacunarity, factor with which offset will be decreased each recursion step
 * @param int                  recStep Number of the current recursion step
 */
public void DiamondSquare(int left, int top, int right, int bottom, PseudoRandomFunction rnd, float r, int recStep) {
    Assert.IsTrue(Heightmap.GetLength(0) % 2 == 1 && Heightmap.GetLength(1) % 2 == 1);
    
    // Diamond step
    int xCenter = left + (right - left)/2,
        yCenter = top + (bottom - top)/2;

    Heightmap[xCenter, yCenter] = BillinearInterpolation(Heightmap[left, top],
                                                         Heightmap[right, top],
                                                         Heightmap[left, bottom],
                                                         Heightmap[right, bottom], 0.5f, 0.5f) + rnd() / Mathf.Pow(r, recStep+1);

    // Square step
    Heightmap[left, yCenter]   = LinearInterpolation(Heightmap[left, top], Heightmap[left, bottom], 0.5f)    + rnd()  / Mathf.Pow(r, recStep+1);
    Heightmap[xCenter, top]    = LinearInterpolation(Heightmap[left, top], Heightmap[right, top], 0.5f)      + rnd() / Mathf.Pow(r, recStep+1);
    Heightmap[right, yCenter]  = LinearInterpolation(Heightmap[right, top], Heightmap[right, bottom], 0.5f)  + rnd() / Mathf.Pow(r, recStep+1);
    Heightmap[xCenter, bottom] = LinearInterpolation(Heightmap[left, bottom], Heightmap[right, bottom], 0.5f)+ rnd() / Mathf.Pow(r, recStep+1);
    
    // Recursion if uncalculated pixel left
    if (right - left >= 2) {
        yield return DiamondSquare(left, top, xCenter, yCenter, rnd, r, recStep+1); // Top left
        yield return DiamondSquare(xCenter, top, right, yCenter, rnd, r, recStep+1); // Top right
        yield return DiamondSquare(left, yCenter, xCenter, bottom, rnd, r, recStep+1); // bottom left
        yield return DiamondSquare(xCenter, yCenter, right, bottom, rnd, r, recStep+1); // bottom left
        if (!UseSteps)
            UpdateTerrainHeightmap();
    }
}