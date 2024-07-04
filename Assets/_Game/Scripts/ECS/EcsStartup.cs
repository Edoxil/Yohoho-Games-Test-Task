using AB_Utility.FromSceneToEntityConverter;
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
            _world = new EcsWorld();
            _systems = new EcsSystems(_world);

            _systems.Add(new PlayerInputSystem())
                .Add(new StorageInitializationSystem())
                .Add(new ItemGenerationSystem())
                .Add(new PlayerMovingSystem())
                .Add(new PlayerRotationSystem())
                .Add(new PlayerAnimationSystem())
                .Add(new ItemTransactionSolvingSystem())
#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .ConvertScene()
                .Inject(_sceneData)
                .Init();

            _lateSystems = new EcsSystems(_world);
            _lateSystems.Add(new CameraFollowSystem())
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