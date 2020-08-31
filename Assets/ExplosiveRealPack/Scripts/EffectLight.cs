//A simple script to enable a light to change color and intensity over life and adapt to a particle systems lifetime and looping behaviour.
using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Light))]
public class EffectLight : MonoBehaviour {

	public float duration = -1f;
	public bool looping;
	public float delay = -1f;
	public float lifetime = -1f;
	public AnimationCurve intensityOverLifetime = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
	public Gradient colorOverLifetime;
	float startTime;
	float originalIntensity;
	Color originalColor;
	Light targetLight;

	void Awake () {
		UpdateSettings();
	}

	void OnEnable()
	{
		startTime = Time.time + delay;
	}
	
	void Update ()
	{
		if(startTime > Time.time || startTime + lifetime < Time.time)
		{
			targetLight.enabled = false;

		}
		else if(startTime + lifetime > Time.time && duration > 0)
		{
			targetLight.enabled = true;
			float normalizedLife = (Time.time - startTime)/lifetime;
			targetLight.intensity =  originalIntensity * intensityOverLifetime.Evaluate(normalizedLife);
			targetLight.color = originalColor * colorOverLifetime.Evaluate(normalizedLife);
		}

		if(looping && ( startTime + duration ) < Time.time)
			startTime = Time.time;
	}
	
	void UpdateSettings ()
	{
		targetLight = GetComponent<Light>();
		originalIntensity = targetLight.intensity;
		originalColor = targetLight.color;
		ParticleSystem targetSystem = null;
		if(GetComponent<ParticleSystem>())
			targetSystem = GetComponent<ParticleSystem>();
		else if(transform.parent.GetComponent<ParticleSystem>())
			targetSystem = transform.parent.GetComponent<ParticleSystem>();
		if(targetSystem != null)
		{
			if(duration < 0)
				duration = targetSystem.duration;
			if(delay < 0)
				delay = targetSystem.startDelay;
			if(lifetime < 0)
				lifetime = targetSystem.startLifetime;
			if(delay > 0)
				targetLight.enabled = false;
		}
		else
		{
			Debug.LogWarning("Effect light has to be attached to gameobject with a particle system component or to a gameobject whos parent has a particle system component.", gameObject);
		}
	}
}