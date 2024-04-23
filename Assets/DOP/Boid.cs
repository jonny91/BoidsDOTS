/*************************************************************************************
 *
 * 文 件 名:   Boid.cs
 * 描    述: 
 * 
 * 创 建 者：  洪金敏 
 * 创建时间：  2024-04-23 21:51:32
*************************************************************************************/
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Boid.DOP
{
    public class Boid : MonoBehaviour
    {
        private class BoidBaker : Baker<Boid>
        {
            public override void Bake(Boid authoring)
            {
                var prefabEntity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(prefabEntity, new BoidCell());
                AddComponent(prefabEntity, new Acceleration()
                {
                    Value = float3.zero,
                });
                AddComponent(prefabEntity, new Velocity()
                {
                    Value = float3.zero,
                });
            }
        }
    }
}