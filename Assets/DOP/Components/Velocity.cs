// *************************************************************************************
//  *
//  * 文 件 名:   Velocity.cs
//  * 描    述:
//  *
//  * 创 建 者：  洪金敏
//  * 创建时间：  2024-04-23 19:04
// *************************************************************************************

using Unity.Entities;
using Unity.Mathematics;

namespace Boid.DOP
{
    public struct Velocity : IComponentData
    {
        public float3 Value;
    }
}