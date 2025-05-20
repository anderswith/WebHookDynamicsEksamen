using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebHookDynamics.DTO;
using WebHookDynamics.DynamicsFacade.Interfaces;

namespace WebHookDynamics.Controller;
[ApiController]
[Route("api/[controller]")]
public class WebHookController : ControllerBase
{
    private readonly IDynamicsWebhookLogic _dynamicsWebhookLogic;
    public WebHookController(IDynamicsWebhookLogic dynamicsWebhookLogic)
    {
        _dynamicsWebhookLogic = dynamicsWebhookLogic;
    }
    [HttpPost("AddOrder")]
    public async Task<IActionResult> AddOrder()
    {
        Console.WriteLine("Entered C# AddOrder Controller");

        try
        {
            Request.EnableBuffering(); // Allows the request body stream to be read multiple times if needed
            using var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            Request.Body.Position = 0;
            
            Console.WriteLine("Received JSON: " + body);
            
            var dto = JsonSerializer.Deserialize<DynamicsOrderDto>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            

            if (dto == null)
                return BadRequest("Invalid order data.");
            
            Console.WriteLine($"Order ID: {dto.id}, Items count: {dto.line_items?.Count}");

            await _dynamicsWebhookLogic.ProcessOrder(dto);

            return Ok("Order processed successfully.");
        }
        catch (JsonException jsonEx)
        {
            return BadRequest($"JSON deserialization error: {jsonEx.Message}");
        }
        catch (HttpRequestException httpEx)
        {
            return StatusCode(502, $"External API call failed: {httpEx.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("CreateCustomer")]
    public async Task<IActionResult> CreateCustomer()
    {
        Console.WriteLine("Entered C# AddOrder Controller");

        try
        {
            Request.EnableBuffering(); // Allows the request body stream to be read multiple times if needed
            using var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            Request.Body.Position = 0;
            
            Console.WriteLine("Received JSON: " + body);
            
            var dto = JsonSerializer.Deserialize<CreateCustomerDto>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            

            if (dto == null)
                return BadRequest("Invalid order data.");
            
            
            Console.WriteLine($"attempting to enter CreateCustomer logic");
            await _dynamicsWebhookLogic.CreateCustomer(dto);

            return Ok("Order processed successfully.");
        }
        catch (JsonException jsonEx)
        {
            return BadRequest($"JSON deserialization error: {jsonEx.Message}");
        }
        catch (HttpRequestException httpEx)
        {
            return StatusCode(502, $"External API call failed: {httpEx.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }

    } 

    
}