using System.Collections.Generic;

namespace VertigoCase.Core
{
    /// <summary>
    /// Validates wheel configs against game rules — reused in WheelConfigSO.OnValidate.
    /// </summary>
    public static class WheelDefinitionValidator
    {
        public static IReadOnlyList<string> Validate(IWheelDefinition wheel)
        {
            var errors = new List<string>();

            if (wheel == null)
            {
                errors.Add("Wheel definition is null.");
                return errors;
            }

            var slices = wheel.Slices;
            if (slices == null || slices.Count == 0)
            {
                errors.Add("Wheel has no slices.");
                return errors;
            }

            int bombCount = 0;
            for (int i = 0; i < slices.Count; i++)
            {
                if (slices[i].IsBomb)
                    bombCount++;
            }

            switch (wheel.ZoneType)
            {
                case ZoneType.Normal:
                    if (bombCount != 1)
                        errors.Add($"Normal wheel must have exactly 1 bomb, found {bombCount}.");
                    break;

                case ZoneType.Safe:
                case ZoneType.Super:
                    if (bombCount > 0)
                        errors.Add($"{wheel.ZoneType} wheel must have no bombs, found {bombCount}.");
                    break;
            }

            return errors;
        }
    }
}
