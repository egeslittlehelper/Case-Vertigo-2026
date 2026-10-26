namespace VertigoCase.UI
{
    /// <summary>
    /// Pure math for wheel slice layout and spin target — static, no dependencies, fully testable.
    /// </summary>
    public static class WheelAngleCalculator
    {
        public static float SliceAngle(int count)
        {
            return 360f / count;
        }

        public static float SliceCenterAngle(int index, int count)
        {
            return index * SliceAngle(count);
        }

        public static float TargetRotation(int index, int count, int fullTurns, float jitterFraction)
        {
            float sliceAngle = SliceAngle(count);
            float center = index * sliceAngle;
            float jitter = jitterFraction * sliceAngle;
            return fullTurns * 360f + center + jitter;
        }

        public static float NormalizeAngle(float angle)
        {
            float result = angle % 360f;
            if (result < 0f) result += 360f;
            return result;
        }
    }
}
