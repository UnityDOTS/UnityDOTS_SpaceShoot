using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS
{
    public class MovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var spaceBottomLeft = GameManager.SpaceBottomLeft;
            var spaceTopRight = GameManager.SpaceTopRight;
            var deltaTime = Time.DeltaTime;
            Entities.WithAll<MovementComponent>().ForEach((ref Translation translation, in MovementComponent movementComponent) =>
            {
                translation.Value += movementComponent.Direction * movementComponent.Speed * deltaTime;
                translation.Value.x = math.clamp(translation.Value.x, spaceBottomLeft.x, spaceTopRight.x);
            }).Run();
        }
    }
}