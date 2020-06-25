using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chunky.Resources;
using JetBrains.Annotations;

namespace Chunky
{
    /// <summary>
    ///     Represents a resource bundle.
    /// </summary>
    public class Bundle : IEnumerable<IResource>
    {
        private readonly List<IResource> _resources;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Bundle" /> class.
        /// </summary>
        /// <param name="resources">A list of <see cref="IResource" /> objects.</param>
        public Bundle([NotNull] List<IResource> resources)
        {
            _resources = resources ?? throw new ArgumentNullException(nameof(resources));
        }

        public IEnumerator<IResource> GetEnumerator()
        {
            return _resources.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Finds the resource with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="TResource"></typeparam>
        /// <returns></returns>
        public TResource FindResourceByName<TResource>(string name) where TResource : INamedResource
        {
            return _resources.OfType<TResource>().First(r => string.Equals(r.Name, name));
        }

        public IEnumerable<TResource> FindResourcesByType<TResource>() where TResource : IResource
        {
            return _resources.OfType<TResource>();
        }
    }
}