using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastHandler<T>
{
    private Transform _source;
    private List<RaycastHit2D> _results = new List<RaycastHit2D>();
    private List<T> _targets = new List<T>();
    
    public RayCastHandler(Transform source)
    {
        _source = source;
    }

    public bool RayCastDown(int mask)
    {
        return RaycastUtils.RaycastAllTargets(_source.position, Vector2.down, mask,out _targets, out _results);
    }

    public List<T> GetTargets()
    {
        return _targets;
    }
}
