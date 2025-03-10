using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePoolObject : PoolObjectBase
{
    [SerializeField] private ParticleSystem particle;

    public void Play() => particle.Play();
    public void Stop() => particle.Stop();

    private void OnEnable()
    {
        Play();
    }

    private void OnDisable()
    {
        Stop();
    }
}
