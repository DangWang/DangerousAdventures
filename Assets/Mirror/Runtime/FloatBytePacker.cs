using UnityEngine;

namespace Mirror
{
    public static class FloatBytePacker
    {
        // ScaleFloatToByte( -1f, -1f, 1f, byte.MinValue, byte.MaxValue) => 0
        // ScaleFloatToByte(  0f, -1f, 1f, byte.MinValue, byte.MaxValue) => 127
        // ScaleFloatToByte(0.5f, -1f, 1f, byte.MinValue, byte.MaxValue) => 191
        // ScaleFloatToByte(  1f, -1f, 1f, byte.MinValue, byte.MaxValue) => 255
        public static byte ScaleFloatToByte(float value, float minValue, float maxValue, byte minTarget, byte maxTarget)
        {
            // note: C# byte - byte => int, hence so many casts
            var targetRange = maxTarget - minTarget; // max byte - min byte only fits into something bigger
            var valueRange = maxValue - minValue;
            var valueRelative = value - minValue;
            return (byte) (minTarget + (byte) (valueRelative / valueRange * targetRange));
        }

        // ScaleByteToFloat(  0, byte.MinValue, byte.MaxValue, -1, 1) => -1
        // ScaleByteToFloat(127, byte.MinValue, byte.MaxValue, -1, 1) => -0.003921569
        // ScaleByteToFloat(191, byte.MinValue, byte.MaxValue, -1, 1) => 0.4980392
        // ScaleByteToFloat(255, byte.MinValue, byte.MaxValue, -1, 1) => 1
        public static float ScaleByteToFloat(byte value, byte minValue, byte maxValue, float minTarget, float maxTarget)
        {
            // note: C# byte - byte => int, hence so many casts
            var targetRange = maxTarget - minTarget;
            var valueRange = (byte) (maxValue - minValue);
            var valueRelative = (byte) (value - minValue);
            return minTarget + valueRelative / (float) valueRange * targetRange;
        }

        // eulerAngles have 3 floats, putting them into 2 bytes of [x,y],[z,0]
        // would be a waste. instead we compress into 5 bits each => 15 bits.
        // so a ushort.
        public static ushort PackThreeFloatsIntoUShort(float u, float v, float w, float minValue, float maxValue)
        {
            // 5 bits max value = 1+2+4+8+16 = 31 = 0x1F
            var lower = ScaleFloatToByte(u, minValue, maxValue, 0x00, 0x1F);
            var middle = ScaleFloatToByte(v, minValue, maxValue, 0x00, 0x1F);
            var upper = ScaleFloatToByte(w, minValue, maxValue, 0x00, 0x1F);
            var combined = (ushort) ((upper << 10) | (middle << 5) | lower);
            return combined;
        }

        // see PackThreeFloatsIntoUShort for explanation
        public static Vector3 UnpackUShortIntoThreeFloats(ushort combined, float minTarget, float maxTarget)
        {
            var lower = (byte) (combined & 0x1F);
            var middle = (byte) ((combined >> 5) & 0x1F);
            var upper = (byte) (combined >> 10); // nothing on the left, no & needed

            // note: we have to use 4 bits per float, so between 0x00 and 0x0F
            var u = ScaleByteToFloat(lower, 0x00, 0x1F, minTarget, maxTarget);
            var v = ScaleByteToFloat(middle, 0x00, 0x1F, minTarget, maxTarget);
            var w = ScaleByteToFloat(upper, 0x00, 0x1F, minTarget, maxTarget);
            return new Vector3(u, v, w);
        }
    }
}