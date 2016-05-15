#pragma target 4.0
inline float FadeFunction(float x) {
	return x * x * x * (x * (x * 6 - 15) + 10);
}

/*
x & y € [0-(lattisize-1)]*/
inline float2 LatticeFunc(sampler2D latticeArray, float latticeSize, float x, float y) {
	float4 col = tex2Dlod(latticeArray, float4(x / (latticeSize-1), y / (latticeSize-1), 0, 0));
	return float2(col.r, col.g) * 2 - float2(1, 1);
	//return x / (latticeSize - 1) * 2 - 1;
}


/*
x && y € [0-(lattisize-1)]*/
inline float S(sampler2D latticeArray, float latticeSize, float x, float y) {
	float floorX = floor(x),
		ceilX = ceil(x),
		floorY = floor(y),
		ceilY = ceil(y),
		tx = x - floorX,
		ty = y - floorY;
	//Calc slopes
	float  n00 = dot(LatticeFunc(latticeArray, latticeSize, floorX, floorY), float2(tx, ty)), n10 = dot(LatticeFunc(latticeArray, latticeSize, ceilX, floorY), float2(tx - 1, ty)),
		n01 = dot(LatticeFunc(latticeArray, latticeSize, floorX, ceilY), float2(tx, ty - 1)), n11 = dot(LatticeFunc(latticeArray, latticeSize, ceilX, ceilY), float2(tx - 1, ty - 1));

	float retVal = (FadeFunction(1 - ty) *
		(n00 * FadeFunction(1 - tx) + n10 * FadeFunction(tx))
		+
		FadeFunction(ty) *
		(n01 * FadeFunction(1 - tx) + n11 * FadeFunction(tx)));

	//return (LatticeFunc(latticeArray, latticeSize, x, y).x + 1) / 2;
	return retVal;
}

/*
x && y € [0-1]*/
float S1F(sampler2D latticeArray, float latticeSize, float x, float y) {
	float truncX = x - floor(x),
		truncY = y - floor(y);
	return S(latticeArray, latticeSize, (x-floor(x))*(latticeSize - 1), (y - floor(y)) * (latticeSize - 1));
}



inline float NoiseFuncPlain(sampler2D latticeArray, float latticeSize, float x, float y, int k, float lacunarity, float h) {

	float currHeight = 0;
	//currHeight = NoiseFuncPlain(latticeArray, latticeSize, x, y, --k, lacunarity, h);

	for (int i = 0; i < k; i++) {
		float val = S1F(latticeArray, latticeSize, x * pow(lacunarity, k), y * pow(lacunarity, k)) / pow(lacunarity, k * h);
		currHeight += val;
	}

	return S1F(latticeArray, latticeSize, x * pow(lacunarity, 0), y * pow(lacunarity, 0)) / pow(lacunarity, 0 * h);
}


inline float GetFractalNoiseHeight(sampler2D latticeArray, float latticeSize, float x, float y, int k, float lacunarity, float h) {
	return NoiseFuncPlain(latticeArray, latticeSize, x, y, k, lacunarity, h);
}