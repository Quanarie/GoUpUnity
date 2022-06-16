using UnityEngine;
using GlobalTypes;

public class GroundEffector : MonoBehaviour
{
    [SerializeField] private GroundType groundType;

    public GroundType GroundType() => groundType;
}
    