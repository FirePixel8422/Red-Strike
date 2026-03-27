using UnityEngine;


/// <summary>
/// Filter Component used to force enable the gameObject its attached to on scene load.
/// </summary>
[DisallowMultipleComponent]
[AddComponentMenu("")]
public sealed class ForceStateOnLoad : MonoBehaviour
{
    [SerializeField] private bool targetState = true;
    public bool TargetState => targetState;
}