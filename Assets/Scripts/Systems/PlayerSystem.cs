using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class PlayerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var playerEntity = GetSingletonEntity<PlayerComponent>();
            var playerComponent = GetComponent<PlayerComponent>(playerEntity);
            var playerTranslation = GetComponent<Translation>(playerEntity);

            if (Input.GetMouseButton(0))
            {
                var mousePosition = (float3) Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.x = math.clamp(mousePosition.x, GameManager.SpaceBottomLeft.x, GameManager.SpaceTopRight.x);
                mousePosition.y = 0f;
                mousePosition.z = math.clamp(mousePosition.z, GameManager.SpaceBottomLeft.z, GameManager.SpaceTopRight.z);
                playerComponent.Destination = mousePosition;
                EntityManager.SetComponentData(playerEntity, playerComponent);
            }

            Vector3 dir = playerComponent.Destination - playerTranslation.Value;
            var direction = dir.sqrMagnitude <= 0.1f ? float3.zero : (float3) dir.normalized;
            Entities
                .WithName("PlayerSystem")
                .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
                .ForEach((ref MovementComponent movementComponent, in PlayerComponent player) => { movementComponent.Direction = direction; }).Schedule();
        }
    }
}