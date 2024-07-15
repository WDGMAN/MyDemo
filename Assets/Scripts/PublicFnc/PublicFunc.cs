
    using System;
    using WDGFrame;

    public static class PublicFunc
    {
        /// <summary>
        /// 此方法用以解决浮点数间的过于小的差异而不相等
        /// </summary>
        /// <returns></returns>
        public static bool CompareFloatEqual(float a,float b)
        {
            if (MathF.Abs(a - b) <= 0.00011f) return true;
            else return false;
        }
        
        
    }
