#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("6ZlAaryTAMVsFpPAsPyw4IiZKYnSYOPA0u/k68hkqmQV7+Pj4+fi4argIzhiCo+UrfOC4RD1uQyw7ItLf3O1qQz3CVekO+QgGKHqPTwq21Jg4+3i0mDj6OBg4+PiQD5EmZf4BaeQWX1V5/OBFCHB06VPvG9WICeBDHObK81io8qGuSKDg1nn6OHKy6CfdhLPQGESheajBOTq33JoeV0i+6MurXHCEIShy7EjLCpetI1ao3yj0+Xen7GT9PAYDTE7bjR2+8UKKu2htNV5dIvH6Feq3Pm7D8evoRiW3tWx8ahPe/9LsXwhGT3vsOAeKj3/CSj9OUAxfwNnPqbFzlh0MHaOELccHlllPWFx6028u30y2zXcCB80BwwVQE3O6jY6K+Dh4+Lj");
        private static int[] order = new int[] { 3,3,4,12,12,6,9,12,11,12,13,12,12,13,14 };
        private static int key = 226;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
