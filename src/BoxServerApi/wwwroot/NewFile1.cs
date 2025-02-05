using System.Text.Json.Nodes;
using Json.Schema;

var schema = """
             {
               "$schema": "https://json-schema.org/draft/2020-12/schema",
               "$id": "https://example.com/product.schema.json",
               "title": "Product",
               "description": "A product from Acme's catalog",
               "type": "object",
               "properties": {
                 "productName": {
                   "description": "Name of the product",
                   "type": "string"
                 },
                 "price": {
                   "description": "The price of the product",
                   "type": "number",
                   "exclusiveMinimum": 0
                 }
               },
               "required": [ "productName", "price" ]
             }
             """;

var invalidJson = """
                  {
                      "productName": "test",
                      "price": -1
                  }
                  """;

// Validate the JSON data against the JSON schema
var jsonSchema = JsonSchema.FromText(schema);
hema.FromText(schema);
var result = jsonSchema.Evaluate(JsonNode.Parse(invalidJson), new EvaluationOptions { OutputFormat = OutputFormat.List });
if (!result.IsValid)
{
    Console.WriteLine("Invalid document");
    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result.Details));

    if (result.HasErrors)
    {
        foreach (var error in result.Errors)
        {
            Console.WriteLine(error.Key + ": " + error.Value);
        }
    }
}
else
{
    Console.WriteLine("Valid document");
}