using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace nlp.data
{
    public static class ExtensionMethods
    {
        public static T ToObject<T>(this JsonElement element)
        {
            return JsonSerializer.Deserialize<T>(element.GetRawText());
        }

        public static T ToModel<T>(this JsonElement Model)
            where T : IModel<T>, new()
        {
            var model = new T()
            {
                Id = Model.GetProperty("id").GetString(),
                Name = Model.GetProperty("name").GetString(),
                Details = Model.GetProperty("details").GetString(),
            };

            var baseModelChildren = Model.GetProperty("children");
            var childrenList = new List<JsonElement>();
            for (int i = 0; i < baseModelChildren.GetArrayLength(); i++)
                childrenList.Add(baseModelChildren[i]);

            var stack = new Stack<JsonElement>(childrenList);
            while (stack.Any())
            {
                var child = stack.Pop();

                model.Children.Add(new T()
                {
                    Id = child.GetProperty("id").GetString(),
                    Name = child.GetProperty("name").GetString(),
                    Details = child.GetProperty("details").GetString(),
                });

                if (child.GetProperty("children").GetArrayLength() > 0)
                {
                    var childModels = child.GetProperty("children");
                    for (int i = 0; i < childModels.GetArrayLength(); i++)
                        stack.Push(childModels[i]);
                }
            }

            return model;
        }
    }
}
