using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Tests
{
    using Moq;

    using OnlineShop.Data.Interfaces;
    using OnlineShop.Models;

    public class MockContainer
    {
        public Mock<IRepository<Ad>> AdRepositoryMock { get; set; }

        public Mock<IRepository<AdType>> AdTypeRepositoryMock { get; set; }

        public Mock<IRepository<Category>> CategoryRepositoryMock { get; set; }

        public Mock<IRepository<ApplicationUser>> UserRepositoryMock { get; set; }

        public void PrepareMocks()
        {
            this.SetupFakeCategories();
            this.SetupFakeAds();
            this.SetupFakeAdTypes();
            this.SetupFakeeUsers();
        }

        private void SetupFakeeUsers()
        {
            var fakeUsers = new List<ApplicationUser>()
            {
               new ApplicationUser() { Id = "123", UserName = "batePesho" },
               new ApplicationUser() { Id = "1234", UserName = "bateGosho" },
               new ApplicationUser() { Id = "12345", UserName = "babaGicka" },
            };

            this.UserRepositoryMock = new Mock<IRepository<ApplicationUser>>();
            this.UserRepositoryMock.Setup(r => r.All()).Returns(fakeUsers.AsQueryable());
            this.UserRepositoryMock.Setup(r => r.Find(It.IsAny<int>())).Returns(
                (string id) =>
                    {
                        var user = fakeUsers.FirstOrDefault(u => u.Id == id);
                        return user;
                    });
        }

        private void SetupFakeAdTypes()
        {
            var fakeAdTypes = new List<AdType>()
            {
                new AdType() { Id = 1, Name = "Normal", Index = 100 },
                new AdType() { Id = 2, Name = "Premium", Index = 200 },
                new AdType() { Id = 3, Name = "Gold", Index = 300 },
                new AdType() { Id = 4, Name = "Diamond", Index = 400 }
            };

            this.AdTypeRepositoryMock = new Mock<IRepository<AdType>>();
            this.AdTypeRepositoryMock.Setup(r => r.All()).Returns(fakeAdTypes.AsQueryable());
            this.AdTypeRepositoryMock.Setup(r => r.Find(It.IsAny<int>())).Returns(
                (int id) =>
                    {
                        var adType = fakeAdTypes.FirstOrDefault(at => at.Id == id);
                        return adType;
                    });
        }

        private void SetupFakeAds()
        {
            var adTypes = new List<AdType>()
            {
                new AdType()
                {
                    Name = "Normal", Index = 100,
                },
                new AdType()
                {
                    Name = "Premium", Index = 200,
                }
            };

            var fakeAds = new List<Ad>()
            {
                new Ad()
                    {
                        Id = 5,
                        Name = "Audi A6",
                        Type = adTypes[0],
                        PostedOn = DateTime.Now.AddDays(-6),
                        Owner = new ApplicationUser()
                                    {
                                        UserName = "gosho",
                                        Id = "123"
                                    },
                        Price = 400
                    },
                new Ad()
                    {
                        Id = 6,
                        Name = "Na Baba Gicka Gashtite",
                        Type = adTypes[1],
                        PostedOn = DateTime.Now.AddDays(-3),
                        Owner = new ApplicationUser()
                                    {
                                        UserName = "parlapun",
                                        Id = "1234"
                                    },
                        Price = 880
                    },
                new Ad()
                    {
                        Id = 7,
                        Name = "Album na Azis",
                        Type = adTypes[1],
                        PostedOn = DateTime.Now.AddDays(-2),
                        Owner = new ApplicationUser()
                                    {
                                        UserName = "pesho",
                                        Id = "12345"
                                    },
                        Price = 150
                    }
            };

            this.AdRepositoryMock = new Mock<IRepository<Ad>>();
            this.AdRepositoryMock.Setup(r => r.All()).Returns(fakeAds.AsQueryable());
            this.AdRepositoryMock.Setup(r => r.Find(It.IsAny<int>())).Returns(
                (int id) =>
                    {
                        var ad = fakeAds.FirstOrDefault(a => a.Id == id);
                        return ad;
                    });
        }

        private void SetupFakeCategories()
        {
            var fakeCategories = new List<Category>()
            {
                 new Category() { Id = 2, Name = "Cars" },
                 new Category() { Id = 3, Name = "Music" },
                 new Category() { Id = 4, Name = "Other" },
            };

            this.CategoryRepositoryMock = new Mock<IRepository<Category>>();
            this.CategoryRepositoryMock.Setup(r => r.All()).Returns(fakeCategories.AsQueryable());
            this.CategoryRepositoryMock.Setup(r => r.Find(It.IsAny<int>())).Returns(
                (int id) =>
                    {
                        var category = fakeCategories.FirstOrDefault(c => c.Id == id);
                        return category;
                    });
        }
    }
}
