using DCEAssignment.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace DCEAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public OrderController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetActiveOrdersByCustomer/{customerId}")]
        public IActionResult GetActiveOrdersByCustomer(int customerId)
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

                SqlCommand cmd = new SqlCommand("GetActiveOrdersByCustomer", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerId", customerId);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                List<OrderModel> orders = new List<OrderModel>();

                foreach (DataRow row in dataTable.Rows)
                {
                    OrderModel order = new OrderModel
                    {
                        OrderId = Convert.ToInt32(row["OrderId"]),
                        UnitPrice = Convert.ToDecimal(row["UnitPrice"]),
                        ProductName = row["ProductName"]?.ToString() ?? "",
                        Username = row["Username"]?.ToString() ?? "",
                        OrderedOn = Convert.ToDateTime(row["OrderedOn"]),
                        OrderType = Convert.ToByte(row["OrderType"])
                    };

                    orders.Add(order);
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {ex.Message}");
            }
        }
    }
}
