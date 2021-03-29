struct VSInput {
	float4 position : POSITION;
	float3 normal : NORMAL;
	float2 texcoord : TEXCOORD;
};

struct PSInput
{
	float4 position : SV_POSITION;
	float3 normal : NORMAL;
	float2 texcoord : TEXCOORD;
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

PSInput VSMain(VSInput input)
{
	PSInput result = (PSInput)0;
}

float4 PSMain(PSInput input)
{

}