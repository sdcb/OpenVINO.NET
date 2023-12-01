using OpenCvSharp;
using System;

namespace Sdcb.OpenVINO.PaddleOCR;

internal static class RectHelper
{
    public static float Distance(in Rect box1, in Rect box2)
    {
        int dx1 = Math.Abs(box2.X - box1.X);
        int dy1 = Math.Abs(box2.Y - box1.Y);
        int dx2 = Math.Abs(box2.Right - box1.Right);
        int dy2 = Math.Abs(box2.Bottom - box1.Bottom);

        float dis = dx1 + dy1 + dx2 + dy2;
        float dis_2 = dx1 + dy1;
        float dis_3 = dx2 + dy2;

        return dis + Math.Min(dis_2, dis_3);
    }

    public static float IntersectionOverUnion(in Rect box1, in Rect box2)
    {
        int x1 = Math.Max(box1.X, box2.X);
        int y1 = Math.Max(box1.Y, box2.Y);
        int x2 = Math.Min(box1.Right, box2.Right);
        int y2 = Math.Min(box1.Bottom, box2.Bottom);

        if (y1 >= y2 || x1 >= x2)
        {
            return 0.0f;
        }

        int intersectArea = (x2 - x1) * (y2 - y1);
        int box1Area = box1.Width * box1.Height;
        int box2Area = box2.Width * box2.Height;
        int unionArea = box1Area + box2Area - intersectArea;

        return (float)intersectArea / unionArea;
    }

    public static Rect Extend(in Rect rect, int extendLength)
    {
        Rect dst = new(rect.TopLeft, rect.Size);
        dst.Inflate(extendLength, extendLength);
        return dst;
    }
}
