using UnityEngine;

public class ParameterAnimatorSet : MonoBehaviour
{
    public string ParameterName;

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetBool(bool b)
    {
        if(_animator == null) _animator = GetComponent<Animator>();
        _animator.SetBool(ParameterName, b);
    }

    public void SetInt(int i)
    {
        if(_animator == null) _animator = GetComponent<Animator>();
        _animator.SetInteger(ParameterName, i);
    }

    public void SetParameterName(string parameterName)
    {
        ParameterName = parameterName;
    }
}
