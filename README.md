ServiceStack.PartialResponse.ServiceModel
=========================================

Google Style Partial Responses for [ServiceStack.Net](https://github.com/ServiceStack/ServiceStack)

```
public object Get(MyRequestDto request)
{
  MyResponseDto response = SomeMethodThatGenerateTheResponse(request);
  return base.requestContext.ToPartialResponse(response)
}
```
--or, if you want it compressed--
```
public object Get(MyRequestDto request)
{
  MyResponseDto response = SomeMethodThatGenerateTheResponse(request);
  return base.ToOptimizedResult(base.requestContext.ToPartialResponse(response))
}
```
