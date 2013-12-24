using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;

namespace ServiceStack.PartialResponse.ServiceModel
{
    internal sealed class PartialResponsinator
    {
        private readonly List<FieldSelectorTreeNode> _partialSelectNodes;
        private readonly IPropertyValueGetterFactory _propertyGetterFactory;

        private readonly ConcurrentDictionary<string, IPropertyValueGetter> _propertyInfoCache =
            new ConcurrentDictionary<string, IPropertyValueGetter>();

        public PartialResponsinator(List<FieldSelectorTreeNode> partialSelectNodes, IPropertyValueGetterFactory propertyGetterFactory)
        {
            if (propertyGetterFactory == null)
            {
                throw new ArgumentNullException("propertyGetterFactory");
            }
            _partialSelectNodes = partialSelectNodes;
            _propertyGetterFactory = propertyGetterFactory;
        }

        public dynamic GetPartialResponse(object response)
        {
            if (response == null || _partialSelectNodes.IsEmpty())
            {
                return response;
            }

            var asEnumerable = response as IEnumerable;

            Type type = asEnumerable.IsGenericEnumerable()
                ? asEnumerable.GetType().GetGenericArguments()[0]
                : response.GetType();

            var typeFullName = type.IsDynamic() ? Guid.NewGuid().ToString() : type.FullName;

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

            if (asEnumerable.IsGenericEnumerable())
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

            IPropertyValueGetter currentProperty =
                _propertyInfoCache.GetOrAdd(
                    currentPathSelector,
                    key => _propertyGetterFactory.CreatePropertyValueGetter(responseNode, selectorNode.MemberName)
                    );

            if (currentProperty == null)
            {
                return new KeyValuePair<string, object>();
            }

            object value = currentProperty.GetPropertyValue(responseNode);

            if (selectorNode.Children.IsEmpty())
            {
                return new KeyValuePair<string, object>(currentProperty.PropertyName, value);
            }

            object keyValue = TraverseSelectorNodes(value, selectorNode.Children, currentPathSelector);

            return new KeyValuePair<string, object>(currentProperty.PropertyName, keyValue);
        }

        private static string FormatSelectorPath(string rootPath, FieldSelectorTreeNode subNode)
        {
            return string.Format("{0}{1}{2}", rootPath, FieldSelectorConstants.NestedFieldSelector, subNode.MemberName);
        }
    }
}