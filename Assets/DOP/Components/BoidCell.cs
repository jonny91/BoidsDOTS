// *************************************************************************************
//  *
//  * 文 件 名:   BoidCell.cs
//  * 描    述:
//  *
//  * 创 建 者：  洪金敏
//  * 创建时间：  2024-04-23 18:04
// *************************************************************************************

using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Boid.DOP
{
    [MaterialProperty("_BaseColor")]
    public struct BoidCell : IComponentData
    {
        public float4 Color;
    }
}