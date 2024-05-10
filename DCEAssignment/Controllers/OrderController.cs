using DCEAssignment.Data;
using DCEAssignment.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DCEAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        [HttpGet]
        [Route("GetActiveOrdersByCustomer/{customerId}")]
        public IActionResult GetActiveOrdersByCustomer(int customerId)
        {
            try
            {
                IEnumerable<OrderModel> orders = _orderRepository.GetActiveOrdersByCustomer(customerId);
                if (orders == null || !orders.Any())
                {
                    return NotFound($"No active orders found for customer with UserId {customerId}.");
                }
                return Ok(orders);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (SqlException ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {ex.Message}");
            }
        }
    }
}
