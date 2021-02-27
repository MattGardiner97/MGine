struct PSInput
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};

PSInput VSMain(float4 position : POSITION)
{
	PSInput result;

	result.position = position;
	result.color = float4(1,0,0,1);

	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
	return input.color;
}