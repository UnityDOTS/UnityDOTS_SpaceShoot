using System.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;
using Entity = Unity.Entities.Entity;

namespace DOTS
{
    public class GameManager : Singleton<GameManager>
    {
        public Text Scroe;
        public Text Life;
        [SerializeField] private GameObject m_RestartTap;

        [Tooltip("自动重新开始的间隔时间")] [SerializeField]
        private float m_GameRestartIntervalTime = 5f;

        private float _restartPauseTime;

        public bool IsPlaying { get; private set; }

        private BuildPhysicsWorld _buildPhysicsWorld;


        public override void Awake()
        {
            base.Awake();
            _restartPauseTime = m_GameRestartIntervalTime;

            IsPlaying = true;
            m_RestartTap.gameObject.SetActive(false);
            _buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        }

        private void Update()
        {
            var player = _buildPhysicsWorld.GetSingletonEntity<PlayerComponent>();
            var dataFromEntity = _buildPhysicsWorld.GetComponentDataFromEntity<PlayerComponent>();

            if (dataFromEntity.HasComponent(player))
            {
                var playerComponent = dataFromEntity[player];
                UpdateInterface(player, playerComponent);

                if (playerComponent.Life == 0)
                {
                    _restartPauseTime -= Time.deltaTime;
                    if (IsPlaying)
                    {
                        IsPlaying = false;
                        StartCoroutine(nameof(RestartTapCoroutine));
                    }

                    if (_restartPauseTime <= 0)
                    {
                        RestartLevel(player);
                        _restartPauseTime = m_GameRestartIntervalTime;
                    }
                }
            }
        }

        IEnumerator RestartTapCoroutine()
        {
            yield return new WaitForSeconds(0.8f);
            m_RestartTap.gameObject.SetActive(true);
        }

        public void ClickRestartGame()
        {
            var world = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
            var player = world.GetSingletonEntity<PlayerComponent>();
            RestartLevel(player);
        }

        private void RestartLevel(Entity player)
        {
            IsPlaying = true;
            m_RestartTap.gameObject.SetActive(false);

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var query = entityManager.CreateEntityQuery(typeof(AsteroidComponent));
            var entities = query.ToEntityArray(Allocator.TempJob);
            entityManager.DestroyEntity(entities);
            entities.Dispose();

            query = entityManager.CreateEntityQuery(typeof(BulletComponent));
            entities = query.ToEntityArray(Allocator.TempJob);
            entityManager.DestroyEntity(entities);
            entities.Dispose();

            query = entityManager.CreateEntityQuery(typeof(DelayDestroyComponent));
            entities = query.ToEntityArray(Allocator.TempJob);
            entityManager.DestroyEntity(entities);
            entities.Dispose();

            // restart player
            var translation = entityManager.GetComponentData<Translation>(player);
            var playerComponent = entityManager.GetComponentData<PlayerComponent>(player);
            var movementComponent = entityManager.GetComponentData<MovementComponent>(player);

            translation.Value = float3.zero;
            movementComponent.Direction = float3.zero;
            playerComponent.Life = 3;
            playerComponent.Score = 0;
            playerComponent.Destination = float3.zero;

            entityManager.SetComponentData(player, translation);
            entityManager.SetComponentData(player, playerComponent);
            entityManager.SetComponentData(player, movementComponent);
        }

        private void UpdateInterface(Entity player, PlayerComponent playerComponent)
        {
            Life.text = playerComponent.Life.ToString();
            Scroe.text = playerComponent.Score.ToString();
        }
    }
}