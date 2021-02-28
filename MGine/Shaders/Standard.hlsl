cbuffer objectCb {
	matrix worldViewProj;
};

struct VSInput {
	float4 position : POSITION;
};

struct PSInput
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};

PSInput VSMain(VSInput input)
{
	PSInput result = (PSInput)0;
	result.position = mul(input.position,worldViewProj);
	result.color = float4(1,0,0,1);

	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
	return input.color;
}