namespace ServiceStack.PartialResponse.ServiceModel
{
    internal static class FieldSelectorConstants
    {
        public const char MultipleFieldSeparator = ',';
        public const char NestedFieldSelector = '/';
        public const char BeginSubSelectExpression = '(';
        public const char EndSubSelectExpression = ')';

        public static bool StartsWithReservedToken(string partialResponseFields)
        {
            if (string.IsNullOrWhiteSpace(partialResponseFields))
            {
                return false;
            }

            if (partialResponseFields[0] == MultipleFieldSeparator)
            {
                return true;
            }
            if (partialResponseFields[0] == NestedFieldSelector)
            {
                return true;
            }
            if (partialResponseFields[0] == BeginSubSelectExpression)
            {
                return true;
            }
            if (partialResponseFields[0] == EndSubSelectExpression)
            {
                return true;
            }

            return false;
        }
    }
}