////// CONSTANTS //////
static const int MAX_POINT_LIGHTS = 32;
static const int MAX_DIRECTIONAL_LIGHTS = 8;

////// STRUCTURES //////
struct VSInput {
	float4 position : POSITION;
	float3 normal : NORMAL;
};

struct PSInput
{
	float4 position : SV_POSITION;
	float4 positionWorld : POSITION;
	float3 normal : NORMAL;
};

struct PointLight {
	float4 ambient;
	float4 diffuse;
	float4 specular;
	float3 position;
	float range;
	float3 attenuation;
	float pad0;
};

struct DirectionalLight {
	float4 ambient;
	float4 diffuse;
	float4 specular;
	float3 direction;
	float pad;
};

struct StandardMaterial
{
	float4 ambient;
	float4 diffuse;
	float4 specular;
};

////// CONSTANT BUFFERS //////
cbuffer frameCb : register(b0)
{
	float3 cameraPosition;
	float pad0;
}

cbuffer objectCb : register(b1) {
	matrix worldViewProj;
	matrix world;
};

cbuffer PointLightCB : register(b2) {
	PointLight pointLights[MAX_POINT_LIGHTS];
};

cbuffer DirectionalLightCB : register(b3)
{
	DirectionalLight directionalLights[MAX_DIRECTIONAL_LIGHTS];
}


cbuffer MaterialCB : register(b4) {
	StandardMaterial material;
};

////// LIGHTING FUNCTIONS //////
void ComputePointLight(PointLight light, float3 fragmentPosition, float3 fragmentNormal, float3 surfaceToCameraUnitVector, out float4 ambient, out float4 diffuse, out float4 specular)
{
	ambient = (float4)0;
	diffuse = (float4)0;
	specular = (float4)0;

	float3 lightVector = light.position - fragmentPosition;
	float surfaceToLightDistance = length(lightVector);
	if (surfaceToLightDistance > light.range)
		return;

	lightVector /= surfaceToLightDistance;

	ambient = material.ambient * light.ambient;

	float diffuseFactor = dot(lightVector, fragmentNormal);

	[flatten]
	if (diffuseFactor > 0.0f)
	{
		float3 reflectVector = reflect(-lightVector, fragmentNormal);
		float specularFactor = pow(max(dot(reflectVector, surfaceToCameraUnitVector), 0.0f), material.specular.w);
		diffuse = diffuseFactor * material.diffuse * light.diffuse;
		specular = specularFactor * material.specular * light.specular;

		float attenuation = 1.0f / dot(light.attenuation, float3(1.0f, surfaceToLightDistance, surfaceToLightDistance * surfaceToLightDistance));
		diffuse *= attenuation;
		specular *= attenuation;
	}
}

void ComputeDirectionalLight(DirectionalLight light, float3 fragmentNormal, float3 surfaceToCameraUnitVector, out float4 ambient, out float4 diffuse, out float4 specular)
{
	ambient = (float4)0;
	diffuse = (float4)0;
	specular = (float4)0;

	ambient = material.ambient * light.ambient;

	float3 lightVector = -light.direction;

	float diffuseFactor = dot(lightVector, fragmentNormal);

	[flatten]
	if (diffuseFactor > 0.0f)
	{
		float3 reflectVector = reflect(-lightVector, fragmentNormal);
		float specularFactor = pow(max(dot(reflectVector, surfaceToCameraUnitVector), 0.0f), material.specular.w);
		diffuse = diffuseFactor * material.diffuse * light.diffuse;
		specular = specularFactor * material.specular * light.specular;
	}
}

////// ENTRY POINTS //////
PSInput VSMain(VSInput input)
{
	PSInput result = (PSInput)0;
	result.position = mul(input.position, worldViewProj);
	result.positionWorld = mul(input.position, world);
	result.normal = mul(input.normal,world);

	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
	input.normal = normalize(input.normal);

float3 surfaceToCameraUnitVector = normalize(cameraPosition - input.positionWorld);

float4 ambient = (float4)0;
float4 diffuse = (float4)0;
float4 specular = (float4)0;

float4 ambientOut, diffuseOut, specularOut;
for (int i = 0; i < MAX_POINT_LIGHTS; i++)
{
	ComputePointLight(pointLights[i], input.positionWorld, input.normal, surfaceToCameraUnitVector, ambientOut, diffuseOut, specularOut);
	ambient += ambientOut;
	diffuse += diffuseOut;
	specular += specularOut;
}
for (int i = 0; i < MAX_DIRECTIONAL_LIGHTS; i++)
{
	ComputeDirectionalLight(directionalLights[i], input.normal, surfaceToCameraUnitVector, ambientOut, diffuseOut, specularOut);
	ambient += ambientOut;
	diffuse += diffuseOut;
	specular += specularOut;
}


float4 resultColour = (float4)0;
resultColour = ambient + diffuse + specular;
resultColour.w = 1;

return resultColour;
}

