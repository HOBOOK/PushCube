#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("CONyNoyEXC/cN8u+sJVABWTwJPMjL2xqfXtmaWZsbntqL39gY2ZsdgsJHA1aXD4cPx4JDFoLBRwFTn9/Px4JDFoLBRwFTn9/Y2ovRmFsIT4CCQYliUeJ+AIODgoKDwyNDg4PU0bXeZA8G2queJvGIg0MDg8OrI0OMiloL4U8ZfgCjcDR5Kwg9lxlVGuAfI5vyRRUBiCdvfdLR/9vN5Ea+hk/GwkMWgsMHAJOf39jai9dYGB7YWsvbGBha2Z7ZmBhfC9gaS96fGqEFobR9kRj+gikLT8N5xcx918G3GiAB7sv+MSjIy9gf7kwDj+DuEzAY2ovRmFsIT4pPysJDFoLBBwSTn/PbDx4+DUII1nk1QAuAdW1fBZAun1ubHtmbGovfHtue2piamF7fCE/AJIy/CRGJxXH8cG6tgHWURPZxDI5lkMid7jig5TT/HiU/XndeD9Azro1ovsAAQ+dBL4uGSF72jMC1G0ZL25hay9san17ZmlmbG57ZmBhL38QioyKFJYySDj9ppRPgSPbvp8d18YWffpSAdpwUJT9Kgy1WoBCUgL+azosGkQaVhK8m/j5k5HAX7XOV1+akXUDq0iEVNsZODzEywBCwRtm3hCe1BFIX+QK4lF2iyLkOa1YQ1rjSnEQQ2RfmU6Gy3ttBB+MTog8hY4liUeJ+AIODgoKDz9tPgQ/BgkMWn9jai9dYGB7L0xOPxEYAj85Pzs9IU+p+EhCcAdRPxAJDFoSLAsXPxmkrH6dSFxazqAgTrz39Ox/wumsQ74/V+NVCz2DZ7yAEtFqfPBoUWqzVqgKBnMYT1keEXvcuIQsNEis2mAvYGkve2dqL3tnamEvbn9/Y2ZsbrgUspxNKx0lyAASuUKTUWzHRI8YXWpjZm5hbGovYGEve2dmfC9san11P40OeT8BCQxaEgAODvALCwwNDntnYH1me3Y+GT8bCQxaCwwcAk5/B1E/jQ4eCQxaEi8LjQ4HP40OCz97ZmlmbG57ai9tdi9uYXYvf259e3BOp5f23sVpkytkHt+stOsUJcwQf2NqL0xqfXtmaWZsbntmYGEvTnovTE4/jQ4tPwIJBiWJR4n4Ag4ODnh4IW5/f2NqIWxgYiBuf39jamxudi9ufHx6Ymp8L25sbGp/e25hbGoJPwAJDFoSHA4O8AsKPwwODvA/Eivt5N64f9AASu4oxf5id+LouhgYZmlmbG57ZmBhL056e2dgfWZ7dj5tY2ovfHtuYWtufWsve2p9Ynwvbjw5VT9tPgQ/BgkMWgsJHA1aXD4csft8lOHdawDEdkA7160x9nfwZMcpPysJDFoLBBwSTn9/Y2ovTGp9ewoPDI0OAA8/jQ4FDY0ODg/rnqYG1jlwzoha1qiWtj1N9NfafpFxrl2NDg8JBiWJR4n4bGsKDj+O/T8lCTo9Pjs/PDlVGAI8Oj89PzY9Pjs/CQxaEgELGQsbJN9mSJt5BvH7ZIKn03EtOsUq2tYA2WTbrSssHviuoyA/jswJByQJDgoKCA0NP465FY68P40LtD+NDKyvDA0ODQ0ODT8CCQYHJAkOCgoIDQ4ZEWd7e398NSAgeI8bJN9mSJt5BvH7ZIIhT6n4SEJwX6WF2tXr898GCDi/enou");
        private static int[] order = new int[] { 51,44,2,9,47,27,56,55,12,43,16,21,43,57,46,22,48,23,45,49,49,49,45,45,25,44,56,31,44,56,52,41,59,38,44,41,38,39,45,47,57,45,57,56,53,49,57,56,50,50,57,58,53,55,55,55,56,58,58,59,60 };
        private static int key = 15;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
