using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models
{
    public class Constants
    {
        public static readonly string ApprovedColor = "green";
        public static readonly string PendingColor = "yellow";
        public static readonly string DeniedColor = "red";

        public static readonly int ApprovedValue = 1;
        public static readonly int PendingValue = 2;
        public static readonly int DeniedValue = 0;

        public static readonly string Issuer = "Vera Water and Power";
        public static readonly string Audience = "https://bermuda.verawp.local";

        public static readonly string PublicKey = @"AAAAB3NzaC1yc2EAAAABJQAAAQEAo7XbTCUcAOWkhyLsqLkJQyo/zAi++n6dk3KZ
l5kM2FAinck1+Y3a/NYSIh14CEXqJmSxcUCS9alDlSpMbZNtAnLYhcnc2tYcXyX3
COFUF6qGXSb6gyhHtECsN4pWpaKjFxqOc6SDWERTBf8jcBzfonNFgVlm7PNTc6iV
tUui3mhy8aoJJ5SHRNiadAdRN6PBZRjv7BUMGhtN0bQzhPBrYkm2Fz0eYmQj6tFD
XK/2s1lgsV1obGjbiWsA2I/iMTBIzLNqZQUpSxaHAQLYqi1Iq00Ff+iqVVoyxoiY
URsZSFv5HAj6PlB4oCgpubgGKDPT4AITcklVBckTElUTtPxGRw==";
        public static readonly string PrivateKey = @"aOZ25O62jYglhGjYOYegkChKMP+26C10MDh9lSlmkgYQl01+Xi2iERlsIt9RS3hI
E8kSpQukmptyX0fXeSO03m2yC6FQSTQODx5EhjJ28TU69bJ1txLNULZyXdzMTBov
xcfb09EYHHIrFm5giVwPCIWHhotAg9/jOxFFHsEdDSU4TDuXC4LR5jjZS3iigoYi
U4oJ0DWGVKxUBMUsjFEVgPN+23mXjroUjvctWFr0N1F4hXHZXv10aZnk952c0loJ
wGaoFWu3bpLT2KdWjfaAyhj4h98z43YwBQwQFcoQN7oYv71XQeGg3W+bj3lgJ74L
l8LwakQzDu6j4AWMafEpQvuZO7zpV1LDczZzMtxlYja3RFwAXPE4I/TtXJ7bZLaz
dy5VXl3gR6+LYI//bzDnSSejOb6JiT6X07nn4xKogAUaBNDrCtNWGNWdj/iT5ZTc
XAsw4++7ceIAoQHpC/la5H2jSuMr3J2dsxz/ol1U8vRqXJzny8Z+RSXwZYmDNOSN
ShcStstMxHm259d2xaQtikwjx2HpaYbSbKMDMfd3ifT6d0KRDjvM8MdXv6yz/s0j
+l+SclXnTgpVRMQnQ/Y/SniX+0+SKIf/jqZz/VOtSZb+s0+IvScMW0yNu+e9SP8r
GpkwJB9oS1Uui/ulBZ6v2qFMuevi/ESoofEQzboBhwfotN1JK1SdFOt+y9aT0KmF
LA/YnXUt6RnkgAZoygojCYBx/s32900gTECFceD+bwmK0ScYTVkHDDw11CrGWguH
YA6IXZXIaa3g5Uw4LdQQujZ5MBJFXWl+yloeKDzgE1hWpvb1MiqgZpz1sEpeMh/p
OktV5BSiEs0qKleG02qXP1MaPHRDO7alx3mlrXLFBD9VkyXSnLO6smUVCQyufxmq";
    }
}