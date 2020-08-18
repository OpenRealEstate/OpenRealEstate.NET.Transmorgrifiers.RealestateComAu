using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using OpenRealEstate.Core;

namespace OpenRealEstate.Transmorgrifiers.RealEstateComAu.Extensions
{
    internal static class XElementExtensions
    {
        internal static string Value(this XElement xElement,
                                     string elementName = null,
                                     string attributeName = null,
                                     string attributeValue = null)
        {
            var value = ValueOrDefault(xElement, elementName, attributeName, attributeValue);

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var errorMessage =
                string.Format(
                    "Expected the {0} '{1}' but failed to find it in the element '{2}' or that element exists, but with no data.",
                    string.IsNullOrWhiteSpace(attributeName) ||
                    string.IsNullOrWhiteSpace(attributeValue)
                        ? "element"
                        : "attribute",
                    string.IsNullOrWhiteSpace(attributeName) ||
                    string.IsNullOrWhiteSpace(attributeValue)
                        ? elementName
                        : attributeName,
                    xElement.Name);
            throw new Exception(errorMessage);
        }

        internal static string ValueOrDefault(this XElement xElement,
                                              string elementName = null,
                                              string attributeName = null,
                                              string attributeValue = null)
        {
            if (xElement == null)
            {
                throw new ArgumentNullException();
            }

            XElement element;
            if (string.IsNullOrWhiteSpace(elementName))
            {
                element = xElement;
            }
            else if (string.IsNullOrWhiteSpace(attributeName) &&
                     string.IsNullOrWhiteSpace(attributeValue))
            {
                element = xElement.Element(elementName);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(attributeValue))
                {
                    // We are trying to find the value of this attribute - so lets get the first element with this attribute.
                    element = xElement
                        .Descendants(elementName)
                        .FirstOrDefault(x => x.Attribute(attributeName) != null);
                }
                else
                {
                    // This is where things get tricky. We need to get the element that contains an attribute name AND attribute value.
                    // For example, an Agent section has 2x <telephone /> elements, but are different by the attributes.
                    // <telephone type="mobile" /> vs <telephone type="BH" />
                    element = xElement
                        .Descendants(elementName)
                        .FirstOrDefault(x => (string) x.Attribute(attributeName) == attributeValue);
                }
            }

            if (element == null)
            {
                return null;
            }

            // This is the next tricky part. Are we after the element value or the attribute value?
            var value = !string.IsNullOrWhiteSpace(attributeName) &&
                        string.IsNullOrWhiteSpace(attributeValue)
                            ? AttributeValueOrDefault(element, attributeName)
                            : element.Value.Trim();

            return string.IsNullOrWhiteSpace(value)
                       ? null
                       : value;
        }

        internal static void ValueOrDefaultIfExists(this XElement xElement,
                                                    Action<string> setValue,
                                                    string elementName = null,
                                                    string attributeName = null,
                                                    string attributeValue = null)
        {
            if (xElement == null)
            {
                throw new ArgumentNullException();
            }
            if (setValue == null)
            {
                throw new ArgumentNullException(nameof(setValue));
            }

            XElement element;
            if (!string.IsNullOrWhiteSpace(attributeName))
            {
                // Are we getting the element by name AND value?
                // This is where things get tricky. We need to get the element that contains an attribute name AND attribute value.
                // For example, an Agent section has 2x <telephone /> elements, but are different by the attributes.
                // <telephone type="mobile" /> vs <telephone type="BH" />
                if (!string.IsNullOrWhiteSpace(attributeValue))
                {
                    element = xElement
                        .Descendants(elementName)
                        .FirstOrDefault(x => (string)x.Attribute(attributeName) == attributeValue);
                }
                else
                {
                    // We are trying to find the value of this attribute - so lets get the first element with this attribute.
                    element = xElement
                        .Descendants(elementName)
                        .FirstOrDefault(x => x.Attribute(attributeName) != null);

                }
            }
            else if (!string.IsNullOrWhiteSpace(elementName))
            {
                element = xElement.Element(elementName);
            }
            else
            {
                element = xElement;
            }

            // There is no element found, so don't do anything.
            if (element == null)
            {
                return;
            }

            // This is the next tricky part. Are we after the element value or the attribute value?
            var value = !string.IsNullOrWhiteSpace(attributeName)
                            ? AttributeValueOrDefault(element, attributeName)
                            : element.Value.Trim();

            setValue(string.IsNullOrWhiteSpace(value)
                         ? null
                         : value);
        }

        internal static string AttributeValue(this XElement xElement,
                                              string attributeName)
        {
            var value = AttributeValueOrDefault(xElement, attributeName);

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var errorMessage = $"Expected the attribute '{attributeName}' but failed to find it in the element '{xElement.Name}'.";
            throw new Exception(errorMessage);
        }

        internal static string AttributeValueOrDefault(this XElement xElement,
                                                       string attributeName)
        {
            if (xElement == null)
            {
                throw new ArgumentNullException();
            }

            if (string.IsNullOrWhiteSpace(attributeName))
            {
                throw new ArgumentNullException(nameof(attributeName));
            }

            var attribute = xElement.Attribute(attributeName);
            return attribute?.Value;
        }

        internal static bool AttributeBoolValueOrDefault(this XElement xElement,
                                                         string attributeName)
        {
            if (xElement == null)
            {
                throw new ArgumentNullException();
            }

            if (string.IsNullOrWhiteSpace(attributeName))
            {
                throw new ArgumentNullException();
            }

            var attribute = xElement.Attribute(attributeName);
            if (attribute == null)
            {
                return false;
            }

            // Check to see if this value can be converted to a bool. Ie. 0/1/true/false.
            return bool.TryParse(attribute.Value, out var boolValue)
                       ? boolValue
                       : attribute.Value.ParseOneYesZeroNoToBool();
        }

        private static string ParsingErrorMessage(string value,
                                                  string parseName,
                                                  XElement xElement,
                                                  string elementName = null)
        {
            return $"Failed to parse element: {xElement.Name}{(string.IsNullOrEmpty(elementName) ? string.Empty : "." + elementName)}; value: '{value}' into a {parseName}.";
        }

        internal static int IntValueOrDefault(this XElement xElement,
                                              string elementName = null)
        {
            var value = xElement.ValueOrDefault(elementName);
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }

            if (int.TryParse(value, out var number))
            {
                return number;
            }

            var errorMessage = ParsingErrorMessage(value, "int", xElement, elementName);
            throw new Exception(errorMessage);
        }

        internal static decimal DecimalValueOrDefault(this XElement xElement,
                                                      string elementName = null)
        {
            var value = xElement.ValueOrDefault(elementName);
            if (string.IsNullOrEmpty(value))
            {
                return 0M;
            }

            // NOTE: This -cannot- handle currencies.
            if (decimal.TryParse(value, out var number))
            {
                return number;
            }

            var errorMessage = ParsingErrorMessage(value, "decimal", xElement, elementName);
            throw new Exception(errorMessage);
        }

        internal static decimal? NullableDecimalValueOrDefault(this XElement xElement,
                                                      string elementName = null)
        {
            var value = xElement.ValueOrDefault(elementName);
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            // NOTE: This -cannot- handle currencies.
            if (decimal.TryParse(value, out var number))
            {
                return number;
            }

            var errorMessage = ParsingErrorMessage(value, "decimal", xElement, elementName);
            throw new Exception(errorMessage);
        }

        internal static decimal MoneyValueOrDefault(this XElement xElement,
                                                    CultureInfo cultureInfo,
                                                    string elementName = null)
        {
            var value = xElement.ValueOrDefault(elementName);
            if (string.IsNullOrEmpty(value))
            {
                return 0M;
            }

            // NOTE: This can now handle values that are either currency or just numbers.
            //       ie. $1000, 1000, etc.
            if (decimal.TryParse(value,
                                 NumberStyles.AllowCurrencySymbol | NumberStyles.Number,
                                 cultureInfo,
                                 out var number))
            {
                return number;
            }

            var errorMessage = ParsingErrorMessage(value, "money", xElement, elementName);
            throw new Exception(errorMessage);
        }

        internal static void MoneyValueOrDefaultIfExists(this XElement xElement,
                                                         Action<decimal> setValue,
                                                         CultureInfo cultureInfo,
                                                         string elementName = null)
        {
            var value = xElement.ValueOrDefault(elementName);
            if (string.IsNullOrEmpty(value))
            {
                // Don't do anything.
                return;
            }

            // NOTE: This can now handle values that are either currency or just numbers.
            //       ie. $1000, 1000, etc.
            if (decimal.TryParse(value,
                                 NumberStyles.AllowCurrencySymbol | NumberStyles.Number,
                                 cultureInfo,
                                 out var number))
            {
                setValue(number);
                return;
            }

            // Damn it! Failed to parse the value as a decimal/money :(
            var errorMessage = ParsingErrorMessage(value, "decimal/money", xElement, elementName);
            throw new Exception(errorMessage);
        }

        internal static decimal? NullableMoneyValueOrDefault(this XElement xElement,
                                                             CultureInfo cultureInfo,
                                                             string elementName = null)
        {
            var value = xElement.ValueOrDefault(elementName);
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return xElement.MoneyValueOrDefault(cultureInfo, elementName);
        }

        internal static byte ByteValueOrDefault(this XElement xElement,
                                                string elementName = null)
        {
            var value = xElement.ValueOrDefault(elementName);
            return value.ParseByteValueOrDefault();
        }

        internal static void ByteValueOrDefaultIfExists(this XElement xElement,
                                                        Action<byte> setValue,
                                                        string elementName = null)
        {
            var action = new Action<string>(value =>
            {
                var byteValue = value.ParseByteValueOrDefault();
                setValue(byteValue);
            });

            xElement.ValueOrDefaultIfExists(action, elementName);
        }

        internal static bool BoolValueOrDefault(this XElement xElement,
                                                string elementName = null)
        {
            var value = xElement.ValueOrDefault(elementName);
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            // Checking for 0/1/YES/NO
            return bool.TryParse(value, out var boolValue)
                       ? boolValue
                       : value.ParseOneYesZeroNoToBool();
        }

        internal static byte BoolOrByteValueOrDefault(this XElement xElement,
                                                      string elementName = null)
        {
            var value = xElement.ValueOrDefault(elementName);
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }

            // We're checking to see if the value is YES/NO -before- we do our number check.
            // So the assumption here is that if it's not a YES/NO, then it's a number.
            return value.TryParseYesOrNoToBool(out var boolValue)
                       ? Convert.ToByte(boolValue)
                       : value.ParseByteValueOrDefault();
        }

        internal static Side SideOrDefault(this XElement xElement,
                                           string elementName,
                                           string attributeName)
        {
            var side = xElement.UnitOfMeasureOrDefault<Side>(elementName, attributeName);
            if (side == null)
            {
                return null;
            }

            side.Name = elementName;
            
            return side;
        }

        internal static T UnitOfMeasureOrDefault<T>(this XElement xElement,
                                                    string elementName,
                                                    string attributeName) where T : UnitOfMeasure, new()
        {
            var value = xElement.DecimalValueOrDefault(elementName);

            T unitOfMeasure = null;
            var type = xElement.ValueOrDefault(elementName, attributeName);

            if (value > 0)
            {
                unitOfMeasure = new T
                {
                    
                    Value = value,
                    Type = string.IsNullOrWhiteSpace(type)
                               ? "Total"
                               : type
                };
            }

            return unitOfMeasure;
        }

        internal static UnitOfMeasure UnitOfMeasureOrDefault(this XElement xElement,
                                                             string elementName,
                                                             string attributeName)
        {
            return xElement.UnitOfMeasureOrDefault<UnitOfMeasure>(elementName, attributeName);
        }

        internal static XElement StripNameSpaces(this XElement root)
        {
            var xElement = new XElement(
                root.Name.LocalName,
                root.HasElements
                    ? root.Elements().Select(StripNameSpaces)
                    : (object) root.Value
            );

            xElement.ReplaceAttributes(root.Attributes()
                                           .Where(attr => (!attr.IsNamespaceDeclaration)));

            return xElement;
        }
    }
}
