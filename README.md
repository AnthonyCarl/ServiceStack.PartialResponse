ServiceStack.PartialResponse.ServiceModel
=========================================

[Google Style Partial Responses](https://developers.google.com/+/api/#partial-responses) for [ServiceStack.Net](https://github.com/ServiceStack/ServiceStack). Currently only the following Content types are supported:
- `application/json`
- `application/jsv`
- `text/html`

`application/xml` is NOT currently supported.

I wanted to implement this as a ServiceStack `IPlugin`, but I was unable to figure out how to get the access I needed to the response DTO for my approach. Currently, this is implemented as an `IRequestContext` extension.

##Providing Field Selectors
Field Selectors can be passed using the header or query string. By default field selectors are combined form both. Duplicate field selectors are reduced. The field selector is applied to all entries in a list if the selector refers to a list. 

| Method | Example |
|:--:|:--|
| `Query String` | `http://myhost/mydtoroute?fields=id&fields=todt` or `http://myhost/mydtoroute?fields=id,todt` |
| `Header` | `x-fields: id,todt` |

If field selectors are passed in the query string, make sure it is properly encoded.

##Field Selector Reserved Characters
| Character | Meaning |
|:--:|:--|
| `,`  | Separates multiple field selectors |
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

##Using the Code

* [Install the NuGet Package](https://nuget.org/packages/ServiceStack.PartialResponse.ServiceModel)
* [NuGet Packages from Latest Successful Build](http://teamcity.codebetter.com/viewLog.html?buildTypeId=bt1037&buildId=lastSuccessful)
* You can check out the code and run build.bat.
  * It will create NuGet packages you can consume in `.\ReleasePackages` or you can directly use the resulting binaries. 
  * If you use a custom made NuGet package and have an internal symbol server, you may be interested in the `IndexSrc` build target to properly index the source code back to GitHub.
* Build requirements
  * .Net 4.0
  * Powershell 2.0


##ToDo

- Publish Code for Client Side support. I have working strongly typed implementation (using expression trees) but it needs to be cleaned up for release.

![CodeBetter CI](http://www.jetbrains.com/img/banners/Codebetter.png)

Special Thanks to [JetBrains](http://www.jetbrains.com/teamcity) and [CodeBetter](http://codebetter.com/codebetter-ci/) for hosting this project!
