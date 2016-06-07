#pragma target 4.0

static int permutation[256] = { 151,160,137,91,90,15,                 // Hash lookup table as defined by Ken Perlin.  This is a randomly
    131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,    // arranged array of all numbers from 0-255 inclusive.
    190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
    88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
    77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
    102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
    135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
    5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
    223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
    129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
    251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
    49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
    138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
};


inline float FadeFunction(float x) {
	return x * x * x * (x * (x * 6 - 15) + 10);
}


// Labels after https://graphics.tudelft.nl/Publications-new/2008/Car08/Thesis-Giliam-Final.pdf p. 123
inline float wDerivative(float t) {
	return 30 * pow(t, 4) - 60 * pow(t, 3) + 30 * pow(t, 2);
}

inline float twDerivative(float t) {
	return 36 * pow(t, 5) - 75 * pow(t, 4) + 40 * pow(t, 3);
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
inline float S(sampler2D latticeArray, float latticeSize, float x, float y, bool derive=false, bool deriveAfterX = true) {
	float floorX = floor(x),
		ceilX = ceil(x),
		floorY = floor(y),
		ceilY = ceil(y),
		tx = x - floorX,
		ty = y - floorY;
	float2 G00 = LatticeFunc(latticeArray, latticeSize, floorX, floorY), G10 = LatticeFunc(latticeArray, latticeSize, ceilX, floorY),
		   G01 = LatticeFunc(latticeArray, latticeSize, floorX, ceilY),  G11 = LatticeFunc(latticeArray, latticeSize, ceilX, ceilY);
	if (!derive) {
		float  n00 = dot(G00, float2(tx, ty)), n10 = dot(G10, float2(tx - 1, ty)),
			   n01 = dot(G01, float2(tx, ty - 1)), n11 = dot(G11, float2(tx - 1, ty - 1));
		//Calc slopes

		float retVal = (FadeFunction(1 - ty) *
			(n00 * FadeFunction(1 - tx) + n10 * FadeFunction(tx))
			+
			FadeFunction(ty) *
			(n01 * FadeFunction(1 - tx) + n11 * FadeFunction(tx)));

		//return (LatticeFunc(latticeArray, latticeSize, x, y).x + 1) / 2;
		return retVal;
	}
	else {
		float w = deriveAfterX ? FadeFunction(ty) : FadeFunction(tx);
		return deriveAfterX 
			?
			   (G00.x + (G01.x - G00.x)*w)+
			   (-G10.x+ (G10.y - G00.y)*ty + (G01.y - G11.x - G11.y + G10.x + (G00.y-G01.y+G11.y-G10.y)*ty)*w)*wDerivative(tx)+
			   (G10.x - G00.x + (G00.x - G01.x + G11.x - G10.x)*w)*twDerivative(tx)
			:
				(G00.y + (G10.y - G00.y)*w)+
				(-G01.y + (G01.x - G00.x)*tx + (G10.x-G11.y - G11.x + G01.y + (G00.x - G10.x + G11.x - G01.x)*tx)*w)*wDerivative(ty)+
				(G01.y-G00.y+(G00.y-G10.y+G11.y-G01.y)*w)*twDerivative(ty);
	}
}



/*
x && y € [0-1]*/
float S1F(sampler2D latticeArray, float latticeSize, float x, float y, bool derive = false, bool deriveAfterX = true) {
	float truncX = x - floor(x),
		truncY = y - floor(y);
	return S(latticeArray, latticeSize, (x-floor(x))*(latticeSize - 1), (y - floor(y)) * (latticeSize - 1), derive, deriveAfterX);
}



inline float SValueNoise(sampler2D latticeArray, float latticeSize, float x, float y, bool derive = false, bool deriveAfterX = true) {
	float floorX = floor(x),
		ceilX = ceil(x),
		floorY = floor(y),
		ceilY = ceil(y),
		tx = x - floorX,
		ty = y - floorY;
	float2 G00 = LatticeFunc(latticeArray, latticeSize, floorX, floorY), G10 = LatticeFunc(latticeArray, latticeSize, ceilX, floorY),
		G01 = LatticeFunc(latticeArray, latticeSize, floorX, ceilY), G11 = LatticeFunc(latticeArray, latticeSize, ceilX, ceilY);
	if (!derive) {

		float retVal = (FadeFunction(1 - ty) *
			(G00.x * FadeFunction(1 - tx) + G10.x * FadeFunction(tx))
			+
			FadeFunction(ty) *
			(G01.x * FadeFunction(1 - tx) + G11.x * FadeFunction(tx)));

		//return (LatticeFunc(latticeArray, latticeSize, x, y).x + 1) / 2;
		return retVal;
	}
	else {
		float w = deriveAfterX ? FadeFunction(ty) : FadeFunction(tx);
		return 0;
	}
}


/*
x && y € [0-1]*/
float S1FValueNoise(sampler2D latticeArray, float latticeSize, float x, float y, bool derive = false, bool deriveAfterX = true) {
	float truncX = x - floor(x),
		truncY = y - floor(y);
	return SValueNoise(latticeArray, latticeSize, (x - floor(x))*(latticeSize - 1), (y - floor(y)) * (latticeSize - 1), derive, deriveAfterX);
}