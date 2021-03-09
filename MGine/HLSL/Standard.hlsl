struct VSInput {
	float4 position : POSITION;
	float3 normal : NORMAL;
};

struct PSInput
{
	float4 position : SV_POSITION;
	float4 colour : COLOR;
	float3 normal : NORMAL;
};

struct Light
{
	float3 dir;
	float pad;
	float4 ambient;
	float4 diffuse;
};

cbuffer objectCb : register(b0) {
	matrix worldViewProj;
	matrix world;
};

cbuffer frameCb : register(b1) {
	Light light;
};

cbuffer colourCb : register(b2) {
	float4 colour;
};


PSInput VSMain(VSInput input)
{
	PSInput result = (PSInput)0;
	result.position = mul(input.position,worldViewProj);
	result.colour = colour;
	result.normal = mul(float4(input.normal,1.0), world);

	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
	input.normal = normalize(float4(input.normal,1.0));

	float3 finalColor;

	finalColor = input.colour * light.ambient;
	finalColor += saturate(dot(light.dir, -input.normal) * light.diffuse * input.colour);

	return float4(finalColor, input.colour.a);
}