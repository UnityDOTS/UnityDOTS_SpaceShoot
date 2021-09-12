using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct ShooterComponent : IComponentData
    {
        public float3 StartPoint;
        public Entity BulletEntity;
        public float ShootIntervalTime;
        public float ShootingCountdown;
    }
}