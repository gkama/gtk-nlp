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
                var newMatched = new Matched() { Value = Value };

                newMatched.Weight++;
                category.Matched
                    .Add(newMatched);
            }
            else
            {
                var newCategory = new Category() { Name = ModelName };
                var newMatched = new Matched() { Value = Value };

                newMatched.Weight++;
                newCategory.Matched.Add(newMatched);

                Categories.Add(newCategory);
            }
        }
    }
}
