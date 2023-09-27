using System.Globalization;
using System.Text;

namespace Sdcb.OpenVINO.NuGetBuilders.Extractors;

internal static class HexUtils
{
    internal static string ByteArrayToHexString(byte[] byteArray)
    {
        StringBuilder hex = new(byteArray.Length * 2);
        foreach (byte b in byteArray)
        {
            hex.AppendFormat("{0:x2}", b);
        }
        return hex.ToString();
    }

    internal static byte[] HexStringToByteArray(string hexString)
    {
        byte[] byteArray = new byte[hexString.Length / 2];
        for (int i = 0; i < byteArray.Length; i++)
        {
            string hex = hexString[(i * 2)..(i * 2 + 2)];
            byteArray[i] = byte.Parse(hex, NumberStyles.HexNumber);
        }
        return byteArray;
    }
}