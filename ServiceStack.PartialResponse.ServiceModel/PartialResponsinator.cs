using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace ServiceStack.PartialResponse.ServiceModel
{
    internal sealed class PartialResponsinator
    {
        private readonly List<FieldSelectorTreeNode> _partialSelectNodes;

        private readonly ConcurrentDictionary<string, PropertyInfo> _propertyInfoCache =
            new ConcurrentDictionary<string, PropertyInfo>();

        public PartialResponsinator(List<FieldSelectorTreeNode> partialSelectNodes)
        {
            _partialSelectNodes = partialSelectNodes;
        }

        public dynamic GetPartialResponse(object response)
        {
            if (response == null || _partialSelectNodes.IsEmpty())
            {
                return response;
            }

            var asEnumerable = response as IEnumerable;
            string typeFullName = asEnumerable == null
                                      ? response.GetType().FullName
                                      : asEnumerable.GetType().GetGenericArguments()[0].FullName;

            dynamic partialResponse = TraverseSelectorNodes(response, _partialSelectNodes, typeFullName);

            return partialResponse ?? new ExpandoObject();
        }

        private dynamic TraverseSelectorNodes(
            object responseNode, List<FieldSelectorTreeNode> selectorNodes, string parentSelectorPath)
        {
            if (responseNode == null)
            {
                return null;
            }

            var asEnumerable = responseNode as IEnumerable;

            if (asEnumerable != null)
            {
                return TraverseEnumerableObject(asEnumerable, selectorNodes, parentSelectorPath);
            }

            var expandoObject = new ExpandoObject() as IDictionary<string, object>;

            foreach (FieldSelectorTreeNode fieldSelectorTreeNode in selectorNodes)
            {
                KeyValuePair<string, object> value = TraverseSingleObject(
                    responseNode,
                    fieldSelectorTreeNode,
                    parentSelectorPath
                    );

                if (value.Value != null)
                {
                    expandoObject.Add(value);
                }
            }

            return expandoObject.Count == 0 ? null : expandoObject;
        }

        private dynamic TraverseEnumerableObject(
            IEnumerable response, List<FieldSelectorTreeNode> selectorNodes, string parentSelectorPath)
        {
            //This condition is not possible with the current way it is called in this class, 
            //therefore I am commenting it out since it can not be tested.
            //if (response == null)
            //{
            //    return new KeyValuePair<string, object>();
            //}

            var returnList = new List<ExpandoObject>();

            foreach (object enumeratedResp in response)
            {
                dynamic nodeValue = TraverseSelectorNodes(enumeratedResp, selectorNodes, parentSelectorPath);
                if (nodeValue != null)
                {
                    returnList.Add(nodeValue);
                }
            }

            return returnList;
        }

        private KeyValuePair<string, object> TraverseSingleObject(
            object responseNode, FieldSelectorTreeNode selectorNode, string parentSelectorPath)
        {
            string currentPathSelector = FormatSelectorPath(parentSelectorPath, selectorNode);

            PropertyInfo currentProperty = _propertyInfoCache.GetOrAdd(
                currentPathSelector,
                key =>
                responseNode.GetType().GetProperty(
                    selectorNode.MemberName,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                );

            if (currentProperty == null)
            {
                return new KeyValuePair<string, object>();
            }

            object value = currentProperty.GetValue(responseNode, null);

            if (selectorNode.Children.IsEmpty())
            {
                return new KeyValuePair<string, object>(currentProperty.Name, value);
            }

            object keyValue = TraverseSelectorNodes(value, selectorNode.Children, currentPathSelector);

            return new KeyValuePair<string, object>(currentProperty.Name, keyValue);
        }

        private static string FormatSelectorPath(string rootPath, FieldSelectorTreeNode subNode)
        {
            return string.Format("{0}/{1}", rootPath, subNode.MemberName);
        }
    }
}