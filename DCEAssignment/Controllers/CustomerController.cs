using DCEAssignment.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;
using System.Data;
using System.Data.SqlClient;

namespace DCEAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CustomerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        [Route("GetCustomer")]
        public IActionResult GetCustomers()
        {
            List<CustomerModel> Lst = new List<CustomerModel>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            if (string.IsNullOrEmpty(connectionString))
            {
                return BadRequest("Connection string 'DefaultConnection' is null or empty.");
            }

            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT *  FROM Customer", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    CustomerModel obj = new CustomerModel();
                    obj.UserId = int.Parse(dataTable.Rows[i]["UserId"].ToString());
                    obj.Username = dataTable.Rows[i]["Username"].ToString();
                    obj.Email = dataTable.Rows[i]["Email"].ToString();
                    obj.FirstName = dataTable.Rows[i]["FirstName"].ToString();
                    obj.LastName = dataTable.Rows[i]["LastName"].ToString();
                    obj.CreatedOn = DateTime.Parse(dataTable.Rows[i]["CreatedOn"].ToString());
                    obj.IsActive = bool.Parse(dataTable.Rows[i]["IsActive"].ToString());

                    Lst.Add(obj);
                }
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {ex.Message}");
            }

            return Ok(Lst);
        }

        [HttpPost]
        [Route("CreateCustomer")]
        public IActionResult CreateCustomer([FromBody] CustomerModel newCustomer)
        {
            // Validate the incoming customer data
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            if (string.IsNullOrEmpty(connectionString))
            {
                return BadRequest("Connection string 'DefaultConnection' is null or empty.");
            }

            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Customer (Username, Email, FirstName, LastName, CreatedOn, IsActive) VALUES (@Username, @Email, @FirstName, @LastName, @CreatedOn, @IsActive); SELECT SCOPE_IDENTITY();", conn);

                cmd.Parameters.AddWithValue("@Username", newCustomer.Username);
                cmd.Parameters.AddWithValue("@Email", newCustomer.Email);
                cmd.Parameters.AddWithValue("@FirstName", newCustomer.FirstName);
                cmd.Parameters.AddWithValue("@LastName", newCustomer.LastName);
                cmd.Parameters.AddWithValue("@CreatedOn", newCustomer.CreatedOn);
                cmd.Parameters.AddWithValue("@IsActive", newCustomer.IsActive);

                // Execute the command and retrieve the newly inserted CustomerId
                int customerId = Convert.ToInt32(cmd.ExecuteScalar());

                // Update the newCustomer object with the generated CustomerId
                newCustomer.UserId = customerId;

                return Ok(newCustomer);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("UpdateCustomer/{userId}")]
        public IActionResult UpdateCustomer(int userId, [FromBody] CustomerModel updatedCustomer)
        {
            // Validate the incoming customer data
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            if (string.IsNullOrEmpty(connectionString))
            {
                return BadRequest("Connection string 'DefaultConnection' is null or empty.");
            }

            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                // Check if the customer with the provided UserId exists
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Customer WHERE UserId = @UserId", conn);
                checkCmd.Parameters.AddWithValue("@UserId", userId);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count == 0)
                {
                    return NotFound($"Customer with UserId {userId} not found.");
                }

                // Update the customer details
                SqlCommand updateCmd = new SqlCommand("UPDATE Customer SET Username = @Username, Email = @Email, FirstName = @FirstName, LastName = @LastName, IsActive = @IsActive WHERE UserId = @UserId", conn);
                updateCmd.Parameters.AddWithValue("@UserId", userId);
                updateCmd.Parameters.AddWithValue("@Username", updatedCustomer.Username);
                updateCmd.Parameters.AddWithValue("@Email", updatedCustomer.Email);
                updateCmd.Parameters.AddWithValue("@FirstName", updatedCustomer.FirstName);
                updateCmd.Parameters.AddWithValue("@LastName", updatedCustomer.LastName);
                updateCmd.Parameters.AddWithValue("@IsActive", updatedCustomer.IsActive);
                updateCmd.ExecuteNonQuery();

                return Ok($"Customer with UserId {userId} updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteCustomer/{userId}")]
        public IActionResult DeleteCustomer(int userId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            if (string.IsNullOrEmpty(connectionString))
            {
                return BadRequest("Connection string 'DefaultConnection' is null or empty.");
            }

            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                // Check if the customer with the provided UserId exists
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Customer WHERE UserId = @UserId", conn);
                checkCmd.Parameters.AddWithValue("@UserId", userId);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count == 0)
                {
                    return NotFound($"Customer with UserId {userId} not found.");
                }

                // Delete the customer
                SqlCommand deleteCmd = new SqlCommand("DELETE FROM Customer WHERE UserId = @UserId", conn);
                deleteCmd.Parameters.AddWithValue("@UserId", userId);
                deleteCmd.ExecuteNonQuery();

                return Ok($"Customer with UserId {userId} deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {ex.Message}");
            }
        }


    }
}
