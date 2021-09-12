using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;

namespace DOTS
{
    /// <summary>
    /// 触发事件System（在EndFramePhysicsSystem之后执行）
    /// </summary>
    [UpdateAfter(typeof(ShooterSystem))]
    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public class TriggerEventSystem : JobComponentSystem
    {
        private BuildPhysicsWorld buildPhysicsWorlds;
        private StepPhysicsWorld stepPhysicsWorlds;

        protected override void OnCreate()
        {
            base.OnCreate();
            buildPhysicsWorlds = World.GetOrCreateSystem<BuildPhysicsWorld>();
            stepPhysicsWorlds = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        [BurstCompile]
        struct ObjectsTriggerSystemJob : ITriggerEventsJob
        {
            public ComponentDataFromEntity<PlayerComponent> players;
            public ComponentDataFromEntity<EnemyComponent> enemys;
            public ComponentDataFromEntity<AsteroidComponent> asteroids;
            public ComponentDataFromEntity<BulletComponent> bullets;
            public ComponentDataFromEntity<Translation> translations;
            public PlayerComponent playerComponent;

            [ReadOnly] public Entity playerEntity;

            public EntityCommandBuffer entityCommandBuffer;

            public void CreateAsteroidParticle(Entity asteroidEntity)
            {
                Entity particleEntity = entityCommandBuffer.Instantiate(asteroids[asteroidEntity].DestructionParticle);

                Translation particleTranslation = new Translation { Value = translations[asteroidEntity].Value };

                entityCommandBuffer.SetComponent(particleEntity, particleTranslation);
            }

            public void CreatePlayerParticle(Entity playerEntity)
            {
                Entity particleEntity = entityCommandBuffer.Instantiate(playerComponent.DestructionParticle);

                Translation particleTranslation = new Translation { Value = translations[playerEntity].Value };

                entityCommandBuffer.SetComponent(particleEntity, particleTranslation);
            }

            public void CreateEnemyParticle(Entity enemyEntity)
            {
                Entity particleEntity = entityCommandBuffer.Instantiate(enemys[enemyEntity].DestructionParticle);

                Translation particleTranslation = new Translation { Value = translations[enemyEntity].Value };

                entityCommandBuffer.SetComponent(particleEntity, particleTranslation);
            }

            void CheckBulletAsteroidCollision(Entity obj1, Entity obj2)
            {
                if ((asteroids.HasComponent(obj1) && bullets.HasComponent(obj2) && bullets[obj2].IsFromPlayer) ||
                    (asteroids.HasComponent(obj2) && bullets.HasComponent(obj1) && bullets[obj1].IsFromPlayer))
                {
                    if (asteroids.HasComponent(obj1))
                        CreateAsteroidParticle(obj1);
                    else
                        CreateAsteroidParticle(obj2);

                    playerComponent.Score += 10;

                    entityCommandBuffer.SetComponent(playerEntity, playerComponent);

                    entityCommandBuffer.DestroyEntity(obj1);
                    entityCommandBuffer.DestroyEntity(obj2);
                }
            }

            void CheckAsteroidPlayerCollision(Entity obj1, Entity obj2)
            {
                if (playerComponent.Life <= 0)
                {
                    return;
                }

                if ((asteroids.HasComponent(obj1) && players.HasComponent(obj2)) ||
                    (asteroids.HasComponent(obj2) && players.HasComponent(obj1)))
                {
                    if (players.HasComponent(obj1))
                    {
                        CreateAsteroidParticle(obj2);
                        entityCommandBuffer.DestroyEntity(obj2);
                    }
                    else
                    {
                        CreateAsteroidParticle(obj1);
                        entityCommandBuffer.DestroyEntity(obj1);
                    }

                    playerComponent.Life--;

                    if (playerComponent.Life == 0)
                    {
                        CreatePlayerParticle(playerEntity);
                    }

                    entityCommandBuffer.SetComponent(playerEntity, playerComponent);
                }
            }

            void CheckEnemyPlayerCollision(Entity obj1, Entity obj2)
            {
                if (playerComponent.Life <= 0)
                {
                    return;
                }

                if ((enemys.HasComponent(obj1) && players.HasComponent(obj2)) ||
                    (enemys.HasComponent(obj2) && players.HasComponent(obj1)))
                {
                    if (players.HasComponent(obj1))
                    {
                        CreateEnemyParticle(obj2);
                        entityCommandBuffer.DestroyEntity(obj2);
                    }
                    else
                    {
                        CreateEnemyParticle(obj1);
                        entityCommandBuffer.DestroyEntity(obj1);
                    }

                    playerComponent.Life--;

                    if (playerComponent.Life == 0)
                    {
                        CreatePlayerParticle(playerEntity);
                    }

                    entityCommandBuffer.SetComponent(playerEntity, playerComponent);
                }
            }

            void CheckBulletEnemyCollision(Entity obj1, Entity obj2)
            {
                if ((enemys.HasComponent(obj1) && bullets.HasComponent(obj2) && bullets[obj2].IsFromPlayer) ||
                    (enemys.HasComponent(obj2) && bullets.HasComponent(obj1) && bullets[obj1].IsFromPlayer))
                {
                    if (enemys.HasComponent(obj1))
                        CreateEnemyParticle(obj1);
                    else
                        CreateEnemyParticle(obj2);

                    playerComponent.Score += 10;

                    entityCommandBuffer.SetComponent(playerEntity, playerComponent);

                    entityCommandBuffer.DestroyEntity(obj1);
                    entityCommandBuffer.DestroyEntity(obj2);
                }
            }

            void CheckBulletPlayerCollision(Entity obj1, Entity obj2)
            {
                if (playerComponent.Life <= 0)
                {
                    return;
                }

                if ((players.HasComponent(obj1) && bullets.HasComponent(obj2) && !bullets[obj2].IsFromPlayer) ||
                    (players.HasComponent(obj2) && bullets.HasComponent(obj1) && !bullets[obj1].IsFromPlayer))
                {
                    if (players.HasComponent(obj1))
                    {
                        CreatePlayerParticle(obj1);
                        entityCommandBuffer.DestroyEntity(obj2);
                    }
                    else
                    {
                        CreatePlayerParticle(obj2);
                        entityCommandBuffer.DestroyEntity(obj1);
                    }

                    playerComponent.Life--;

                    if (playerComponent.Life == 0)
                    {
                        CreatePlayerParticle(playerEntity);
                    }

                    entityCommandBuffer.SetComponent(playerEntity, playerComponent);
                }
            }

            public void Execute(TriggerEvent triggerEvent)
            {
                Entity obj1 = triggerEvent.EntityA;
                Entity obj2 = triggerEvent.EntityB;

                CheckBulletAsteroidCollision(obj1, obj2);
                CheckAsteroidPlayerCollision(obj1, obj2);
                CheckEnemyPlayerCollision(obj1, obj2);
                CheckBulletEnemyCollision(obj1, obj2);
                CheckBulletPlayerCollision(obj1, obj2);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            ObjectsTriggerSystemJob job = new ObjectsTriggerSystemJob();
            job.players = GetComponentDataFromEntity<PlayerComponent>();
            job.enemys = GetComponentDataFromEntity<EnemyComponent>();
            job.asteroids = GetComponentDataFromEntity<AsteroidComponent>();
            job.bullets = GetComponentDataFromEntity<BulletComponent>();
            job.translations = GetComponentDataFromEntity<Translation>();

            BeginInitializationEntityCommandBufferSystem ecbSystem;

            ecbSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
            job.playerEntity = ecbSystem.GetSingletonEntity<PlayerComponent>();
            job.playerComponent = job.players[job.playerEntity];
            job.entityCommandBuffer = ecbSystem.CreateCommandBuffer();

            JobHandle jobHandle = job.Schedule(stepPhysicsWorlds.Simulation, ref buildPhysicsWorlds.PhysicsWorld, inputDeps);

            ecbSystem.AddJobHandleForProducer(jobHandle);

            return jobHandle;
        }
    }
}