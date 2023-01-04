using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SystemBase.Core
{
    public static class GameSystemSorter
    {
        private static GameSystemAttribute GetAttribute(MemberInfo t)
        {
            return (GameSystemAttribute) Attribute.GetCustomAttribute(t, typeof(GameSystemAttribute));
        }

        public static List<IGameSystem> Sort(List<IGameSystem> systemInstances)
        {
            var inDegrees = systemInstances.ToDictionary(system => system.GetType(), _ => 0);
            
            foreach (var dependency in systemInstances
                .SelectMany(system => GetAttribute(system.GetType()).Dependencies))
            {
                inDegrees[dependency]++;
            }
            
            var result = new List<IGameSystem>();
            var q = new Queue<IGameSystem>(systemInstances
                .Where(system => inDegrees[system.GetType()] == 0));
            
            while (q.Any())
            {
                var system = q.Dequeue();
                result.Add(system);
                foreach (var dependency in GetAttribute(system.GetType()).Dependencies)
                {
                    inDegrees[dependency]--;
                    if (inDegrees[dependency] == 0)
                    {
                        q.Enqueue(
                            systemInstances
                                .First(instance => dependency == instance.GetType()));
                    } 
                }
            }

            result.Reverse();
            if (systemInstances.Count == result.Count) return result;
            var circ = systemInstances.First(s => !result.Contains(s));
            throw new ArgumentException("Circular dependency in GameSystem registration! System: " + circ.GetType());
        }
    }
}