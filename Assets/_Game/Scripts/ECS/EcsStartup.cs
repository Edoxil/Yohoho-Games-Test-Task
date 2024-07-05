using AB_Utility.FromSceneToEntityConverter;
using LeoEcsPhysics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using TriInspector;
using UnityEngine;

namespace Game
{
    public sealed class EcsStartup : MonoBehaviour
    {
        [SerializeField, Required] private SceneData _sceneData;
        private EcsWorld _world;
        private IEcsSystems _systems;
        private IEcsSystems _lateSystems;

        private void Start()
        {
            Application.targetFrameRate = 60;

            _world = new EcsWorld();

            EcsPhysicsEvents.ecsWorld = _world;

            _systems = new EcsSystems(_world);
            _systems.Add(new PlayerInputSystem())
                .Add(new StorageInitializationSystem())
                .Add(new ItemGenerationSystem())
                .Add(new PlayerStorageSystem())
                .Add(new PlayerMovingSystem())
                .Add(new PlayerRotationSystem())
                .Add(new PlayerAnimationSystem())
                .Add(new ItemTransactionHandleSystem())
#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .DelHerePhysics()
                .ConvertScene()
                .Inject(_sceneData)
                .Init();

            _lateSystems = new EcsSystems(_world);
            _lateSystems.Add(new CameraFollowSystem())
                .Add(new FreezRotationSystem())
                .Add(new CapacityDisplaySystem())
                .Init();
        }

        private void Update()
        {
            _systems?.Run();
        }

        private void LateUpdate()
        {
            _lateSystems?.Run();
        }

        private void OnDestroy()
        {
            if (_systems != null)
            {
                _systems.Destroy();
                _systems = null;
            }

            if (_lateSystems != null)
            {
                _lateSystems.Destroy();
                _lateSystems = null;
            }

            if (_world != null)
            {
                _world.Destroy();
                _world = null;
            }
        }
    }
}