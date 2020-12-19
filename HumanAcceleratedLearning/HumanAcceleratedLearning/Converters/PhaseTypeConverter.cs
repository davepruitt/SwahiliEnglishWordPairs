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
    public class PhaseTypeConverter
    {
        /// <summary>
        /// Converts from a string to a PhaseType
        /// </summary>
        public static PhaseType ConvertDescriptionToPhaseType(string description)
        {
            var type = typeof(PhaseType);

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (PhaseType)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (PhaseType)field.GetValue(null);
                }
            }

            return PhaseType.Unknown;
        }

        /// <summary>
        /// Converts a phase type to a string description
        /// </summary>
        public static string ConvertPhaseTypeToDescription(PhaseType device_type)
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
