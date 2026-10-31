using UnityEngine;

public class DayNightByHeight : MonoBehaviour
{
    [Header("높이 설정")]
    public Transform player;
    public float startY = 0f;     // 이 높이에서 아침
    public float goalY = 100f;    // 이 높이에서 완전한 밤
    public float smooth = 2f;     // 값이 부드럽게 따라가는 속도

    [Header("디버그")]
    public bool useManual = false;
    [Range(0f, 1f)] public float manualProgress = 0f;

    [Header("참조")]
    public Light sun;
    public Material skyMaterial;  // SkyBlendMat

    [Header("색상 (아침 → 저녁 → 밤)")]
    public Gradient sunColor;
    public Gradient fogColor;
    public Gradient ambientColor;
    public AnimationCurve sunIntensity = new AnimationCurve(
        new Keyframe(0f, 1.2f), new Keyframe(0.6f, 0.8f), new Keyframe(1f, 0.15f));
    public AnimationCurve skyExposure = new AnimationCurve(
        new Keyframe(0f, 1f), new Keyframe(1f, 1.3f));

    float current;

    void Start()
    {
        RenderSettings.skybox = skyMaterial;
        RenderSettings.fog = true;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.sun = sun;
    }

    void Update()
    {
        float target = useManual
            ? manualProgress
            : Mathf.Clamp01(Mathf.InverseLerp(startY, goalY, player.position.y));

        // 점프 중 값이 튀지 않도록 부드럽게 따라감
        current = Mathf.Lerp(current, target, 1f - Mathf.Exp(-smooth * Time.deltaTime));
        Apply(current);
    }

    void Apply(float t)
    {
        // 태양: 낮은 각도(아침) → 반대편 낮은 각도(밤, 달빛 역할)
        float angle = Mathf.Lerp(15f, 160f, t);
        sun.transform.rotation = Quaternion.Euler(angle, -30f, 0f);
        sun.color = sunColor.Evaluate(t);
        sun.intensity = sunIntensity.Evaluate(t);

        RenderSettings.fogColor = fogColor.Evaluate(t);
        RenderSettings.ambientLight = ambientColor.Evaluate(t);

        skyMaterial.SetFloat("_Blend", t);
        skyMaterial.SetFloat("_Exposure", skyExposure.Evaluate(t));
    }
}