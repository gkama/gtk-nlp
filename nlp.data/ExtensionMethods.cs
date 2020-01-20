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
        /// Deserializes a <see cref="string"/> to a <see cref="{T}"/> by preserving self referencing loops
        /// </summary>
        public static T DeserializeSelfReferencing<T>(this string json)
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings
            {
                PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize
            };

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json, settings);
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

        /// <summary>
        /// Subtraction of two <see cref="IEnumerable{T}"/>
        /// </summary>
        public static IEnumerable<T> Minus<T>(this IEnumerable<T> orgList, IEnumerable<T> toRemove)
        {
            var list = orgList.OrderBy(x => x).ToList();
            foreach (var x in toRemove)
            {
                var inx = list.BinarySearch(x);
                if (inx >= 0) list.RemoveAt(inx);
            }
            return list;
        }

        /// <summary>
        /// Dot product of two <see cref="double[]"/>
        /// </summary>
        public static double DotProduct(this double[] A, double[] B)
        {
            var dotProduct = 0.0;
            for (int i = 0; i < A.Count(); i++)
            {
                dotProduct += A[i] * B[i];
            }

            return dotProduct;
        }

        /// <summary>
        /// Get the schema of an <see cref="object"/>
        /// </summary>
        public static object Schema(this object obj)
        {
            return obj.GetType()
                .GetProperties()
                .Select(x => new
                {
                    name = x.Name,
                    type = x.PropertyType.Name
                });
        }
    }
}
