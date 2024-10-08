# Brand_Details_Requires_Brand_Name_With_Proper_Type_With_Explicit_Generic

## SQL

```text
SELECT b."Name", b."Id"
FROM "Brands" AS b
WHERE b."Id" = 1
```

## Result

```json
{
  "data": {
    "brandById": {
      "details": "Brand Name:Brand0"
    }
  }
}
```
