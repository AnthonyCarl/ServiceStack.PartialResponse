ServiceStack.PartialResponse.ServiceModel
=========================================

[Google Style Partial Responses](https://developers.google.com/+/api/#partial-responses) for [ServiceStack.Net](https://github.com/ServiceStack/ServiceStack). 

##Providing Field Selectors
Field Selectors can be passed using the header or query string. By default field selectors are combined form both. Duplicate field selectors are reduced.

| Method | Example |
|:--:|:--|
| `Query String` | `fields=id&fields=todt` or `fields=id,todt` |
| `Header` | `x-fields: id,todt` |

##Field Selector Reserved Characters
| Character | Meaning |
|:--:|:--|
| `,`  | Separates multimple field selectors |
| `/` | Field sub selector  |
| `(` | Begin subselection expression |
| `)` | End subselection expression |

Currently `*` is not supported.

##Example

```c#
public object Get(MyRequestDto request)
{
  MyResponseDto response = SomeMethodThatGenerateTheResponse(request);
  return base.requestContext.ToPartialResponse(response)
}
```
--or, if you want it compressed--
```C#
public object Get(MyRequestDto request)
{
  MyResponseDto response = SomeMethodThatGenerateTheResponse(request);
  return base.ToOptimizedResult(base.requestContext.ToPartialResponse(response))
}
```

##ToDo

- Publish NuGet Package
- Finish Code for Client Side support
