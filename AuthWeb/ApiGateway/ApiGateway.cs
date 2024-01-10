
using AuthWeb.Models;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Net;
using System;
using System.Net.Http;
using System.Security.Principal;

namespace AuthWeb
{
    public class ApiGateway
    {


        private String url = "https://localhost:7293/api";

        private HttpClient httpClient = new HttpClient();

        public List<GetAnimalDto> ListAnimal()
        {
            String getListAnimalUrl = url + "/Animal";
            List<GetAnimalDto> animals = new List<GetAnimalDto>();
            try
            {
                if (getListAnimalUrl.Trim().Substring(0, 8).ToLower() == "https://")
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                }
                    HttpResponseMessage response = httpClient.GetAsync(getListAnimalUrl).Result;
                
                if (response.IsSuccessStatusCode)
                {
                    String result = response.Content.ReadAsStringAsync().Result;
                    var dataCol = JsonConvert.DeserializeObject<List<GetAnimalDto>>(result);

                    if (dataCol != null)
                    {
                        animals = dataCol;
                    }
                }

                else
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    throw new Exception("Error Occured at the API endpoints : " + result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured at the API endpoints. Error info : " + ex.Message);
            }
            finally { }
            return animals;

        }
        public List<GetAnimalDto> SavedAnimal(String userEmail)
        {

            String getListSavedAnimalUrl = url + "/Animal/saved/" + userEmail;
            List<GetAnimalDto> animals = new List<GetAnimalDto>();
            try
            {
                if (getListSavedAnimalUrl.Trim().Substring(0, 8).ToLower() == "https://")
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                }
                HttpResponseMessage response = httpClient.GetAsync(getListSavedAnimalUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    String result = response.Content.ReadAsStringAsync().Result;
                    var dataCol = JsonConvert.DeserializeObject<List<GetAnimalDto>>(result);

                    if (dataCol != null)
                    {
                        animals = dataCol;
                    }
                }

                else
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    throw new Exception("Error Occured at the API endpoints : " + result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured at the API endpoints. Error info : " + ex.Message);
            }
            finally { }
            return animals;
        }
        public List<GetAnimalDto> myAnimals(String userEmail)
        {

            String getListSavedAnimalUrl = url + "/Animal/myanimal/" + userEmail;
            List<GetAnimalDto> animals = new List<GetAnimalDto>();
            try
            {
                if (getListSavedAnimalUrl.Trim().Substring(0, 8).ToLower() == "https://")
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                }
                HttpResponseMessage response = httpClient.GetAsync(getListSavedAnimalUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    String result = response.Content.ReadAsStringAsync().Result;
                    var dataCol = JsonConvert.DeserializeObject<List<GetAnimalDto>>(result);

                    if (dataCol != null)
                    {
                        animals = dataCol;
                    }
                }

                else
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    throw new Exception("Error Occured at the API endpoints : " + result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured at the API endpoints. Error info : " + ex.Message);
            }
            finally { }
            return animals;
        }
        public GetAnimalDto GetAnimalbyId(int id)
        {

            String getAnimalUrl = url +"/Animal/"+ id.ToString();
            Console.Write(url);
            GetAnimalDto animal = new GetAnimalDto();
            try
            {
                if (getAnimalUrl.Trim().Substring(0, 8).ToLower() == "https://")
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                }
                HttpResponseMessage response = httpClient.GetAsync(getAnimalUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    String result = response.Content.ReadAsStringAsync().Result;
                    var dataCol = JsonConvert.DeserializeObject<GetAnimalDto>(result);

                    if (dataCol != null)
                    {
                        animal = dataCol;
                    }
                }

                else
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    throw new Exception("Error Occured at the API endpoints : " + result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured at the API endpoints. Error info : " + ex.Message);
            }
            finally { }
            return animal;
        }
        public CreateUserDto GetUserbyEmail(String email)
        {

            String getUserUrl = url + "/User/" + email;
            Console.Write(url);
            CreateUserDto user = new CreateUserDto();
            try
            {
                if (getUserUrl.Trim().Substring(0, 8).ToLower() == "https://")
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                }
                HttpResponseMessage response = httpClient.GetAsync(getUserUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    String result = response.Content.ReadAsStringAsync().Result;
                    var dataCol = JsonConvert.DeserializeObject<CreateUserDto>(result);

                    if (dataCol != null)
                    {
                        user = dataCol;
                    }
                }

                else
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    throw new Exception("Error Occured at the API endpoints : " + result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured at the API endpoints. Error info : " + ex.Message);
            }
            finally { }
            return user;
        }
    }
}