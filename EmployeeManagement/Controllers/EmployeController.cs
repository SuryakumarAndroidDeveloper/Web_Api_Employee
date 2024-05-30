using EmployeeManagement.DataAcessLayer;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagement.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class EmployeController : ControllerBase
    {

        public readonly IConfiguration _configuration;
        
        private readonly EmployeDAL _employeeDAL;

        public EmployeController(IConfiguration configuration)
        {
            _configuration = configuration;
            
            _employeeDAL = new EmployeDAL(configuration);

        }


        //This code for getting all the employees to form a api/Employe/GetAllEmployees
        [HttpGet]
        [Route("GetAllEmployees")]
        public String GetEmployee()
        {
            List<EmployeModel> activeEmployees = _employeeDAL.GetActiveEmployees();

            if (activeEmployees.Count > 0)
            {

               return JsonConvert.SerializeObject(activeEmployees);

            }else
            {
                return "No active employees found.";
            }

        }

        [HttpPost]
        [Route("InsertEmployee")]
        public IActionResult InsertEmployee(EmployeModel employee)
        {
            bool isInserted = _employeeDAL.InsertEmployee(employee);

            if (isInserted)
            {
                return Ok("Employee inserted successfully.");
            }
            else
            {
                return BadRequest("Failed to insert employee.");
            }
        }

        [HttpGet]
        [Route("GetEmployeeById")]
        public async Task<IActionResult> GetById(int id)
        {
            EmployeModel employeModel = await _employeeDAL.GetEmployeeByIdAsync(id);
            if (employeModel == null)
            {
                return NotFound();
            }

            return Ok(employeModel);
        }
    



        [HttpPost]
        [Route("UpdateEmployeeById")]
        public async Task<IActionResult> UpdateEmployeeById( [FromBody] EmployeModel employee)
        {
           if (employee.Id == null )
            {
                return BadRequest("Invalid employee data.");
            }

            try
            {
                bool isUpdated = await _employeeDAL.UpdateEmployeeByIdAsync( employee);

                if (isUpdated)
                {
                    return Ok("Employee updated successfully.");
                }
                else
                {
                    return NotFound("Employee not found.");
                }
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, "Internal server error");
            }
        }





        [HttpPost]
        [Route("DeleteEmployeeById")]
        public async Task<IActionResult> DeactivateEmployee(int id)
        {
            try
            {
                bool isDeactivated = await _employeeDAL.DeactivateEmployeeByIdAsync(id);

                if (isDeactivated)
                {
                    return Ok("Employee deactivated successfully.");
                }
                else
                {
                    return NotFound("Employee not found.");
                }
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal server error");
            }
            }






        }
}





    