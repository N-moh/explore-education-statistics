﻿namespace GovUk.Education.ExploreEducationStatistics.Common
{
    public interface IBlobContainer
    {
        public string Name { get; }
        public string EmulatedName { get; }
    }

    public static class BlobContainers
    {
        public static readonly IBlobContainer PrivateReleaseFiles = new BlobContainer("releases");
        public static readonly IBlobContainer PublicReleaseFiles = new BlobContainer("downloads");
        public static readonly IBlobContainer PublicContent = new BlobContainer("cache");
        public static readonly IBlobContainer Permalinks = new BlobContainer("permalinks");
        public static readonly IBlobContainer PermalinkMigrations = new BlobContainer("permalink-migrations");
        public static readonly IBlobContainer PublisherLeases = new BlobContainer("leases");
        public static readonly IBlobContainer PrivateMethodologyFiles = new PrivateBlobContainer("methodologies");
        public static readonly IBlobContainer PublicMethodologyFiles = new PublicBlobContainer("methodologies");
    }

    /// <summary>
    /// Blob container with an immutable name that doesn't change when used with emulator storage
    /// </summary>
    public class BlobContainer : IBlobContainer
    {
        public string Name { get; }

        public BlobContainer(string name)
        {
            Name = name;
        }

        public string EmulatedName => Name;

        public override string ToString() => Name;
    }

    /// <summary>
    /// Blob container with a name prefixed by 'public-' when used with emulator storage
    /// </summary>
    public class PublicBlobContainer : IBlobContainer
    {
        public string Name { get; }

        public PublicBlobContainer(string name)
        {
            Name = name;
        }

        public string EmulatedName => $"public-{Name}";

        public override string ToString() => Name;
    }

    /// <summary>
    /// Blob container with a name prefixed by 'private-' when used with emulator storage
    /// </summary>
    public class PrivateBlobContainer : IBlobContainer
    {
        public string Name { get; }

        public PrivateBlobContainer(string name)
        {
            Name = name;
        }

        public string EmulatedName => $"private-{Name}";

        public override string ToString() => Name;
    }
}