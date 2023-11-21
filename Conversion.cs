using g3;
using Elements.Geometry;
namespace Elements.G3;

/// <summary>
/// Provides static methods for converting between Elements.Geometry and g3 types.
/// </summary>
public static class Conversion
{
    /// <summary>
    /// Converts a single Mesh to a DMesh3 type.
    /// </summary>
    /// <param name="mesh">The Mesh to be converted.</param>
    /// <param name="transform">An optional transformation to be applied to the Mesh.</param>
    /// <returns>A DMesh3 instance corresponding to the input Mesh.</returns>
    public static DMesh3 ToDMesh3(this Mesh mesh, Transform? transform = null)
    {
        return new Mesh[] { mesh }.ToDMesh3(transform);
    }

    /// <summary>
    /// Converts a collection of Meshes to a single DMesh3 type.
    /// </summary>
    /// <param name="meshes">The collection of Meshes to be converted.</param>
    /// <param name="transform">An optional transformation to be applied to each Mesh in the collection.</param>
    /// <returns>A DMesh3 instance representing the combined input Meshes.</returns>
    public static DMesh3 ToDMesh3(this IEnumerable<Mesh> meshes, Transform? transform = null)
    {
        var dMesh = new DMesh3();
        var indexOffset = 0;

        foreach (var m in meshes)
        {
            foreach (var v in m.Vertices)
            {
                var pt = transform == null ? v.Position : transform.OfPoint(v.Position);
                dMesh.AppendVertex(pt.ToG3Vector3d());
            }
            foreach (var tri in m.Triangles)
            {
                var triVerts = tri.Vertices;
                dMesh.AppendTriangle(triVerts[0].Index + indexOffset, triVerts[1].Index + indexOffset, triVerts[2].Index + indexOffset);
            }
            indexOffset += m.Vertices.Count;
        }
        return dMesh;
    }

    /// <summary>
    /// Converts a single Mesh to a DMeshAABBTree3 type.
    /// </summary>
    /// <param name="mesh">The Mesh to be converted.</param>
    /// <returns>A DMeshAABBTree3 instance corresponding to the input Mesh.</returns>
    public static DMeshAABBTree3 ToAABBTree(this Mesh mesh)
    {
        return new Mesh[] { mesh }.ToAABBTree();
    }

    /// <summary>
    /// Converts a collection of Meshes to a single DMeshAABBTree3 type.
    /// </summary>
    /// <param name="meshes">The collection of Meshes to be converted.</param>
    /// <returns>A DMeshAABBTree3 instance representing the combined input Meshes.</returns>

    public static DMeshAABBTree3 ToAABBTree(this IEnumerable<Mesh> meshes)
    {
        return new DMeshAABBTree3(meshes.ToDMesh3(), true);
    }

    /// <summary>
    /// Converts an Elements.Geometry Vector3 to a g3 Vector3d.
    /// </summary>
    /// <param name="v">The Vector3 to be converted.</param>
    /// <returns>A Vector3d instance corresponding to the input Vector3.</returns>
    public static Vector3d ToG3Vector3d(this Vector3 v)
    {
        return new Vector3d(v.X, v.Y, v.Z);
    }

    /// <summary>
    /// Converts a g3 Vector3d to an Elements.Geometry Vector3.
    /// </summary>
    /// <param name="v">The Vector3d to be converted.</param>
    /// <returns>A Vector3 instance corresponding to the input Vector3d.</returns>
    public static Vector3 ToElementsVector3(this Vector3d v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

    /// <summary>
    /// Converts an Elements.Geometry Ray to a g3 Ray3d.
    /// </summary>
    /// <param name="ray">The Ray to be converted.</param>
    /// <returns>A Ray3d instance corresponding to the input Ray.</returns>
    public static Ray3d ToG3Ray3d(this Ray ray)
    {
        return new Ray3d(ray.Origin.ToG3Vector3d(), ray.Direction.ToG3Vector3d());
    }
}
