using snaps.wms.api.pda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace snaps.wms.api.pda.Utils {
    public static class TokenReader {

        public static TokenReaderModel GetToken(this ClaimsIdentity identity) {
            try {
                var claims = identity.Claims;
                if(claims.Count() > 0) {
                    var token = new TokenReaderModel();
                    token.Uname = claims.Where(p => p.Type.EndsWith("sid")).FirstOrDefault()?.Value;
                    token.Name = claims.Where(p => p.Type.EndsWith("name")).FirstOrDefault()?.Value;
                    token.Email = claims.Where(p => p.Type.EndsWith("email")).FirstOrDefault()?.Value;
                    token.HasCode = claims.Where(p => p.Type.EndsWith("hash")).FirstOrDefault()?.Value;
                    token.Images = claims.Where(p => p.Type.EndsWith("userdata")).FirstOrDefault()?.Value;
                    return token;
                }
                return null;
            } catch(Exception) {
                return null;
            }
        }
        public static TokenReaderModel Decode(this IIdentity identity) {
            try {
                var claims = (identity as ClaimsIdentity).Claims;
                if(claims.Count() > 0) {
                    var token = new TokenReaderModel();
                    token.Uname = claims.Where(p => p.Type.EndsWith("sid")).FirstOrDefault()?.Value;
                    token.Name = claims.Where(p => p.Type.EndsWith("name")).FirstOrDefault()?.Value;
                    token.Email = claims.Where(p => p.Type.EndsWith("email")).FirstOrDefault()?.Value;
                    token.HasCode = claims.Where(p => p.Type.EndsWith("hash")).FirstOrDefault()?.Value;
                    token.Images = claims.Where(p => p.Type.EndsWith("userdata")).FirstOrDefault()?.Value;
                    return token;
                }
                return null;
            } catch(Exception) {
                return null;
            }
        }
    }
}
