using System.Collections.Generic;
using UnityEngine;

public class Interpolator : MonoBehaviour
{
    [SerializeField] private InterpolatorMode mode;

    private List<TransformUpdate> futureTransformUpdates = new List<TransformUpdate>(); // Oldest first

    private TransformUpdate to;
    private TransformUpdate from;
    private TransformUpdate previous;

    [SerializeField] private float timeElapsed = 0f;
    [SerializeField] private float timeToReachTarget = 0.1f;

    [SerializeField] private bool isLocalRotation = false;
    
    private void Start()
    {
        to = new TransformUpdate(GameLogic.instance.tick, transform, isLocalRotation);
        from = new TransformUpdate(GameLogic.instance.delayTick, transform, isLocalRotation);
        previous = new TransformUpdate(GameLogic.instance.delayTick, transform, isLocalRotation);
    }

    private void Update()
    {
        for (int i = 0; i < futureTransformUpdates.Count; i++)
        {
            if (GameLogic.instance.tick >= futureTransformUpdates[i].tick)
            {
                previous = to;
                to = futureTransformUpdates[i];
                from = new TransformUpdate(GameLogic.instance.delayTick, transform, isLocalRotation);
                futureTransformUpdates.RemoveAt(i);
                timeElapsed = 0;
                timeToReachTarget = (to.tick - from.tick) * GameLogic.instance.secPerTick;
            }
        }
        timeElapsed += Time.deltaTime;
        Interpolate(timeElapsed / timeToReachTarget);
    }
    
    private void Interpolate(float _lerpAmount)
    {
        switch (mode)
        {
            case InterpolatorMode.both:
                if (isLocalRotation)
                {
                    InterpolatePosition(_lerpAmount);
                    InterpolateLocalRotation(_lerpAmount);
                }
                else
                {
                    InterpolatePosition(_lerpAmount);
                    InterpolateRotation(_lerpAmount);
                }
                break;
            case InterpolatorMode.position:
                InterpolatePosition(_lerpAmount);
                break;
            case InterpolatorMode.rotation:
                if (isLocalRotation)
                {
                    InterpolateLocalRotation(_lerpAmount);
                }
                else
                {
                    InterpolateRotation(_lerpAmount);
                }
                break;
        }
    }

    private void InterpolatePosition(float _lerpAmount)
    {
        if (to.position == previous.position)
        {
            // If this object isn't supposed to be moving, we don't want to interpolate and potentially extrapolate
            if (to.position != from.position)
            {
                // If this object hasn't reached it's intended position
                if (to.position != Vector3.zero)
                transform.position = Vector3.Lerp(from.position, to.position, _lerpAmount); // Interpolate with the _lerpAmount clamped so no extrapolation occurs
            }
            return;
        }
        if (to.position != Vector3.zero)
        transform.position = Vector3.LerpUnclamped(from.position, to.position, _lerpAmount); // Interpolate with the _lerpAmount unclamped so it can extrapolate
    }

    private void InterpolateRotation(float _lerpAmount)
    {
        if (to.rotation == previous.rotation)
        {
            // If this object isn't supposed to be rotating, we don't want to interpolate and potentially extrapolate
            if (to.rotation != from.rotation)
            {
                // If this object hasn't reached it's intended rotation
                transform.rotation = Quaternion.Slerp(from.rotation, to.rotation, _lerpAmount); // Interpolate with the _lerpAmount clamped so no extrapolation occurs
            }
            return;
        }

        transform.rotation = Quaternion.SlerpUnclamped(from.rotation, to.rotation, _lerpAmount); // Interpolate with the _lerpAmount unclamped so it can extrapolate
    }

    private void InterpolateLocalRotation(float _lerpAmount)
    {
        if (to.rotation == previous.rotation)
        {
            // If this object isn't supposed to be rotating, we don't want to interpolate and potentially extrapolate
            if (to.rotation != from.rotation)
            {
                // If this object hasn't reached it's intended local rotation
                transform.localRotation = Quaternion.Slerp(from.rotation, to.rotation, _lerpAmount); // Interpolate with the _lerpAmount clamped so no extrapolation occurs
            }
            return;
        }
        transform.localRotation = Quaternion.SlerpUnclamped(from.rotation, to.rotation, _lerpAmount); // Interpolate with the _lerpAmount unclamped so it can extrapolate
    }

    internal void NewUpdate(int _tick, Vector3 _position, Quaternion _rotation)
    {
        if (_tick <= GameLogic.instance.delayTick)
        {
            return;
        }

        if (futureTransformUpdates.Count == 0)
        {
            futureTransformUpdates.Add(new TransformUpdate(_tick, _position, _rotation));
            return;
        }

        for (int i = 0; i < futureTransformUpdates.Count; i++)
        {
            if (_tick < futureTransformUpdates[i].tick)
            {
                // Transform update is older
                futureTransformUpdates.Insert(i, new TransformUpdate(_tick, _position, _rotation));
                break;
            }
        }
    }
    internal void NewUpdate(int _tick, Vector3 _position)
    {
        if (_tick <= GameLogic.instance.delayTick)
        {
            return;
        }

        if (futureTransformUpdates.Count == 0)
        {
            futureTransformUpdates.Add(new TransformUpdate(_tick, _position));
            return;
        }

        for (int i = 0; i < futureTransformUpdates.Count; i++)
        {
            if (_tick < futureTransformUpdates[i].tick)
            {
                // Position update is older
                futureTransformUpdates.Insert(i, new TransformUpdate(_tick, _position));
                break;
            }
        }
    }
    internal void NewUpdate(int _tick, Quaternion _rotation)
    {
        if (_tick <= GameLogic.instance.delayTick)
        {
            return;
        }

        if (futureTransformUpdates.Count == 0)
        {
            futureTransformUpdates.Add(new TransformUpdate(_tick, _rotation));
            return;
        }

        for (int i = 0; i < futureTransformUpdates.Count; i++)
        {
            if (_tick < futureTransformUpdates[i].tick)
            {
                // Rotation update is older
                futureTransformUpdates.Insert(i, new TransformUpdate(_tick, _rotation));
                break;
            }
        }
    }

    enum InterpolatorMode
    {
        both,
        position,
        rotation
    }
}
