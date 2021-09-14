using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace DOTS
{
    /// <summary>
    /// 固定时间生成敌机并飞行
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class EnemyCreateSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        private float _timePassed = 5.5f;
        private float _timeToCreateEnemy = 4.0f;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (!GameManager.Instance.IsPlaying) return;

            _timePassed += Time.DeltaTime;
            if (_timePassed >= _timeToCreateEnemy)
            {
                _timePassed = 0.0f;
                CreateEnemy((uint) UnityEngine.Random.Range(1, 100000));

                // Reduce the creation time during the game
                if (_timeToCreateEnemy > 2f)
                    _timeToCreateEnemy -= 0.2f;
            }
        }

        private void CreateEnemy(uint seek)
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            Entities
                .WithName("EnemyCreateSystem")
                .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
                .ForEach((int entityInQueryIndex, in EnemyManagerComponent enemyManagerComponent, in LocalToWorld location) =>
                {
                    var random = new Random(seek);
                    var instance = commandBuffer.Instantiate(entityInQueryIndex, enemyManagerComponent.EnmeyPrefab_01);

                    commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation() { Value = math.transform(location.Value, new float3(random.NextFloat(GameManager.Instance.SpaceBottomLeft.x, GameManager.Instance.SpaceTopRight.x), 0, GameManager.Instance.BoundaryTopRight.z)) });
                    commandBuffer.SetComponent(entityInQueryIndex, instance, new MovementComponent() { Speed = 5.0f, Direction = new float3(0, 0, -1) });
                    commandBuffer.SetComponent(entityInQueryIndex, instance, new EnemyComponent() { ChangeDestinationCountdown = random.NextFloat(0, 0.5f), DestructionParticle = enemyManagerComponent.DestructionParticle });
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}