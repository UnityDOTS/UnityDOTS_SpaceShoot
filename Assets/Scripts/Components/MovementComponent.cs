using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct MovementComponent : IComponentData
    {
        public float Speed;
        public float3 Direction;
    }
}