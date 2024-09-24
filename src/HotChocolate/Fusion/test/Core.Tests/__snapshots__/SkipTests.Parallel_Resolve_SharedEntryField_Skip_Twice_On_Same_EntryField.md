# Parallel_Resolve_SharedEntryField_Skip_Twice_On_Same_EntryField

## Result

```json
{
  "errors": [
    {
      "message": "Only one of each directive is allowed per location.",
      "locations": [
        {
          "line": 1,
          "column": 39
        }
      ],
      "extensions": {
        "specifiedBy": "https://spec.graphql.org/October2021/#sec-Directives-Are-Unique-Per-Location"
      }
    }
  ],
  "data": {
    "viewer": null
  }
}
```

## Request

```graphql
query Test {
  viewer @skip(if: true) {
    __typename
  }
  viewer @skip(if: false) {
    __typename
  }
}
```

## QueryPlan Hash

```text
02D05A80BBDC9A838BB8B907071A985AD35C6A82
```

## QueryPlan

```json
{
  "document": "query Test { viewer @skip(if: true) { __typename } viewer @skip(if: false) { __typename } }",
  "operation": "Test",
  "rootNode": {
    "type": "Sequence",
    "nodes": [
      {
        "type": "Resolve",
        "subgraph": "Subgraph_1",
        "document": "query Test_1 { viewer @skip(if: true) @skip(if: false) { __typename } }",
        "selectionSetId": 0
      },
      {
        "type": "Compose",
        "selectionSetIds": [
          0
        ]
      }
    ]
  }
}
```

