# gtk-nlp

Completely customizable natural language processing (nlp) API

## Overview
This API is designed to take text as input and perform a categorization algorithm. The text can be either summarized or not. The pseudo code for the algorithm is:
- Get a `string` as the input (content text)
- Summarize the content text or not?
- Iterate through each model's details and check if it exists in the content text
- Compile the categorization response
- Return the categorization response to the end user

##### Input
The API can take multiple combinations of input from end users. They are represented in the form of a `NlpRequest<T>` object where not all fields are required

```csharp
public class NlpRequest<T> : INlpRequest<T>
{
    public string Content { get; set; }         //Required
    public T Model { get; set; }                //Optional: if `ModelId` is provided
    public string[] Delimiters { get; set; }    //Optional: use default if not provided
    public string[] StopWords { get; set; }     //Optional: use default if not provided
    public string ModelId { get; set; }         //Optional: if `T Model` is not provided and must be together with `ModelName` and `ModelDetails`
    public string ModelName { get; set; }       //Optional: must be together with `ModelId` and `ModelName`
    public string ModelDetails { get; set; }    //Optional: must be together with `ModelId` and `ModelName`
}
```

Additionally, there are 2 endpoints that take the categorization request. One for pure `NlpRequest<T>` object and the other with a model id that is available in the static models, such as `Vanguard`
- `../nlp/categorize`
- `../nlp/categorize/{modelId}`

###### Examples
- Example - `../nlp/categorize` - with content text, `T Model` populated, some delimiters and stop words

```json
{
	"model": {
		"id": "test model 1",
		"name": "test model",
		"details": "find, me, please",
		"children": [
			{
				"id": "child model 1",
				"name": "child model 1",
				"details": "1, 2, 3",
				"children": [
					{
						"id": "child model of child model 1",
						"name": "child's child model",
						"details": "fascinating|me|pls",
						"children": [
							{
								"id": "testing another level",
								"name": "yep",
								"details": ""
							}
						]
					}
				]
			}
		]
	},
	"delimiters": [
		","
	],
	"stopWords": [
		"test",
		"test1"
	],
	"content": "this is to test the categorization with find me and me but not this really and 1 and fascinating"
}
```

##### Output
The response contains the categorization description

```csharp
public class NlpResponse : INlpResponse
{
    public bool Summarized { get; set; } = false;
    public int? SummarizedLength { get; set; }
    public int Length { get; set; }
    public ICollection<ICategory> Categories { get; set; } = new List<ICategory>();
}
```
```csharp
public class Category : ICategory
{
    public string Name { get; set; }
    public int TotalWeight => Matched.Sum(x => x.Weight);
    public double TotalWeightPercentage { get; set; }
    public ICollection<IMatched> Matched { get; set; } = new List<IMatched>();
}

public class Matched : IMatched
{
    public string Value { get; set; }
    public int Weight { get; set; } = 1;
    public double WeightPercentage { get; set; }
}
```

###### Examples
- Response from above request with content text, `T Model` populated, some delimiters and stop words

```json
{
  "summarized": false,
  "length": 96,
  "categories": [
    {
      "name": "test model",
      "totalWeight": 2,
      "totalWeightPercentage": 2.08,
      "matched": [
        {
          "value": "find",
          "weight": 1,
          "weightPercentage": 1.04
        },
        {
          "value": "me",
          "weight": 1,
          "weightPercentage": 1.04
        }
      ]
    },
    {
      "name": "child model 1",
      "totalWeight": 1,
      "totalWeightPercentage": 1.04,
      "matched": [
        {
          "value": "1",
          "weight": 1,
          "weightPercentage": 1.04
        }
      ]
    },
    {
      "name": "child's child model",
      "totalWeight": 2,
      "totalWeightPercentage": 2.08,
      "matched": [
        {
          "value": "me",
          "weight": 1,
          "weightPercentage": 1.04
        },
        {
          "value": "fascinating",
          "weight": 1,
          "weightPercentage": 1.04
        }
      ]
    }
  ]
}
```