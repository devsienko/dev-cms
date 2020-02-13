using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using DevCms.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;

namespace DevCms.Tests
{
    internal static class DevCmsDbMocks
    {
        public static Mock<MockableDbSetWithExtensions<T>> SetupDbSetMock<T>(this Mock<DevCmsDb> db, 
            Expression<Func<DevCmsDb, DbSet<T>>> dbSetSelector, List<T> listItems, bool verifiable = false)
             where T : class
        {
            var listMock = CreateMockableDbSetMock(listItems);
            if (verifiable)
            {
                db.Setup(dbSetSelector)
                    .Returns(listMock.Object)
                    .Verifiable();
                db.Setup(x => x.Set<T>())
                    .Returns(listMock.Object)
                    .Verifiable();
            }
            else
            {
                db.Setup(dbSetSelector).Returns(listMock.Object);
                db.Setup(x => x.Set<T>()).Returns(listMock.Object);
            }

            return listMock;
        }

        public static int IdentityCounter = 1;

        public static Mock<MockableDbSetWithExtensions<T>> CreateMockableDbSetMock<T>(List<T> list)
            where T : class
        {
            var queryable = list.AsQueryable();
            var result = new Mock<MockableDbSetWithExtensions<T>>();
            result.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            result.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            result.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            result.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            result.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(item =>
            {
                var itemType = item.GetType();
                var idPi = itemType.GetProperty("Id");
                // if it's int or long and then set autoincremented value,
                // but don't set autoincremented value if unit test explicitly tries to set the value
                if (idPi != null && (idPi.PropertyType == typeof(int) && (int)idPi.GetValue(item) <= 0
                    || idPi.PropertyType == typeof(long) && (long)idPi.GetValue(item) <= 0))
                {
                    idPi.SetValue(item, IdentityCounter++);
                }
                list.Add(item);
            });

            result.Setup(m => m.Remove(It.IsAny<T>())).Callback<T>(entity => list.Remove(entity));
            result.Setup(m => m.AddOrUpdate(It.IsAny<T>())).Callback<T[]>(items =>
            {
                if (!EnumerableExtensions.Any(items))
                    return;
                foreach (var item in items)
                {
                    var itemType = item.GetType();
                    var props = itemType.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(KeyAttribute)));
                    if (props == null || !props.Any())
                        AddOrUpdateById(item, list);
                    else
                        AddOrUpdateByAttribute(item, list);
                }
            });
            result.Setup(m => m.AddRange(It.IsAny<IEnumerable<T>>()))
                .Callback<IEnumerable<T>>(entity => list.AddRange(entity));
            return result;
        }

        private static void AddOrUpdateById<T>(T item, List<T> list)
        {
            var itemType = item.GetType();
            var idPi = itemType.GetProperty("Id");
            if (idPi != null && (idPi.PropertyType == typeof(int) || idPi.PropertyType == typeof(long)))
            {
                var id = (long)idPi.GetValue(itemType);
                var index = list.FindIndex(i => GetIdValue(i) != id);
                if (index != -1)
                    list[index] = item;
                else
                    list.Add(item);
            }
        }

        private static void AddOrUpdateByAttribute<T>(T item, List<T> list)
        {
            var itemType = item.GetType();
            var props = itemType.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(KeyAttribute)));
            if (!EnumerableExtensions.Any(list))
            {
                list.Add(item);
                return;
            }
            var isExist = true;
            var index = -1;
            foreach (var existingItem in list)
            {
                isExist = true;
                foreach (var prop in props)
                {
                    var value = GetKeyPropertyValue(item, prop.Name);
                    if (value != null && !value.Equals(GetKeyPropertyValue(existingItem, prop.Name)))
                    {
                        isExist = false;
                        break;
                    }
                }
                if (isExist)
                {
                    index = list.IndexOf(existingItem);
                    break;
                }

            }
            if (isExist)
                list[index] = item;
            else
                list.Add(item);
        }

        private static long GetIdValue(object item)
        {
            var itemType = item.GetType();
            var idPi = itemType.GetProperty("Id");
            var result = (long)idPi.GetValue(itemType);
            return result;
        }

        private static object GetKeyPropertyValue(object item, string propertyName)
        {
            var itemType = item.GetType();
            var idPi = itemType.GetProperty(propertyName);
            var result = (object)idPi.GetValue(item);
            return result;
        }
    }
}
