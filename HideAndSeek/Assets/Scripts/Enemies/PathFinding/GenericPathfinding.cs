using System;
using System.Collections.Generic;
using UnityEngine;
public class GenericPathfinding
{
    public static List<T> BFS<T>(T start, Func<T, bool> predicate, Func<T, List<T>> GetNeighbors) where T : class?
    {
        var frontier = new Queue<T>();
        frontier.Enqueue(start);
        var cameFrom = new Dictionary<T, T>();
        cameFrom[start] = null;

        while (frontier.Count > 0)
        {
            T current = frontier.Dequeue();

            if (predicate(current))
            {
                T newCurrent = current;
                var path = new List<T>();
                while (newCurrent != null)
                {
                    path.Add(newCurrent);
                    newCurrent = cameFrom[newCurrent];
                }
                path.Reverse();
                return path;
            }

            foreach (var next in GetNeighbors(current))
            {
                if (cameFrom.ContainsKey(next)) continue;
                frontier.Enqueue(next);
                cameFrom[next] = current;
            }
        }
        return new List<T>();
    }

    public static List<T> ThetaStar<T>(T start, Func<T, bool> isSatisfies, Func<T, List<T>> getConnections,Func<T, T, float> getCost, Func<T, float> heuristic, Func<T, T, bool> inView, int watchdog = 500)
    {
        Dictionary<T, T> parents = new Dictionary<T, T>();
        PriorityQueue<T> pending = new PriorityQueue<T>();
        HashSet<T> visited = new HashSet<T>();
        Dictionary<T, float> cost = new Dictionary<T, float>();

        pending.Enqueue(start, 0);
        cost[start] = 0;
        Debug.Log("TheataAstar Generated");
        while (!pending.IsEmpty)
        {
            watchdog--;
            if (watchdog <= 0) break;
            T current = pending.Dequeue();
            if (isSatisfies(current))
            {
                //Path
                List<T> path = new List<T>();
                path.Add(current);
                while (parents.ContainsKey(path[path.Count - 1]))
                {
                    path.Add(parents[path[path.Count - 1]]);
                }
                path.Reverse();
                return path;
            }
            else
            {
                visited.Add(current);
                List<T> connections = getConnections(current);
                for (int i = 0; i < connections.Count; i++)
                {
                    T child = connections[i];
                    if (visited.Contains(child)) continue;
                    //InView
                    T realParent = current;
                    if (parents.ContainsKey(current) && inView(parents[current], child))
                    {
                        realParent = parents[current];
                    }
                    var currentCost = cost[realParent] + getCost(realParent, child);
                    if (cost.ContainsKey(child) && cost[child] <= currentCost) continue;
                    cost[child] = currentCost;
                    pending.Enqueue(child, currentCost + heuristic(child));
                    parents[child] = realParent;
                }
            }
        }
        return new List<T>();
    }
}
