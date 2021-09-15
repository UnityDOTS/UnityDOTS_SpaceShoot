using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace DOTS
{
    public class ShooterSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (!GameManager.IsPlaying())
            {
                return;
            }

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var deltaTime = Time.DeltaTime;
            var seek = (uint)UnityEngine.Random.Range(1, 100000);
            var mouseButton = Input.GetMouseButton(0);

            // player shoot
            Entities
                .WithName("PlayerShooterSystem")
                .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
                .WithAll<PlayerComponent>()
                .ForEach((int entityInQueryIndex, Entity entity, ref ShooterComponent playerShooterComponent, in Translation translation, in LocalToWorld location) =>
                {
                    playerShooterComponent.ShootingCountdown -= deltaTime;
                    if (playerShooterComponent.ShootingCountdown <= 0f && mouseButton)
                    {
                        playerShooterComponent.ShootingCountdown = playerShooterComponent.ShootIntervalTime;
                        var instance = commandBuffer.Instantiate(entityInQueryIndex, playerShooterComponent.BulletEntity);
                        commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation() { Value = playerShooterComponent.StartPoint + translation.Value });
                        commandBuffer.SetComponent(entityInQueryIndex, instance, new MovementComponent() { Speed = 14.0f, Direction = Vector3.forward });
                        commandBuffer.SetComponent(entityInQueryIndex, instance, new BulletComponent() { IsFromPlayer = true });
                    }
                }).ScheduleParallel();

            // enemy shoot
            Entities
                .WithName("EnemyShooterSystem")
                .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
                .WithAll<EnemyComponent>()
                .ForEach((int entityInQueryIndex, Entity entity, ref ShooterComponent enemyShooterComponent, in Translation translation, in LocalToWorld location) =>
                {
                    var random = new Random(seek);
                    enemyShooterComponent.ShootingCountdown -= deltaTime;
                    if (enemyShooterComponent.ShootingCountdown <= 0f)
                    {
                        enemyShooterComponent.ShootingCountdown = random.NextFloat(0.2f, 1.2f);
                        var instance = commandBuffer.Instantiate(entityInQueryIndex, enemyShooterComponent.BulletEntity);
                        commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation() { Value = enemyShooterComponent.StartPoint + translation.Value });
                        commandBuffer.SetComponent(entityInQueryIndex, instance, new MovementComponent() { Speed = 14.0f, Direction = -Vector3.forward });
                        commandBuffer.SetComponent(entityInQueryIndex, instance, new BulletComponent() { IsFromPlayer = false });
                    }
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}