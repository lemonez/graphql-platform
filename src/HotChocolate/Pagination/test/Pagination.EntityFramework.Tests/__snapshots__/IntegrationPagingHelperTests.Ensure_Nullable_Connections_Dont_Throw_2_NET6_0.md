# Ensure_Nullable_Connections_Dont_Throw_2

## SQL

```text
SELECT f."Id", f."Name", b."Id" IS NULL, b."Id", b."Description", b."SomeField1", b."SomeField2"
FROM "Foos" AS f
LEFT JOIN "Bars" AS b ON f."BarId" = b."Id"
ORDER BY f."Name", f."Id"
```

## Expression

```text
[Microsoft.EntityFrameworkCore.Query.QueryRootExpression].OrderBy(t => t.Name).ThenBy(t => t.Id).Select(root => new Foo() {Id = root.Id, Name = IIF((root.Name == null), null, root.Name), Bar = IIF((root.Bar == null), null, new Bar() {Id = root.Bar.Id, Description = IIF((root.Bar.Description == null), null, root.Bar.Description), SomeField1 = IIF((root.Bar.SomeField1 == null), null, root.Bar.SomeField1), SomeField2 = IIF((root.Bar.SomeField2 == null), null, root.Bar.SomeField2)})})
```

## Result

```json
{
  "data": {
    "foos": {
      "edges": [
        {
          "cursor": "Rm9vIDE6MQ=="
        },
        {
          "cursor": "Rm9vIDI6Mg=="
        }
      ],
      "nodes": [
        {
          "id": 1,
          "name": "Foo 1",
          "bar": null
        },
        {
          "id": 2,
          "name": "Foo 2",
          "bar": {
            "id": 1,
            "description": "Bar 1",
            "someField1": "abc",
            "someField2": null
          }
        }
      ],
      "pageInfo": {
        "hasNextPage": false,
        "hasPreviousPage": false,
        "startCursor": "Rm9vIDE6MQ==",
        "endCursor": "Rm9vIDI6Mg=="
      }
    }
  }
}
```
