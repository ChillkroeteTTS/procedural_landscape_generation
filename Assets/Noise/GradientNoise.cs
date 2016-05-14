using UnityEngine;

public class GradientNoise : AbstractNoise {

    public delegate Vector2 AuxFunxDel(int x, int y);

    private AuxFunxDel LatticeFunc;

    private Vector2[,] _latticeArray;


    public GradientNoise(int seed = 0, int auxSize = 6, float maxVal = 0.1f) : base(seed, auxSize) {
        _latticeArray = new Vector2[AuxSize, AuxSize];
        if (seed == 0)
            FillLatticeArray();
        else
            FillLatticeArray(seed);

        LatticeFunc = GetFromLatticeArray;
    }


    protected override float GetLatticeFuncFloat(int x, int y) {
        return GetFromLatticeArray(x, y).x;
    }


    /// <summary>
    /// Naming convention roughly after http://staffwww.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    protected override float S(float x, float y) {
        int floorX = Mathf.FloorToInt(x),
            ceilX = Mathf.CeilToInt(x),
            floorY = Mathf.FloorToInt(y),
            ceilY = Mathf.CeilToInt(y);
        float tx = x - floorX,
              ty = y - floorY;
        //Calc slopes
        float  n00 = Vector2.Dot(LatticeFunc(floorX, floorY), new Vector2(tx, ty)), n10 = Vector2.Dot(LatticeFunc(ceilX, floorY), new Vector2(tx-1, ty)),
               n01 = Vector2.Dot(LatticeFunc(floorX, ceilY), new Vector2(tx, ty-1)), n11 = Vector2.Dot(LatticeFunc(ceilX, ceilY), new Vector2(tx - 1, ty - 1));

        float retVal = (FadeFunction(1 - ty) *
                        (n00 * FadeFunction(1 - tx) + n10 * FadeFunction(tx))
                        +
                        FadeFunction(ty) *
                        (n01 * FadeFunction(1 - tx) + n11 * FadeFunction(tx)));

        //Debug.Log(String.Format("x: {0} y: {1} fadefunc for adjusted x: {2} and aux top left {3}  ->  {4}", 
        //            tx, ty, FadeFunction(tx), _auxFunc(floorX, floorY), retVal));

        return retVal;
    }


    private Vector2 GetFromLatticeArray(int x, int y) {
        return _latticeArray[x, y];
    }

    private void FillLatticeArray(int seed = 0) {
        if (seed != 0)
            Random.seed = seed;
        Seed = Random.seed;

        for (int i = 0; i <= _latticeArray.GetUpperBound(0); i++) {
            for (int j = 0; j <= _latticeArray.GetUpperBound(1); j++) {
                _latticeArray[i, j] = new Vector2(Random.value * 2 - 1,
                                            Random.value * 2 - 1);
                if (i == _latticeArray.GetUpperBound(0)) {
                    _latticeArray[i, j] = _latticeArray[0, j];
                }
                if (j == _latticeArray.GetUpperBound(1)) {
                    _latticeArray[i, j] = _latticeArray[i, 0];
                }
                if (i == _latticeArray.GetUpperBound(0)
                    && j == _latticeArray.GetUpperBound(1)) {
                    _latticeArray[i, j] = _latticeArray[0, 0];
                }
                    /*_latticeArray[i, j] = new Vector2(RandomFromDistribution.RandomRangeNormalDistribution(-1f, 1f,
                    RandomFromDistribution.ConfidenceLevel_e._999),
                                                      RandomFromDistribution.RandomRangeNormalDistribution(-1f, 1f,
                    RandomFromDistribution.ConfidenceLevel_e._999));*/
            }
        }
    }
}
