using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using Moq;
using NUnit.Framework;
using NuGet;
using deployd.Features.AppLocating;
using log4net;

namespace deployd.tests.Features.AppLocating
{
    [TestFixture]
    public class GetLatestNuGetPackageByNameQueryTests
    {
        private GetLatestNuGetPackageByNameQuery _query;
        private Mock<IPackageRepositoryFactory> _packageRepoFactory;
        private Mock<ILog> _log;
        private Mock<IPackageRepository> _repo;
        private IQueryable<IPackage> _packages;
        private List<IPackage> _packageList;

        [SetUp]
        public void SetUp()
        {
            _packageRepoFactory = new Mock<IPackageRepositoryFactory>();
            _repo = new Mock<IPackageRepository>();
            _log = new Mock<ILog>();
            _packageList = new List<IPackage>();
            _packages = new EnumerableQuery<IPackage>(_packageList);
            _query = new GetLatestNuGetPackageByNameQuery(_packageRepoFactory.Object, _log.Object);
            _repo.Setup(x => x.GetPackages()).Returns(_packages);
            _packageRepoFactory.Setup(x => x.CreateRepository(It.IsAny<string>())).Returns(_repo.Object);
        }

        [Test]
        public void GetLatestVersionOf_CreatesPackageRepositoryForLocation()
        {
            _query.GetLatestVersionOf("app", "c:\\location");

            _packageRepoFactory.Verify(x => x.CreateRepository("c:\\location"));
        }

        [Test]
        public void GetLatestVersionOf_GetsPackagesFromCreatedRepo()
        {
            _query.GetLatestVersionOf("app", "c:\\location");

            _repo.Verify(x=>x.GetPackages());
        }

        [Test]
        public void GetLatestVersionOf_FiltersReturnsNewestPackageMatchingId()
        {
            var packageOne = new FakePackage{Id = "app", Version = SemanticVersion.Parse("1.0.0.0")};
            var packageThree = new FakePackage{Id = "app", Version = SemanticVersion.Parse("1.5.0.0"), IsLatestVersion = true};
            var packageTwo = new FakePackage{Id = "app", Version = SemanticVersion.Parse("1.1.0.0")};
            _packageList.Add(packageOne);
            _packageList.Add(packageThree);
            _packageList.Add(packageTwo);

            var package =_query.GetLatestVersionOf("app", "c:\\location");

            Assert.That(package, Is.EqualTo(packageThree));
        }


        public class FakePackage : IPackage
        {
            public string Id { get; set; }
            public SemanticVersion Version { get; set; }
            public string Title { get; private set; }
            public IEnumerable<string> Authors { get; private set; }
            public IEnumerable<string> Owners { get; private set; }
            public Uri IconUrl { get; private set; }
            public Uri LicenseUrl { get; private set; }
            public Uri ProjectUrl { get; private set; }
            public bool RequireLicenseAcceptance { get; private set; }
            public string Description { get; private set; }
            public string Summary { get; private set; }
            public string ReleaseNotes { get; private set; }
            public string Language { get; private set; }
            public string Tags { get; private set; }
            public string Copyright { get; private set; }
            public IEnumerable<FrameworkAssemblyReference> FrameworkAssemblies { get; private set; }
            public IEnumerable<PackageDependencySet> DependencySets { get; private set; }
            public Uri ReportAbuseUrl { get; private set; }
            public int DownloadCount { get; private set; }

            public IEnumerable<IPackageFile> GetFiles()
            {
                throw new NotImplementedException();
            }

            public IEnumerable<FrameworkName> GetSupportedFrameworks()
            {
                throw new NotImplementedException();
            }

            public Stream GetStream()
            {
                throw new NotImplementedException();
            }

            public bool IsAbsoluteLatestVersion { get; private set; }
            public bool IsLatestVersion { get; set; }
            public bool Listed { get; private set; }
            public DateTimeOffset? Published { get; private set; }
            public IEnumerable<IPackageAssemblyReference> AssemblyReferences { get; private set; }
        }
    }


}
