using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HumanAcceleratedLearning.Models;

namespace HumanAcceleratedLearning.Converters
{
    public class LanguageClassificationConverter
    {
        /// <summary>
        /// Converts from a string to a LanguageClassification
        /// </summary>
        public static LanguageClassification ConvertDescriptionToLanguageClassification (string description)
        {
            var type = typeof(LanguageClassification);

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (LanguageClassification)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (LanguageClassification)field.GetValue(null);
                }
            }

            return LanguageClassification.Undefined;
        }

        /// <summary>
        /// Converts a LanguageClassification to a string description
        /// </summary>
        public static string ConvertLanguageClassificationToDescription(LanguageClassification device_type)
        {
            FieldInfo fi = device_type.GetType().GetField(device_type.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return device_type.ToString();
        }
    }
}
