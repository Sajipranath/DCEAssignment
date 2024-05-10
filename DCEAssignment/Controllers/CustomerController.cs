using DCEAssignment.Data;
using DCEAssignment.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SqlClient;

namespace DCEAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        [Route("GetCustomer")]
        public IActionResult GetCustomers()
        {
            try
            {
                var customers = _customerRepository.GetCustomers();
                return Ok(customers);
            }
            catch (SqlException ex) // Handle SqlException
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {ex.Message}");
            }
            catch (ArgumentException ex) // Handle ArgumentException
            {
                return BadRequest($"Invalid argument: {ex.Message}");
            }
            catch (Exception ex) // Handle other Exceptions
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("CreateCustomer")]
        public IActionResult CreateCustomer([FromBody] CustomerModel newCustomer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                int customerId = _customerRepository.CreateCustomer(newCustomer);
                newCustomer.UserId = customerId;
                return Ok(newCustomer);
            }
            catch (SqlException ex) // Handle SqlException 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {ex.Message}");
            }
            catch (ArgumentException ex) // Handle ArgumentException
            {
                return BadRequest($"Invalid argument: {ex.Message}");
            }
            catch (Exception ex) // Handle other Exceptions
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("UpdateCustomer/{userId}")]
        public IActionResult UpdateCustomer(int userId, [FromBody] CustomerModel updatedCustomer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _customerRepository.UpdateCustomer(userId, updatedCustomer);
                return Ok($"Customer with UserId {userId} updated successfully.");
            }
            catch (SqlException ex) // Handle SqlException 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {ex.Message}");
            }
            catch (ArgumentException ex) // Handle ArgumentException
            {
                return BadRequest($"Invalid argument: {ex.Message}");
            }
            catch (Exception ex) // Handle other Exceptions
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteCustomer/{userId}")]
        public IActionResult DeleteCustomer(int userId)
        {
            try
            {
                _customerRepository.DeleteCustomer(userId);
                return Ok($"Customer with UserId {userId} deleted successfully.");
            }
            catch (SqlException ex) // Handle SqlException
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {ex.Message}");
            }
            catch (ArgumentException ex) // Handle ArgumentException
            {
                return BadRequest($"Invalid argument: {ex.Message}");
            }
            catch (Exception ex) // Handle other Exceptions
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }
}
