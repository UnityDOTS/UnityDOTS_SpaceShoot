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

            var direction = float3.zero;
            if (playerComponent.IsTouch)
            {
                var playerTranslation = GetComponent<Translation>(playerEntity);
                if (Input.GetMouseButton(0))
                {
                    var mousePosition = (float3)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mousePosition.x = math.clamp(mousePosition.x, -5f, 5f);
                    mousePosition.y = 0f;
                    mousePosition.z = math.clamp(mousePosition.z, -4f, 14f);
                    playerComponent.Destination = mousePosition;
                    EntityManager.SetComponentData(playerEntity, playerComponent);
                }

                Vector3 dir = playerComponent.Destination - playerTranslation.Value;
                direction = dir.sqrMagnitude <= 0.1f ? float3.zero : (float3)dir.normalized;
            }
            else
            {
                var varX = Input.GetAxis("Horizontal");
                var varZ = Input.GetAxis("Vertical");
                direction = new float3(varX, 0f, varZ);
            }

            Entities
                .WithName("PlayerSystem")
                .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
                .ForEach((ref MovementComponent movementComponent, in PlayerComponent player) => { movementComponent.Direction = direction; }).Schedule();
        }
    }
}