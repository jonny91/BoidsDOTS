// *************************************************************************************
//  *
//  * 文 件 名:   Simulation.cs
//  * 描    述:
//  *
//  * 创 建 者：  洪金敏
//  * 创建时间：  2024-04-23 21:14
// *************************************************************************************

using Unity.Entities;
using UnityEngine;

namespace Boid.DOP
{
    public class Simulation : MonoBehaviour
    {
        [Range(0, 50000)]
        public int BoidCount = 100;
        [Range(10, 100)]
        public int CreatePerFrame = 50;
        public GameObject BoidPrefab;

        private class SimulationBaker : Baker<Simulation>
        {
            public override void Bake(Simulation authoring)
            {
                var envEntity = GetEntity(TransformUsageFlags.None);
                var boidPrefabEntity = GetEntity(authoring.BoidPrefab, TransformUsageFlags.Dynamic);
           
                AddComponent(envEntity, new SimulationEnv
                {
                    BoidCount = authoring.BoidCount,
                    CreatePerFrame = authoring.CreatePerFrame,
                    BoidPrefab = boidPrefabEntity,
                });
            }
        }
    }
}