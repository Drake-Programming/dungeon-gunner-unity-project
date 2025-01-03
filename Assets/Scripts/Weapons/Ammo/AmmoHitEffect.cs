using UnityEngine;

[DisallowMultipleComponent]
public class AmmoHitEffect : MonoBehaviour
{
    private ParticleSystem ammoHitEffectParticleSystem;

    private void Awake()
    {
        ammoHitEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Set Ammo Hit Effect from passed in AmmoHitEffectSO details
    /// </summary>
    /// <param name="ammoHitEffect"></param>
    public void SetHitEffect(AmmoHitEffectSO ammoHitEffect)
    {
        // Set shoot effect color gradient
        SetHitEffectColorGradient(ammoHitEffect.colorGradient);

        // Set shoot effect particle system starting values
        SetHitEffectParticleStartingValues(ammoHitEffect.duration, ammoHitEffect.startParticleSize,
            ammoHitEffect.startParticleSpeed, ammoHitEffect.startLifetime, ammoHitEffect.effectGravity, ammoHitEffect.maxParticleNumber);

        // Set shoot effect particle system particle burst particle number
        SetHitEffectParticleEmission(ammoHitEffect.emissionRate, ammoHitEffect.burstParticleNumber);

        // Set shoot effect particle sprite
        SetHitEffectParticleSprite(ammoHitEffect.sprite);

        // Set shoot effect lifetime min and max velocities
        SetHitEffectVelocityOverLifetime(ammoHitEffect.velocityOverLifetimeMin, ammoHitEffect.velocityOverLifetimeMax);
    }

    /// <summary>
    /// Set the hit effect particle system color gradient
    /// </summary>
    /// <param name="gradient"></param>
    private void SetHitEffectColorGradient(Gradient gradient)
    {
        // Set color gradient
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = ammoHitEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = gradient;
    }

    /// <summary>
    /// Set hit effect particle system starting values
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="startParticleSize"></param>
    /// <param name="startParticleSpeed"></param>
    /// <param name="startLifetime"></param>
    /// <param name="effectGravity"></param>
    /// <param name="maxParticles"></param>
    private void SetHitEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed,
        float startLifetime, float effectGravity, int maxParticles)
    {
        ParticleSystem.MainModule mainModule = ammoHitEffectParticleSystem.main;

        // Set particle system duration
        mainModule.duration = duration;

        // Set particle system size
        mainModule.startSize = startParticleSize;

        // Set particle speed
        mainModule.startSpeed = startParticleSpeed;

        // Set particle lifetime
        mainModule.startLifetime = startLifetime;

        // Set particle gravity
        mainModule.gravityModifier = effectGravity;

        // Set max particles
        mainModule.maxParticles = maxParticles;
    }

    /// <summary>
    /// Set hit effect particle system particle burst particle number
    /// </summary>
    /// <param name="emissionRate"></param>
    /// <param name="burstParticleNumber"></param>
    private void SetHitEffectParticleEmission(int emissionRate, float burstParticleNumber)
    {
        ParticleSystem.EmissionModule emissionModule = ammoHitEffectParticleSystem.emission;

        // Set particle burst number
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);

        // Set particle emission rate
        emissionModule.rateOverTime = emissionRate;
    }

    /// <summary>
    /// Set hit effect particle system sprite
    /// </summary>
    /// <param name="sprite"></param>
    private void SetHitEffectParticleSprite(Sprite sprite)
    {
        // Set particle burst number
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = ammoHitEffectParticleSystem.textureSheetAnimation;

        textureSheetAnimationModule.SetSprite(0, sprite);
    }

    /// <summary>
    /// Set the hit effect velocity over lifetime
    /// </summary>
    /// <param name="minVelocity"></param>
    /// <param name="maxVelocity"></param>
    private void SetHitEffectVelocityOverLifetime(Vector3 minVelocity, Vector3 maxVelocity)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = ammoHitEffectParticleSystem.velocityOverLifetime;

        // Define min max X velocity
        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveX.constantMin = minVelocity.x;
        minMaxCurveX.constantMax = maxVelocity.x;
        velocityOverLifetimeModule.x = minMaxCurveX;

        // Define min max Y velocity
        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.constantMin = minVelocity.y;
        minMaxCurveY.constantMax = maxVelocity.y;
        velocityOverLifetimeModule.y = minMaxCurveY;

        // Define min max Z velocity
        ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve();
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveZ.constantMin = minVelocity.z;
        minMaxCurveZ.constantMax = maxVelocity.z;
        velocityOverLifetimeModule.z = minMaxCurveZ;
    }
}
