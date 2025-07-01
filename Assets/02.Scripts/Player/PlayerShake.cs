using System;
using System.Collections;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerShakingAbility : PlayerAbility
{
    // 무엇을 어떤 힘으로 몇초동안 흔들것인가
    public Transform Target;
    public float Strength;
    public float Duration;

    private CinemachineVirtualCamera _virtualCamera;
    
    private void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void ShakeCamera(CinemachineVirtualCamera camera, float amplitude, float frequency, float duration)
    {
        var noise = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise == null)
        {
            Debug.LogWarning("Noise component missing");
            return;
        }

        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;

        DOTween.To(() => noise.AmplitudeGain, x => noise.AmplitudeGain = x, 0f, duration)
            .SetEase(Ease.InQuad)
            .SetUpdate(true);

        DOTween.To(() => noise.FrequencyGain, x => noise.FrequencyGain = x, 0f, duration)
            .SetEase(Ease.InQuad)
            .SetUpdate(true);
    }
    
    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(Shake_Coroutine());
    }

    private IEnumerator Shake_Coroutine()
    {
        float elapsedTime = 0f;

        // 원위치 저장
        Vector3 startPosition = Target.localPosition;

        while (elapsedTime <= Duration)
        {
            elapsedTime += Time.deltaTime;
            
            // 흔들어 재낀다음
            Vector3 randomPosition = Random.insideUnitSphere.normalized * Strength;
            randomPosition.y = startPosition.y;
            Target.localPosition = randomPosition;
            
            yield return null;
        }
        
        // 원위치로
        Target.localPosition = startPosition;
    }

}