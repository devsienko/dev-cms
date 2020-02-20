using System.Linq;
using DevCms.ContentTypes;
using DevCms.Models;

namespace DevCms.Util
{
    public class DtoHelper
    {
        public static EditDictionaryDto GeEditDictionaryDto(Dictionary dictionary)
        {
            var result = new EditDictionaryDto
            {
                Id = dictionary.Id,
                Name = dictionary.Name,
                Items = dictionary.Items.Select(i => new EditDictionaryItemDto
                {
                    Id = i.Id,
                    Name = i.Name
                })
            };
            return result;
        }

        public static EditContentTypeModel GetEditContentTypeModel(EntityType entityType)
        {
            var result = new EditContentTypeModel
            {
                Id = entityType.Id,
                Name = entityType.Name,
                Attrs = entityType.Attrs.Select(a => new ContentAttributeModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    AttributeType = a.AttrType,
                    Required = a.Required,
                    DictionaryId = a.DictionaryId
                }).ToList()
            };
            return result;
        }

    }
}
