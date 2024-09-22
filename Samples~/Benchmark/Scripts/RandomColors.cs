using System.Collections;
using UnityEngine;
using Unity.MaterialPropertyProvider;

public class RandomColors : MaterialPropertyProviderBase
{
    [SerializeField] private float _frequency = 3f;
    [SerializeField] AnimationCurve _lerpCurve = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0, 0, 0), new Keyframe(1, 1, 0, 0) });

    [SerializeField] Renderer[] _renderers;
    protected override Renderer[] renderers => _renderers;

    [SerializeField] bool _alwaysUseMaterialPropertyBlocks = false;
    protected override bool AlwaysUseMaterialPropertyBlocks => _alwaysUseMaterialPropertyBlocks;

    [MaterialProperty("_BaseColor")]
    Color Color { get; set; }

    Color _current, _next;
    WaitForSeconds wait;
    float timer = 0;

    protected override void Start()
    {
        base.Start();
        wait = new WaitForSeconds(_frequency);
        _current = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        _next = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        Color = _current;
        StartCoroutine(GenerateColors());
    }

    private IEnumerator GenerateColors()
    {
        while (true)
        {
            yield return wait;
            _current = _next;
            //_next = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            _next = Color.HSVToRGB(Random.Range(0f, 1f), 1f, 1f);
            timer = 0;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        Color = Color.Lerp(_current, _next, _lerpCurve.Evaluate(timer));
        UpdateProperties();
    }
}
