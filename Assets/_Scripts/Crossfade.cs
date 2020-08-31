//using FMODUnity;
using UnityEngine;

public class Crossfade : MonoBehaviour
{
    public float ActivateTime;
    public float DeactivateTime;
    public bool Active;
    public bool PlaySfx = true;

    [Header("SFX")]
    //[EventRef] public string ActivateSfx;
    //[EventRef] public string ActiveLoopSfx;
    //[EventRef] public string DeactivateSfx;
    //[EventRef] public string MixerSnapshot;

    //private FmodEvent _activateEvent;
    //private FmodEvent _activeLoopEvent;
    //private FmodEvent _deactivateEvent;
    //private FmodEvent _mixerSnapshot;

    private MeshRenderer _renderer;

    private bool _oldActive;

    private float _currentIn = 1.5f;
    private float _targetIn = 1.5f;
    private float _inVelocity;
    
    private float _currentOut = 1.5f;
    private float _targetOut = 1.5f;
    private float _outVelocity;
    
    private static readonly int UpperDissolve = Shader.PropertyToID("_UpperDissolve");
    private static readonly int LowerDissolve = Shader.PropertyToID("_LowerDissolve");

    private void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        //if (string.IsNullOrEmpty(MixerSnapshot)) return;
        
        //_activateEvent = FmodEvent.Create(ActivateSfx, gameObject);
        //_activeLoopEvent = FmodEvent.Create(ActiveLoopSfx, gameObject);
        //_deactivateEvent = FmodEvent.Create(DeactivateSfx, gameObject);
        //_mixerSnapshot = FmodEvent.Create(MixerSnapshot, gameObject);
    }

    private void Update()
    {
        if (Active && !_oldActive) // became active
        {
            _currentOut = -0.5f;
            _targetOut = -0.5f;

            _currentIn = -0.5f;
            _targetIn = 1.5f;

            if (PlaySfx)
            {
                //_activateEvent?.Play();
                //_activeLoopEvent?.Play();
                //_mixerSnapshot?.Play();
            }
        }
        else if (!Active && _oldActive) // became inactive
        {
            _currentOut = -0.5f;
            _targetOut = 1.5f;

            if (PlaySfx)
            {
                //_activeLoopEvent?.Stop();
                //_deactivateEvent?.Play();
                //_mixerSnapshot?.Stop();
            }
        }

        _oldActive = Active;

        var speed = Active ? ActivateTime : DeactivateTime;
        _currentIn = Mathf.SmoothDamp(_currentIn, _targetIn, ref _inVelocity, speed);
        _currentOut = Mathf.SmoothDamp(_currentOut, _targetOut, ref _outVelocity, speed);
        
        _renderer.material.SetFloat(UpperDissolve, _currentIn);
        _renderer.material.SetFloat(LowerDissolve, _currentOut);
    }
}
