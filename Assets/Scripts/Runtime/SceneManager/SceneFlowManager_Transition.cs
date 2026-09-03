using System.Collections;
using UnityEngine;
using TMPro;

public partial class SceneFlowManager
{
    private void TransitionInitialize()
    {
        if (_fadeGroup == null)
        {
            //경고 -> 인스펙터 확인
            return;
        }

        _fadeGroup.alpha = 0.0f;
        _fadeGroup.blocksRaycasts = false;
        _fadeGroup.interactable = false;

        Logger.Log("초기화 완료");
    }

    private IEnumerator Co_FadeTo(float targetAlpha, float duration = -1f, bool blockRaycastWhileFading = true)
    {
        if (_fadeGroup == null)
        {
            // 경고 - 비어있으니 확인 요망
            yield break;
        }
        if (duration < 0f)
        {
            // 기본 페이드 시간을 적용하겠다.
            duration = _fadeDuration;
        }

        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
            _fadeRoutine = null;
        }

        _fadeRoutine = StartCoroutine(Co_Fade_Internal(targetAlpha, duration, blockRaycastWhileFading));
        yield return _fadeRoutine;
        _fadeRoutine = null;
    }

    private IEnumerator Co_Fade_Internal(float targetAlpha, float duration, bool blockRaycastWhileFading)
    {
        // 현재 알파값을 시작 값으로 저장
        float startAlpha = _fadeGroup.alpha;
        // 페이드 중에는 막기
        _fadeGroup.blocksRaycasts = blockRaycastWhileFading;
        // 페이드 자체는 상호작용 UI가 아니므로 false
        _fadeGroup.interactable = false;

        // 지속시간이 없을 경우, 바로 맞춰주고
        // 너무 어둡다 싶을때 레이캐스트 막기
        if (duration <= 0f)
        {
            _fadeGroup.alpha = targetAlpha;
            _fadeGroup.blocksRaycasts = (targetAlpha >= 0.99f);
            yield break;
        }

        float t = 0f;

        while (t < duration)
        {
            // dt 선택
            // ㄴ Time.deltaTime : 타임 스케일 영향을 받음
            // ㄴ Time.unscaledTime : 타임 스케일 무시
            float dt = Time.unscaledDeltaTime;
            t += dt;
            float lerp = Mathf.Clamp01(t / duration);
            _fadeGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, lerp);
            yield return null; // 다음 프레임까지 대기

        }
        _fadeGroup.alpha = targetAlpha;
        _fadeGroup.blocksRaycasts = (targetAlpha >= 0.99f);
    }
}