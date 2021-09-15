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
        static GameManager instance;

        [SerializeField] private Camera m_CacheMainCamera;
        [SerializeField] private Text m_Score;
        [SerializeField] private Text m_Life;
        [SerializeField] private GameObject m_RestartTap;
        [SerializeField] private float m_GameRestartIntervalTime = 5f;

        private Vector3 _spaceBottomLeft;
        private Vector3 _spaceTopRight;
        private Vector3 _boundaryBottomLeft;
        private Vector3 _boundaryTopRight;
        private bool _isPlaying;

        private float _restartPauseTime;
        private BuildPhysicsWorld _buildPhysicsWorld;

        public static Vector3 SpaceBottomLeft => instance._spaceBottomLeft;
        public static Vector3 SpaceTopRight => instance._spaceTopRight;
        public static Vector3 BoundaryBottomLeft => instance._boundaryBottomLeft;
        public static Vector3 BoundaryTopRight => instance._boundaryTopRight;
        public static bool IsPlaying() => instance._isPlaying;

        public void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }

            m_CacheMainCamera = Camera.main;

            instance = this;
            _isPlaying = true;
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
            var bottomLeft = m_CacheMainCamera.ViewportToWorldPoint(new Vector3(0, 0, m_CacheMainCamera.nearClipPlane)); // -5.6, -5   左下角
            var topRight = m_CacheMainCamera.ViewportToWorldPoint(new Vector3(1, 1, m_CacheMainCamera.nearClipPlane)); // 5.6, 15   右上角
            var half = Vector3.one / 2.0f;

            _spaceBottomLeft = bottomLeft + half;
            _spaceTopRight = topRight - half;
            _boundaryBottomLeft = bottomLeft - Vector3.one;
            _boundaryTopRight = topRight + Vector3.one;
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
                    if (_isPlaying)
                    {
                        _isPlaying = false;
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
            _isPlaying = true;
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
            m_Life.text = playerComponent.Life.ToString();
            m_Score.text = playerComponent.Score.ToString();
        }
    }
}