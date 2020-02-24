using System;
using System.Collections.Generic;
using System.Linq;
using DevCms.ContentTypes;
using DevCms.Models;
using Microsoft.EntityFrameworkCore;

namespace DevCms.Util
{
    public class ContentManager
    {
        private readonly DevCmsDb _db;

        public ContentManager(DevCmsDb db)
        {
            _db = db;
        }

        public IEnumerable<Entity> this[string typeName] => GetList(typeName);

        //todo: test
        public Entity Get(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                throw new ArgumentNullException(nameof(typeName));
            var result = _db.Content
                .Where(c => c.EntityType.Name == typeName)
                .Include(c => c.AttrValues)
                .ThenInclude(av => av.Attr)
                .First();

            return result;
        }

        //todo: test
        public IEnumerable<Entity> GetList(string typeName)
        {
            if(string.IsNullOrEmpty(typeName))
                throw new ArgumentNullException(nameof(typeName));
            var result = _db.Content
                .Where(c => c.EntityType.Name == typeName)
                .Include(c => c.AttrValues)
                .ThenInclude(av => av.DictionaryItem)
                .Include(c => c.AttrValues)
                .ThenInclude(av => av.Attr)
                .AsEnumerable();

            return result.Any()
                ? result
                : new List<Entity>();
        }
    }
}
