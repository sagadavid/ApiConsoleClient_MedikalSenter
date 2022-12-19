using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using ApiConsoleClient_MedikalSenter.Models;
using System.Linq;

namespace ApiConsoleClient_MedikalSenter
{
    internal class Program
    {
        static void Main(string[] args)
        {
           
            RunAsync().Wait();//genereate RunAsync method below
            Console.ReadLine();
        }

        static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                //prepare client
                client.BaseAddress = new Uri("http://localhost:53207");//check your api's uri
                client.DefaultRequestHeaders
                    .Accept
                    .Clear();
                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));//we want data in json format

                //prepare response (lets say grabber) object
                HttpResponseMessage response;
                //get the patients
                try
                {
                    response =await client.GetAsync("api/patients");//go get from client
                    response.EnsureSuccessStatusCode();//if cant, throw exception
                    List<Patient> patients = await response.Content.ReadAsAsync<List<Patient>>();//gets list of patients
                    
                    foreach (Patient p in patients)
                    {
                        //this is a console app.. no way to show it 
                        Console.WriteLine(
                            "ID: {0}\tFullName:{1}\tExpYrVisits:{2}\tDoctor:{3}"
                            ,p.ID,p.FullName,p.ExpYrVisits,p.Doctor.FullName);
                    }
                }
                catch (Exception)
                {

                    Console.WriteLine("couldnt get patient list from the api server");
                    return;//stop execution
                }

                //CRUD
                //NEW PATIENT
                var comingPatient = new Patient()
                { FirstName="Ezo", LastName="Gelin", OHIP="1234539676", ExpYrVisits=12, DoctorID=5};
                Console.WriteLine("\r\n coming patient's:\r\nId:{0}\tFullname:{1}\tExpYrVisits:{2}\tDoctorID:{3}",
                    comingPatient.ID,comingPatient.FullName, comingPatient.ExpYrVisits, comingPatient.DoctorID);

                try
                {
                    //HTTP POST
                    response = await client.PostAsJsonAsync("api/patients", comingPatient);
                    response.EnsureSuccessStatusCode(); // Throw exception if not success code
                   
                    //Get the new ID
                    Uri patientUrl = response.Headers.Location;
                    int newID = Convert.ToInt32(patientUrl.ToString().Split('/').Last());

                    Console.WriteLine("SUCCESSFULLY Uploaded:\r\nID:{0}\tFullname:{1}\tExpYrVisits:{2}\tDoctorID:{3}",
                        comingPatient.ID, comingPatient.FullName, comingPatient.ExpYrVisits, comingPatient.DoctorID);

                    //HTTP PUT
                    //We have to get a copy of the coming patient we added in order to have the correct row version
                    response = await client.GetAsync("api/patients" + "/" + newID);
                    response.EnsureSuccessStatusCode(); // Throw exception if not success code

                    Patient addedPatient = await response.Content.ReadAsAsync<Patient>();//gets newPatient's content
                    Console.WriteLine("Record From Database:\r\nID:{0}\tFullname:{1}\tExpYrVisits:{2}\tDoctor:{3}",
                        addedPatient.ID, addedPatient.FullName, addedPatient.ExpYrVisits,addedPatient.Doctor.FullName);

                    // UPDATE ExpYsVisits
                    addedPatient.ExpYrVisits = 9;
                    //addedPatient.DoctorID = 3;//not sure.. didnt work?

                    response = await client.PutAsJsonAsync(patientUrl, addedPatient);
                    response.EnsureSuccessStatusCode(); // Throw exception if not success code
                    
                    Console.WriteLine("Updated/Edited:\r\nID:{0}\t:{1}\tExpYrVisits:{2}\tDoctor:{3}", 
                        addedPatient.ID, addedPatient.FullName, addedPatient.ExpYrVisits, addedPatient.Doctor.FullName);

                    // HTTP DELETE
                    response = await client.DeleteAsync(patientUrl);
                    response.EnsureSuccessStatusCode(); // Throw exception if not success code
                    
                    Console.WriteLine ("DELETED!\r\n - GET THE LIST AGAIN TO see that IT IS GONE\r\n");

                    //get list and show again
                    response = await client.GetAsync("api/patients");
                    if (response.IsSuccessStatusCode)
                    {
                        List<Patient>patients = await response.Content.ReadAsAsync<List<Patient>>();
                        foreach (Patient p in patients)
                        {
                            //this is a console app.. no way to show it 
                            Console.WriteLine(
                                "ID:{0}\tFullName:{1}\tExpYrVisits:{2}\tDoctor:{3}"
                                ,p.ID, p.FullName, p.ExpYrVisits, p.Doctor.FullName);
                        }
                    }
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("error during CRUD test on console");
                    return;
                }
            }
        }
    }
}
