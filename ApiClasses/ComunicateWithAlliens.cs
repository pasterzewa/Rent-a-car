using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Rent_a_Car.Messenge.FromAlien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace Rent_a_Car.ApiClasses
{
    public static class ComunicateWithAlliens
    {
        public static async Task<string> CallToAllien(string request, object body = null)
        {
            

            // discover endpoints from metadata
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync("https://indentitymanager.snet.com.pl");
            if (disco.IsError)
            {
                return (disco.Error);
                
            }

            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "team3c",
                ClientSecret = "c88b805d-1ee9-42a5-8594-33af4bfd33b4",

                Scope = "MiNI.RentApp.API"
            });

            if (tokenResponse.IsError)
            {
                return (tokenResponse.Error);
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            if (body == null)
            {
                var response = await apiClient.GetAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return (response.StatusCode).ToString();
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return (content);
                }
            }
            else
            {
                var objAsJson = JsonConvert.SerializeObject(body);
                var res = await apiClient.PostAsync(request,
                   new StringContent( JsonConvert.SerializeObject(body),Encoding.UTF8, "application/json" ));

                try
                {
                    res.EnsureSuccessStatusCode();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                //var response = await apiClient.PostAsJsonAsync(request, objAsJson);


                // Get the response
                return await res.Content.ReadAsStringAsync(); ;
            }
        }
    }
}
