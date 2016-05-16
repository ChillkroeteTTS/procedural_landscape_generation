#pragma target 4.0
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