ServiceStack.PartialResponse.ServiceModel
=========================================

[Google Style Partial Responses](https://developers.google.com/+/api/#partial-responses) for [ServiceStack.Net](https://github.com/ServiceStack/ServiceStack). 

##Providing Field Selectors
Field Selectors can be passed using the header or query string. By default field selectors are combined form both. Duplicate field selectors are reduced. The field selector is applied to all entries in a list if the selector referes to a list. 

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

##Field Selector Example

`person(name(first,last),address(zip,street)),person/salary,link(url,description/short)`

This will select the person's first and last name, their salary, and the zip and street portions of their address. It will also select the url and short description of the link.

`people/name/first` 

`people` refers to a list. This will select the first name of all people in the list.

##Code Example

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
