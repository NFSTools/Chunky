namespace Chunky.Resources
{
    /// <summary>
    ///     Exposes an API for resource names.
    /// </summary>
    public interface INamedResource : IResource
    {
        /// <summary>
        ///     Gets or sets the name of the resource.
        /// </summary>
        string Name { get; set; }
    }
}