﻿using Common.Util;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Common.XUnitTest
{
    public class TestModel
    {
        //[Description("id")]
        public int Id { get; set; }

        //[Description("name")]
        public string Name { get; set; }
    }

    public class MapUtilUnitTest : BaseUnitTest
    {
        private IMapUtil _mapUtil { get; set; }

        public MapUtilUnitTest() : base() 
        {
            _serviceCollection.AddMapUtil();
            BuilderServiceProvider();
            _mapUtil = _serviceProvider.GetService<IMapUtil>();
        }

        [Fact]
        public void EntityToDictionaryTest()
        {
            var model = new TestModel()
            {
                Id = 10,
                Name="Jack"
            };
            var dict = _mapUtil.EntityToDictionary(model);
            Assert.Equal(dict["id"], 10.ToString());
        }

        [Fact]
        public void ObjectToDictionaryTest()
        {
            var model = new TestModel()
            {
                Id = 10,
                Name = "Jack"
            };
            var dict = _mapUtil.EntityToDictionary(model);
            Assert.Equal(dict["id"], 10.ToString());
        }

        [Fact]
        public void EntityToCookieStrListTest()
        {
            var model = new TestModel()
            {
                Id = 10,
                Name = "Jack"
            };
            var list = _mapUtil.EntityToCookieStrList(model);
            Assert.True(list.Where(x=>x.Contains("id")).Any());
        }

        [Fact]
        public void GetAllPropertyNamesTest()
        {
            var s = _mapUtil.GetAllPropertyNames(typeof(TestModel));
            Assert.Equal(2, s.Length);
        }

        [Fact]
        public async Task GetAllPropertyNameTest()
        {
            var s = _mapUtil.GetAllPropertyName(typeof(TestModel));
            Assert.Equal(2, s.Length);
        }
    }
}
