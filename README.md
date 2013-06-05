ServiceStack.PartialResponse.ServiceModel
=========================================

[Google Style Partial Responses](https://developers.google.com/+/api/#partial-responses) for [ServiceStack.Net](https://github.com/ServiceStack/ServiceStack). Currently only the following Content types are supported:
- `application/json`
- `application/jsv`
- `text/html`

`application/xml` is NOT currently supported.

##Providing Field Selectors
Field Selectors can be passed using the header or query string. By default field selectors are combined form both. Duplicate field selectors are reduced. The field selector is applied to all entries in a list if the selector referes to a list. 

| Method | Example |
|:--:|:--|
| `Query String` | `http://myhost/mydtoroute?fields=id&fields=todt` or `http://myhost/mydtoroute?fields=id,todt` |
| `Header` | `x-fields: id,todt` |

If field selectors are passed in the query string, make sure it is properly encoded.

##Field Selector Reserved Characters
| Character | Meaning |
|:--:|:--|
| `,`  | Separates multimple field selectors |
| `/` | Field sub selector  |
| `(` | Begin subselection expression |
| `)` | End subselection expression |

Currently `*` is not supported.

**Note:** All field selector reserved characters may be nested.

##Field Selector Examples

**Example:** `person(name(first,last),address(zip,street)),person/salary,link(url,description/short)`

**Explanation:** This will select the person's first and last name, their salary, and the zip and street portions of their address. It will also select the url and short description of the link.

---

**Example:** `people/name/first`

**Explanation:** `people` refers to a list. This will select the first name of all people in the list.

##Code Examples

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

##Basic Benchmark

The DTO used contains 32 properties. The response is a list of 250 of these DTOs. The partial response only selects one integer property.

```
===================================
Completed 1000 Test Interations
===================================

===================================
Partial Response min/max/avg 12ms/323ms/15.208ms
===================================
Full Response min/max/avg 18ms/383ms/25.624ms
===================================
```

##ToDo

- Publish NuGet Package
- Finish Code for Client Side support
