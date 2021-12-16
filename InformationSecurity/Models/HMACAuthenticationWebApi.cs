using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace InformationSecurity.Models
{
    public class HMACAuthenticationAttribute : Attribute, IAsyncAuthorizationFilter
    {
        // this Dictionary is for Public and Private key but i should get it from th DB
        private static Dictionary<string, string> allowedApps = new Dictionary<string, string>();
        // the maximum time of the request
        private readonly UInt64 requestMaxAgeInSeconds = 300; //Means 5 min
        // type of authentication
        private readonly string authenticationScheme = "hmacauth";

        public HMACAuthenticationAttribute()
        {
            if (allowedApps.Count == 0)
            {
                allowedApps.Add("65d3a4f0-0239-404c-8394-21b94ff50604", "WLUEWeL3so2hdHhHM5ZYnvzsOUBzSGH4+T3EgrQ91KI=");
            }
        }

        // we implement the logic for validating the signature of the incoming request.
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            HttpRequestMessageFeature hreqmf = new HttpRequestMessageFeature(context.HttpContext);
            HttpRequestMessage req = hreqmf.HttpRequestMessage;

            // we need to make sure that the Authorization Header is present in the request and it should not be empty.
            // We also need to make sure that it contains the “hmacauth” scheme.

            if (req.Headers.Authorization != null && authenticationScheme.Equals(req.Headers.Authorization.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                //we need to read the Authorization Header value from the request and then split its content based on
                //the delimiter we have specified earlier in client i.e. using a colon “:”.

                var rawAuthzHeader = req.Headers.Authorization.Parameter;
                var autherizationHeaderArray = GetAutherizationHeaderValues(rawAuthzHeader);
                if (autherizationHeaderArray != null)
                {
                    var APPId = autherizationHeaderArray[0];
                    var incomingBase64Signature = autherizationHeaderArray[1];
                    var nonce = autherizationHeaderArray[2];
                    var requestTimeStamp = autherizationHeaderArray[3];

                    //calling the “IsValidRequest” method where we implement all the logic of reconstructing the HMAC Signature
                    //from the request data and then comparing this signature with the incoming signature.

                    var isValid = IsValidRequest(req, APPId, incomingBase64Signature, nonce, requestTimeStamp);

                    if (isValid.Result)
                    {
                        //var currentPrincipal = new GenericPrincipal(new GenericIdentity(APPId), null);
                        //context.HttpContext.Response.Headers = currentPrincipal.;
                        context.Result = new JsonResult(new
                        {
                            Message = "Token Validation Has Succeed."
                        })
                        {
                            StatusCode = StatusCodes.Status200OK
                        };
                    }
                    else
                    {
                        context.Result = new JsonResult(new
                        {
                            Message = "Token Validation Has Failed. Request Access Denied"
                        })
                        {
                            StatusCode = StatusCodes.Status401Unauthorized
                        };
                    }
                }
                else
                {
                    context.Result = new JsonResult(new
                    {
                        Message = "Token Validation Has Failed. Request Access Denied"
                    })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }
            }
            else
            {
                context.Result = new JsonResult(new
                {
                    Message = "Token Validation Has Failed. Request Access Denied"
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
            //The else statments 
            //In case the Authorization Header is not present or if the Authorization Header does not the “hmacauth” scheme
            //or if the “IsValidRequest” method returns false, then we will consider this request as an unauthorized request and returns 401 unauthorized.

            return Task.FromResult(0);
        }

        private string[] GetAutherizationHeaderValues(string rawAuthzHeader)
        {

            var credArray = rawAuthzHeader.Split(':');

            if (credArray.Length == 4)
            {
                return credArray;
            }
            else
            {
                return null;
            }
        }


        //The custom implementation logic of reconstructing the signature and comparing it with the signature received from the client is done here
        private async Task<bool> IsValidRequest(HttpRequestMessage req, string APPId, string incomingBase64Signature, string nonce, string requestTimeStamp)
        {
            string requestContentBase64String = "";
            string requestUri = HttpUtility.UrlEncode(req.RequestUri.AbsoluteUri.ToLower());
            string requestHttpMethod = req.Method.Method;
            //we check whether the received Public Shared APP ID is registered in our system or not,
            //if it is not registered in our system, then we simply return false from the isValidRequest method

            if (!allowedApps.ContainsKey(APPId))
            {
                return false;
            }

            var sharedKey = allowedApps[APPId];
            //then we need to check whether the request received is a replay request.
            if (isReplayRequest(nonce, requestTimeStamp))
            {
                return false;
            }

            byte[] hash = await ComputeHash(req.Content);

            if (hash != null)
            {
                requestContentBase64String = Convert.ToBase64String(hash);
            }

            string data = String.Format("{0}{1}{2}{3}{4}{5}", APPId, requestHttpMethod, requestUri, requestTimeStamp, nonce, requestContentBase64String);

            var secretKeyBytes = Convert.FromBase64String(sharedKey);

            byte[] signature = Encoding.UTF8.GetBytes(data);

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyBytes))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);

                return (incomingBase64Signature.Equals(Convert.ToBase64String(signatureBytes), StringComparison.Ordinal));
            }

        }

        //The replay request means we need to check if the nonce received from the client is used before.
        //Currently, we are storing all the nonce received from the client in Cache Memory for 5 minutes only.
        private bool isReplayRequest(string nonce, string requestTimeStamp)
        {
            if (System.Runtime.Caching.MemoryCache.Default.Contains(nonce))
            {
                return true;
            }

            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan currentTs = DateTime.UtcNow - epochStart;

            var serverTotalSeconds = Convert.ToUInt64(currentTs.TotalSeconds);
            var requestTotalSeconds = Convert.ToUInt64(requestTimeStamp);

            if ((serverTotalSeconds - requestTotalSeconds) > requestMaxAgeInSeconds)
            {
                return true;
            }
            //If it is not exist in the Cache, then add it.
            System.Runtime.Caching.MemoryCache.Default.Add(nonce, requestTimeStamp, DateTimeOffset.UtcNow.AddSeconds(requestMaxAgeInSeconds));

            return false;
        }
        //we need to compute the MD5 hash of the body content if it is available (POST or PUT methods),
        //then we built the signature raw data by concatenating the parameters (APP ID, request HTTP method, request URI, request timestamp, nonce, requestContentBase64String) without any delimiters.
        private static async Task<byte[]> ComputeHash(HttpContent httpContent)
        {
            using (MD5 md5 = MD5.Create()) // here we have to use AES 
            {
                byte[] hash = null;
                var content = await httpContent.ReadAsByteArrayAsync();
                if (content.Length != 0)
                {
                    hash = md5.ComputeHash(content);
                }
                return hash;
            }
            // It is mandatory that both the parties (Client and Server) need to use the same data format to produce the same signature;
            // the data eventually will get hashed using the same hashing algorithm and API Key used by the client.
            // If the incoming client signature equals the signature generated on the server then we will consider this request as authentic and will process it.
        }

    }
}

// Let us understand the replay request with an example. For example,
// if the client generates a nonce lets say “abcd1234” and send it with the request to the server.
// Then the server will check whether this nonce “abcd1234” is used before.
// If not then the server will store the nonce value in Cache Memory for the next 5 minutes.
// So any request coming from the client with the same nonce value i.e. “abcd1234” during the 5 minutes time interval will be considered as a replay attack or replay request.
// if the same nonce “abcd1234” is used after 5 minutes time interval then this is fine and the request is not considered as a replay request.

// But there might a situation where let’s say an evil person might try to re-post the same request using the same nonce after the 5 minutes window,
// so in situation like this the request timestamp becomes handy,
// the implementation here is comparing the current server UNIX time with the request UNIX time received from the client,
// if the request timestamp is older than 5 minutes then it is rejected by the server
// and the evil person has no possibility to fake the request timestamp and send fresher one
// because we have already included the request timestamp in the signature raw data,
// so any changes on it will result in a new signature and it will not match the client incoming signature.