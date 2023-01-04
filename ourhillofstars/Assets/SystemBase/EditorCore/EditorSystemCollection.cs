using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SystemBase.EditorCore
{
    public class EditorSystemCollection
    {
        private readonly Dictionary<Type, IEditorSystem> _systems = new Dictionary<Type, IEditorSystem>();

        public void Initialize()
        {
            foreach (var systemType in CollectAllSystems())
            {
                RegisterSystem(Activator.CreateInstance(systemType) as IEditorSystem);
            }
        }

        private void RegisterSystem(IEditorSystem systemInstance)
        {   
            Debug.Log($"System created {systemInstance.GetType()}");
            AddSystem(systemInstance);
        }

        private static IEnumerable<Type> CollectAllSystems()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(ass => ass.GetTypes(), (ass, type) => new { ass, type })
                .Where(assemblyType => Attribute.IsDefined(assemblyType.type, typeof(EditorSystemAttribute)))
                .Select(assemblyType => assemblyType.type);
        }

        private void AddSystem(IEditorSystem system)
        {
            _systems[system.GetType()] = system;
        }

        public TSystem GetSystem<TSystem>() where TSystem : IEditorSystem, new()
        {
            if (_systems.TryGetValue(typeof(TSystem), out var system))
            {
                return (TSystem) system;
            }

            system = new TSystem();
            AddSystem(system);
            return (TSystem)system;
        }
    }
}