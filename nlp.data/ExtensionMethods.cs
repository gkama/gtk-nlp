using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace nlp.data
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Converts a <see cref="JsonElement"/> object to <see cref="{T}"/>
        /// </summary>
        public static T ToObject<T>(this JsonElement element)
        {
            return JsonSerializer.Deserialize<T>(element.GetRawText());
        }

        /// <summary>
        /// Converts a <see cref="JsonElement"/> object to <see cref="IModel{T}"/>
        /// </summary>
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
            var parentModel = model;
            while (stack.Any())
            {
                var child = stack.Pop();
                var childModel = new T()
                {
                    Id = child.GetProperty("id").GetString(),
                    Name = child.GetProperty("name").GetString(),
                    Details = child.GetProperty("details").GetString(),
                };

                parentModel.Children
                    .Add(childModel);

                var children = new JsonElement();
                child.TryGetProperty("children", out children);

                if (children.ValueKind != JsonValueKind.Undefined)
                {
                    var childModels = child.GetProperty("children");
                    for (int i = 0; i < childModels.GetArrayLength(); i++)
                        stack.Push(childModels[i]);
                }

                parentModel = childModel;
            }

            return model;
        }

        /// <summary>
        /// If a <see cref="ICategory"/> exists in the <paramref name="Categories"/> then it either adds the <paramref name="Value"/>
        /// to the <see cref="ICategory.Matched"/> list of it updates the <see cref="IMatched.Weight"/> and <seealso cref="ICategory.TotalWeight"/> accordingly.
        /// If a <see cref="ICategory"/> doesn't exist, then it simply adds it to the list. The majority of the work is done if a <see cref="ICategory"/>
        /// already exists and it needs to be updated accordingly
        /// </summary>
        public static void AddCategory(this ICollection<ICategory> Categories, string ModelName, string Value)
        {
            var category = Categories
                .FirstOrDefault(x => x.Name == ModelName);

            if (category != null
                && category.Matched
                    .Any(x => string.Compare(x.Value, Value, StringComparison.OrdinalIgnoreCase) == 0))
            {
                category.Matched
                    .FirstOrDefault(x => string.Compare(x.Value, Value, StringComparison.OrdinalIgnoreCase) == 0)
                    .Weight++;
            }
            else if (category != null
                && category.Matched
                    .Any(x => string.Compare(x.Value, Value, StringComparison.OrdinalIgnoreCase) != 0))
            {
                category.Matched
                    .Add(new Matched()
                    {
                        Value = Value
                    });
            }
            else
            {
                var newCategory = new Category() { Name = ModelName };

                newCategory.Matched
                    .Add(new Matched()
                    {
                        Value = Value
                    });

                Categories.Add(newCategory);
            }
        }

        /// <summary>
        /// Populates an array with a specific value <see cref="{T}"/>
        /// </summary>
        public static T[] Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }

            return arr;
        }
    }
}
