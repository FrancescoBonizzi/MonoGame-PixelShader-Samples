﻿// Effect applies normalmapped lighting from i punctual light to a 2D sprite.

float3 LightPosition; // in World Space
float3 LightColor = 1.0;
float3 AmbientColor = 0.35;

float4x4 World;
float4x4 ViewProjection;

Texture2D ScreenTexture;
Texture2D NormalTexture;

SamplerState TextureSampler = sampler_state
{
	Texture = <ScreenTexture>;
};

SamplerState NormalSampler = sampler_state
{
	Texture = <NormalTexture>;
};

struct VertexShaderInput
{
	float4 Position: POSITION0;
	float2 TexCoords : TEXCOORD0;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
	float4 Position: POSITION0;
	float4 PosWorld: POSITION1;
	float2 TexCoords : TEXCOORD0;
	float4 Color : COLOR0;
};

VertexShaderOutput VS(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 pos = mul(input.Position, World);
	output.PosWorld = pos; // handing over WorldSpace Coordinates to PS
	output.Position = mul(pos, ViewProjection);

	// fill other fields of output
	output.TexCoords = input.TexCoords;
	output.Color = input.Color;

	return output;
}

float4 PS(VertexShaderOutput input) : COLOR0
{
	// input.PosWorld how has the Position of this Pixel in World Space
	float3 lightdir = normalize(input.PosWorld - LightPosition); // this is now the direction of light for this pixel

	// Look up the texture value
	float4 tex = ScreenTexture.Sample(TextureSampler, input.TexCoords);

	// Look up the normalmap value
	//float4 normal = 2 * NormalTexture.Sample(NormalSampler, input.TexCoords) - 1;
	float3 normal = normalize((2 * NormalTexture.Sample(NormalSampler, input.TexCoords)) - 1);

	// do the same, what you do for a regular directional light as you now have the light direction for this pixel and that pointlight, you may also want to calculate fallof distance etc depending on distance - this is where it gets pretty specific
	// TODO Introduce fall-off of light intensity
	// TODO diffuseLighting *= (LightDistanceSquared / dot(LightPosition - input.PosWorld, LightPosition - input.PosWorld));

	// Compute lighting
	float lightAmount = saturate(dot(normal, -lightdir));
	input.Color.rgb *= AmbientColor + (lightAmount * LightColor);
	
	return input.Color * tex;

	// if you have multiple pointlights, do a loop over every light you have and combine the outcome
}

technique PointLightNormalMap
{
	pass Pass1
	{
		VertexShader = compile vs_4_0_level_9_1 VS();
		PixelShader = compile ps_4_0_level_9_1 PS();
	}
}