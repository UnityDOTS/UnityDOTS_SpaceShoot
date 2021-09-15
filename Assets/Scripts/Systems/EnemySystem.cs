using Unity.Burst;
using Unity.Entities;
using Random = Unity.Mathematics.Random;

namespace DOTS
{
    /// <summary>
    /// Enemy AI
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class EnemySystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var seek = (uint) UnityEngine.Random.Range(1, 100000);
            var random = new Random(seek);
            var deltaTime = Time.DeltaTime;

            Entities
                .WithName("EnemySystem")
                .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
                .ForEach((Entity entity, ref EnemyComponent enemyComponent, ref MovementComponent movementComponent) =>
                {
                    enemyComponent.ChangeDestinationCountdown -= deltaTime;
                    if (enemyComponent.ChangeDestinationCountdown <= 0f)
                    {
                        movementComponent.Direction.x = random.NextFloat(-1f, 1f);
                        enemyComponent.ChangeDestinationCountdown = random.NextFloat(1.5f, 2f);
                    }
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}