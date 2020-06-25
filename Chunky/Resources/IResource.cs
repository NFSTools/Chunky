namespace Chunky.Resources
{
    /// <summary>
    ///     Represents a resource. A resource is a specification of data within a chunk.
    /// </summary>
    public interface IResource
    {
        /// <summary>
        ///     Returns a user-friendly type name for the resource.
        /// </summary>
        /// <returns>A user-friendly type name for the resource.</returns>
        string GetResourceTypeName();
    }
}