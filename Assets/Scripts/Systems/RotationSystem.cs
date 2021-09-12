using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS
{
    public class RotationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float speed = 5.0f;
            float deltaTime = Time.DeltaTime * speed;

            Entities.WithAll<RotationComponent>().ForEach((ref Rotation rotation, in RotationComponent rotationComponent) =>
            {
                rotation.Value = math.mul(rotation.Value, quaternion.EulerXYZ(rotationComponent.AngularVelocity * deltaTime));
            }).ScheduleParallel();
        }
    }
}