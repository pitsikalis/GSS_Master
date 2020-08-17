using UnityEngine;

public class NoiseShake : MonoBehaviour
{
    public float Amplitude = 1f;
    public float Frequency = 1f;

    private Vector3 _originalLocalPos;
    private Vector2 _x, _y, _z;

    private void Awake()
    {
        var t = transform;
        _originalLocalPos = t.localPosition;
        
        var pos = t.position;
        _x = new Vector2(pos.x, pos.z);
        _y = _x + new Vector2(50f, -50f);
        _z = _x + new Vector2(-50f, 50f);
    }

    private void Update()
    {
        var seedDelta = new Vector2(Frequency * Time.deltaTime, Frequency * Time.deltaTime);

        _x += seedDelta;
        _y += seedDelta;
        _z += seedDelta;

        var xOffset = Mathf.Lerp(-Amplitude, Amplitude, Mathf.PerlinNoise(_x.x, _x.y));
        var yOffset = Mathf.Lerp(-Amplitude, Amplitude, Mathf.PerlinNoise(_y.x, _y.y));
        var zOffset = Mathf.Lerp(-Amplitude, Amplitude, Mathf.PerlinNoise(_z.x, _z.y));

        transform.localPosition = _originalLocalPos + new Vector3(xOffset, yOffset, zOffset);
    }
}
