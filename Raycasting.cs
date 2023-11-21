using System.Reflection.Metadata;
using Elements.Geometry;
using g3;

namespace Elements.G3;


/// <summary>
/// Represents a raycaster for performing raycasting operations on 3D meshes.
/// </summary>
public class Raycaster
{
    /// <summary>
    /// Initializes a new instance of the Raycaster class with a single mesh as the raycast target.
    /// </summary>
    /// <param name="raycastTarget">The mesh to be used as the raycasting target.</param>
    /// <param name="transform">An optional transformation to be applied to the mesh.</param>
    public Raycaster(Mesh raycastTarget, Transform? transform = null)
    {
        this.Meshes = new List<Mesh> { raycastTarget };
        this.G3Mesh = raycastTarget.ToDMesh3(transform);
        this.AABBTree = new DMeshAABBTree3(G3Mesh, true);
    }

    /// <summary>
    /// Initializes a new instance of the Raycaster class with multiple meshes as the raycasting targets.
    /// </summary>
    /// <param name="raycastTargets">A collection of meshes to be used as the raycasting targets.</param>
    /// <param name="transform">An optional transformation to be applied to the meshes.</param>
    public Raycaster(IEnumerable<Mesh> raycastTargets, Transform? transform = null)
    {
        this.Meshes = raycastTargets;
        this.G3Mesh = raycastTargets.ToDMesh3(transform);
        this.AABBTree = new DMeshAABBTree3(G3Mesh, true);
    }

    private IEnumerable<Mesh> Meshes { get; }
    private DMesh3 G3Mesh { get; }
    private DMeshAABBTree3 AABBTree { get; }

    /// <summary>
    /// Casts a ray against the mesh and returns information about the intersection, if any.
    /// </summary>
    /// <param name="ray">The ray to be cast.</param>
    /// <returns>A MeshRayIntersection struct containing details about the intersection.</returns>
    public MeshRayIntersection Cast(Ray ray)
    {
        var origin = ray.Origin.ToG3Vector3d();
        var dRay = ray.ToG3Ray3d();
        int hit_tid = this.AABBTree.FindNearestHitTriangle(dRay, 10000);
        if (hit_tid != DMesh3.InvalidID)
        {
            IntrRay3Triangle3 intr = MeshQueries.TriangleIntersection(this.G3Mesh, hit_tid, dRay);
            var hitPoint = dRay.PointAt(intr.RayParameter);
            var hitDist = origin.Distance(hitPoint);
            Console.WriteLine($"Got a hit! {hitPoint}, {hitDist}");
            return new MeshRayIntersection(true, hitDist, hitPoint.ToElementsVector3());
        }
        return new MeshRayIntersection(false, 0, null);
    }
}

/// <summary>
/// Represents the result of a raycasting operation, indicating if an intersection occurred,
/// the distance to the intersection point, and the intersection point itself.
/// </summary>
public readonly struct MeshRayIntersection
{
    public MeshRayIntersection(bool hit, double distance, Vector3? hitPoint)
    {
        this.Hit = hit;
        this.Distance = distance;
        this.HitPoint = hitPoint;
    }
    public readonly bool Hit;
    public readonly double Distance;
    public readonly Vector3? HitPoint;
}