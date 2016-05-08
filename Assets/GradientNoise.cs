using UnityEngine;

public class GradientNoise : AbstractNoise {

    public delegate Vector2 AuxFunxDel(int x, int y);

    private AuxFunxDel AuxFunc;

    private Vector2[,] _auxArray;


    public GradientNoise(int seed = 0, int auxSize = 6, float maxVal = 0.1f) : base(seed, auxSize, maxVal) {
        _auxArray = new Vector2[AuxSize, AuxSize];
        if (seed == 0)
            FillAuxArray();
        else
            FillAuxArray(seed);

        AuxFunc = GetFromAuxArray;
    }


    protected override float GetAuxFuncFloat(int x, int y) {
        return GetFromAuxArray(x, y).x;
    }


    /// <summary>
    /// Naming convention roughly after http://staffwww.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    protected override float S1(float x, float y) {
        int floorX = Mathf.FloorToInt(x),
            ceilX = Mathf.CeilToInt(x),
            floorY = Mathf.FloorToInt(y),
            ceilY = Mathf.CeilToInt(y);
        float tx = x - floorX,
              ty = y - floorY;
        //Calc slopes
        float  n00 = Vector2.Dot(AuxFunc(floorX, floorY), new Vector2(tx, ty)), n10 = Vector2.Dot(AuxFunc(ceilX, floorY), new Vector2(tx-1, ty)),
               n01 = Vector2.Dot(AuxFunc(floorX, ceilY), new Vector2(tx, ty-1)), n11 = Vector2.Dot(AuxFunc(ceilX, ceilY), new Vector2(tx - 1, ty - 1));

        float retVal = (FadeFunction(1 - ty) *
                        (n00 * FadeFunction(1 - tx) + n10 * FadeFunction(tx))
                        +
                        FadeFunction(ty) *
                        (n01 * FadeFunction(1 - tx) + n11 * FadeFunction(tx)));

        //Debug.Log(String.Format("x: {0} y: {1} fadefunc for adjusted x: {2} and aux top left {3}  ->  {4}", 
        //            tx, ty, FadeFunction(tx), _auxFunc(floorX, floorY), retVal));

        return retVal;
    }


    private Vector2 GetFromAuxArray(int x, int y) {
        return _auxArray[x, y];
    }

    private void FillAuxArray(int seed = 0) {
        if (seed != 0)
            Random.seed = seed;

        for (int i = 0; i <= _auxArray.GetUpperBound(0); i++) {
            for (int j = 0; j <= _auxArray.GetUpperBound(1); j++) {
                _auxArray[i, j] = new Vector2(Random.value * 2 * MaxVal - MaxVal,
                                              Random.value * 2 * MaxVal - MaxVal);
            }
        }
    }
}
