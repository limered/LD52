using System;
using System.Collections.Generic;

namespace SystemBase.Core
{
    public static class ComponentToSystemsMapper
    {
        public static Dictionary<Type, List<IGameSystem>> CreateMap(IEnumerable<IGameSystem> gameSystems)
        {
            var componentToSystemsMap = new Dictionary<Type, List<IGameSystem>>();
            foreach (var system in gameSystems)
            {
                foreach (var componentType in system.ComponentsToRegister)
                {
                    if (!componentToSystemsMap.ContainsKey(componentType))
                    {
                        componentToSystemsMap.Add(componentType, new List<IGameSystem>());
                    }
                    componentToSystemsMap[componentType].Add(system);
                }
            }

            return componentToSystemsMap;
        }
    }
}