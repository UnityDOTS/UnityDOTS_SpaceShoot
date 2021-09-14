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
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public Camera CacheMainCamera;
        public Text Scroe;
        public Text Life;
        [SerializeField] private GameObject m_RestartTap;
        [SerializeField] private float m_GameRestartIntervalTime = 5f;

        public Vector3 SpaceBottomLeft { get; private set; }
        public Vector3 SpaceTopRight { get; private set; }
        public Vector3 BoundaryBottomLeft { get; private set; }
        public Vector3 BoundaryTopRight { get; private set; }
        public bool IsPlaying { get; private set; }
        private float _restartPauseTime;
        private BuildPhysicsWorld _buildPhysicsWorld;

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            CacheMainCamera = Camera.main;

            Instance = this;
            IsPlaying = true;
            _restartPauseTime = m_GameRestartIntervalTime;
            m_RestartTap.gameObject.SetActive(false);
            _buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
            CalculateScreenBounds();
        }

        /// <summary>
        /// 计算屏幕区域边界
        /// </summary>
        private void CalculateScreenBounds()
        {
            var bottomLeft = CacheMainCamera.ViewportToWorldPoint(new Vector3(0, 0, CacheMainCamera.nearClipPlane)); // -5.6, -5   左下角
            var topRight = CacheMainCamera.ViewportToWorldPoint(new Vector3(1, 1, CacheMainCamera.nearClipPlane)); // 5.6, 15   右上角
            var half = Vector3.one / 2.0f;

            SpaceBottomLeft = bottomLeft + half;
            SpaceTopRight = topRight - half;
            BoundaryBottomLeft = bottomLeft - Vector3.one;
            BoundaryTopRight = topRight + Vector3.one;
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