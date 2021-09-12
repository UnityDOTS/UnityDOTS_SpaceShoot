using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct RotationComponent : IComponentData
    {
        public float3 AngularVelocity; // 角速率
    }
}