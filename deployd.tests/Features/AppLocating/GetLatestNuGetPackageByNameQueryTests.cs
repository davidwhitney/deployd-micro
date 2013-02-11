using System;
using System.Collections.Generic;
using System.Linq;
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
            var packageOne = new NuGetPackageStub{Id = "app", Version = SemanticVersion.Parse("1.0.0.0")};
            var packageThree = new NuGetPackageStub{Id = "app", Version = SemanticVersion.Parse("1.5.0.0"), IsLatestVersion = true};
            var packageTwo = new NuGetPackageStub{Id = "app", Version = SemanticVersion.Parse("1.1.0.0")};
            _packageList.Add(packageOne);
            _packageList.Add(packageThree);
            _packageList.Add(packageTwo);

            var package = _query.GetLatestVersionOf("app", "c:\\location");

            Assert.That(package, Is.EqualTo(packageThree));
        }

        [Test]
        public void GetLatestVersionOf_NuGetRepoThrows_ReturnsLogsErrorAndReturnsNull()
        {
            var ex = new Exception();
            _repo.Setup(x => x.GetPackages()).Throws(ex);

            var result = _query.GetLatestVersionOf("app", "c:\\location");

            Assert.That(result, Is.Null);
            _log.Verify(x => x.Error("Could not get packages", ex));
        }
    }
}
