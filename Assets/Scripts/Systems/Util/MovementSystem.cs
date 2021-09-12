using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS
{
    public class MovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            Entities.WithAll<MovementComponent>().ForEach((ref Translation translation, in MovementComponent movementComponent) =>
            {
                translation.Value += movementComponent.Direction * movementComponent.Speed * deltaTime;
                translation.Value.x = math.clamp(translation.Value.x, -5f, 5f);
            }).Run();
        }
    }
}