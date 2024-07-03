using AB_Utility.FromSceneToEntityConverter;
using Leopotam.EcsLite;
using SimpleInputNamespace;
using TriInspector;
using UnityEngine;

namespace Game
{
    public sealed class EcsStartup : MonoBehaviour
    {
        [SerializeField, Required] private Joystick _joystick;

        private EcsWorld _world;
        private IEcsSystems _systems;
        private IEcsSystems _lateSystems;

        private void Start()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world);

            _systems.Add(new PlayerInputSystem(_joystick))
                .Add(new PlayerMovingSystem())
                .Add(new PlayerRotationSystem())
                // register your systems here, for example:
                // .Add (new TestSystem1 ())
                // .Add (new TestSystem2 ())

                // register additional worlds here, for example:
                // .AddWorld (new EcsWorld (), "events")
#if UNITY_EDITOR
                // add debug systems for custom worlds here, for example:
                // .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ("events"))
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .ConvertScene()
                .Init();

            _lateSystems = new EcsSystems(_world);
            _lateSystems.Add(new CameraFollowSystem())
                .Init();
        }

        private void Update()
        {
            // process systems here.
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
                // list of custom worlds will be cleared
                // during IEcsSystems.Destroy(). so, you
                // need to save it here if you need.
                _systems.Destroy();
                _systems = null;
            }

            // cleanup custom worlds here.

            // cleanup default world.
            if (_world != null)
            {
                _world.Destroy();
                _world = null;
            }
        }
    }
}