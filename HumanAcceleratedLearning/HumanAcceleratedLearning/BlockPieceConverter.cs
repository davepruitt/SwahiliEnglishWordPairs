using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HumanAcceleratedLearning
{
    public class BlockPieceConverter
    {
        /// <summary>
        /// Converts a string to a BlockPiece.
        /// </summary>
        public static BlockPiece ConvertToBlockPiece(string description)
        {
            var type = typeof(BlockPiece);

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description.Equals(description, StringComparison.InvariantCultureIgnoreCase))
                        return (BlockPiece)field.GetValue(null);
                }
                else
                {
                    if (field.Name.Equals(description, StringComparison.InvariantCultureIgnoreCase))
                        return (BlockPiece)field.GetValue(null);
                }
            }

            return BlockPiece.S;
        }

        /// <summary>
        /// Converts a block piece type to a string description
        /// </summary>
        public static string ConvertToDescription(BlockPiece thresholdType)
        {
            FieldInfo fi = thresholdType.GetType().GetField(thresholdType.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return thresholdType.ToString();
        }
    }
}
