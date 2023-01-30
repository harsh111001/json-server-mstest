using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace EmployeePayrollRestSharpMstest
{
    [TestClass]
    public class UnitTest1
    {
        RestClient client;
        [TestInitialize]
        public void setup()
        {
            client = new RestClient("http://localhost:5000");
        }
        public RestResponse GetAllEmployee()
        {
            RestResponse response;
            //arrange
            RestRequest request=new RestRequest("/Employees",Method.Get);
            response = client.ExecuteAsync(request).Result;
            return response;
        }
        [TestMethod]
        public void CallinggetAPIToReturnEmployees()
        {
            RestResponse response=GetAllEmployee();

            var jsonObj = JsonConvert.DeserializeObject<List<EmployeeModel>>(response.Content);

            Assert.AreEqual(4,jsonObj.Count);
            foreach(var emp in jsonObj)
            {
                Console.WriteLine($"id: {emp.id}, name: {emp.Name}, salary: {emp.Salary}");
            }
            Assert.AreEqual(HttpStatusCode.OK,response.StatusCode);
        }
        public RestResponse AddToJson(EmployeeModel model)
        {
            RestResponse response;
            RestRequest request = new RestRequest("/Employees", Method.Post);
            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(new { Name = model.Name, Salary = model.Salary });
            response= client.ExecuteAsync(request).Result;
            return response;
        }
        [TestMethod]
        public void CallingPostAPIToAddEmployees()
        {
            EmployeeModel employee= new EmployeeModel();
            employee.Name = "Shashank";
            //pass as strings even if its int
            employee.Salary = "100000";
            RestResponse response= AddToJson(employee);
            var emp= JsonConvert.DeserializeObject<EmployeeModel>(response.Content);
            Console.WriteLine($"id: {emp.id}, name: {emp.Name}, salary: {emp.Salary}");
            Assert.AreEqual("Shashank",emp.Name);
            Assert.AreEqual("100000", emp.Salary);
            Assert.AreEqual(HttpStatusCode.Created,response.StatusCode);
        }
        [TestMethod]
        public void TestMethodForUpdateEmployee()
        {
            RestRequest request = new RestRequest("/Employees/5",Method.Put);
            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(new { Name = "Utkarsh", Salary = "85000" });
            RestResponse restResponse= client.ExecuteAsync(request).Result;
            var emp = JsonConvert.DeserializeObject<EmployeeModel>(restResponse.Content);
            Assert.AreEqual(HttpStatusCode.OK,restResponse.StatusCode);
            Console.WriteLine($"id: {emp.id}, name: {emp.Name}, salary: {emp.Salary}");
        }
        [TestMethod]
        public void TestMethodForDeleteEmployee()
        {
            RestRequest request = new RestRequest("/Employees/1",Method.Delete);
            RestResponse restResponse = client.ExecuteAsync(request).Result;
            RestResponse response = GetAllEmployee();
            var jsonObj = JsonConvert.DeserializeObject<List<EmployeeModel>>(response.Content);

            //Assert.AreEqual(4, jsonObj.Count);
            foreach (var emp in jsonObj)
            {
                Console.WriteLine($"id: {emp.id}, name: {emp.Name}, salary: {emp.Salary}");
            }
            Assert.AreEqual(HttpStatusCode.OK, restResponse.StatusCode);
        }
    }
}