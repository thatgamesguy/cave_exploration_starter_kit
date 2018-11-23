using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions
{

	public static void EnableEmission (this ParticleSystem particleSystem, bool enabled)
	{
		var emission = particleSystem.emission;
		emission.enabled = enabled;
	}

	public static float GetEmissionRate (this ParticleSystem particleSystem)
	{
		return particleSystem.emission.rateOverTime.constantMax;
	}

	public static void SetEmissionRate (this ParticleSystem particleSystem, float emissionRate)
	{
		var emission = particleSystem.emission;
		var rate = emission.rateOverTime;
		rate.constantMax = emissionRate;
		rate.constantMin = emissionRate;
		emission.rateOverTime = rate;
	}

	private static System.Random rng = new System.Random();  

	public static void Shuffle<T>(this IList<T> list)  
	{  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}
}
