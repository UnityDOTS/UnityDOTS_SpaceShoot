using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace DOTS
{
    /// <summary>
    /// 固定时间生成小行星并飞行
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class AsteroidCreateSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        private float _timePassed = 7.5f;
        private float _timeToCreateAsteroid = 3.0f;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (!GameManager.Instance.IsPlaying) return;

            _timePassed += Time.DeltaTime;
            if (_timePassed >= _timeToCreateAsteroid)
            {
                _timePassed = 0.0f;
                CreateAsteroid((uint)UnityEngine.Random.Range(1, 100000));

                // Reduce the creation time during the game
                if (_timeToCreateAsteroid > 1f)
                    _timeToCreateAsteroid -= 0.1f;
            }
        }

        private void CreateAsteroid(uint seek)
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            Entities
                .WithName("AsteroidCreateSystem")
                .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
                .ForEach((int entityInQueryIndex, in AsteroidManagerComponent asteroidManagerComponent, in LocalToWorld location) =>
                {
                    var random = new Random(seek);
                    Entity spawnedAsteroid = Entity.Null;
                    int value = random.NextInt(1, 4);
                    switch (value)
                    {
                        case 1:
                            spawnedAsteroid = asteroidManagerComponent.AsteroidPrefab_01;
                            break;
                        case 2:
                            spawnedAsteroid = asteroidManagerComponent.AsteroidPrefab_02;
                            break;
                        case 3:
                            spawnedAsteroid = asteroidManagerComponent.AsteroidPrefab_03;
                            break;
                    }

                    var instance = commandBuffer.Instantiate(entityInQueryIndex, spawnedAsteroid);
                    commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation { Value = math.transform(location.Value, new float3(random.NextFloat(-5f, 5f), 0, 15f)) });
                    commandBuffer.SetComponent(entityInQueryIndex, instance, new MovementComponent() { Speed = 5f, Direction = new float3(0, 0, -1) });
                    commandBuffer.SetComponent(entityInQueryIndex, instance, new RotationComponent() { AngularVelocity = new float3(0.0f, 0.0f, random.NextFloat(-1.0f, 1.0f)) });
                    commandBuffer.SetComponent(entityInQueryIndex, instance, new AsteroidComponent() { DestructionParticle = asteroidManagerComponent.DestructionParticle });
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}