using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.JSInterop.Infrastructure;
using WebHookDynamics.DTO;
using WebHookDynamics.DynamicsFacade.Interfaces;
using WebHookDynamics.Services.Interfaces;

namespace WebHookDynamics.DynamicsFacade;

public class DynamicsWebhookLogic: IDynamicsWebhookLogic
{
    private readonly HttpClient _httpClient;
    private readonly IEmail _email;
    
    public DynamicsWebhookLogic(IHttpClientFactory httpClientFactory, IEmail email)
    {
        var handler = new HttpClientHandler()
        {
            UseDefaultCredentials = true
        };
        _httpClient = new HttpClient(handler);
        _email = email;
    }

    public async Task ProcessOrder(DynamicsOrderDto dto)
    {
            Console.WriteLine("Entered ProcessOrder");

        if (dto == null) throw new ArgumentException("Invalid order data.");
        var CustomerNo = dto.customer_id;
        var firstName = dto.billing.first_name;
        var lastName = dto.billing.last_name;
        var customerFullName = $"{firstName} {lastName}";
        var Address = dto.billing.address_1;
        var City = dto.billing.city;
        var Postcode = dto.billing.postcode;
        var Country = dto.billing.country;
        var Email = dto.billing.email;
        var Phone = dto.billing.phone;
        var Currency = dto.currency;
        var OrderDate = dto.date_created.ToString("yyyy-MM-dd");
        var PaymentMethod = dto.payment_method;

        var payload = new
        {
            customerNo = CustomerNo,
            customerName = customerFullName,
            address = Address,
            city = City,
            postcode = Postcode,
            country = Country,
            email = Email,
            phone = Phone,
            currency = Currency,
            orderDate = OrderDate,
            paymentMethod = PaymentMethod,
            salesLines = dto.line_items.Select(item => new
            {
                itemNo = item.sku,
                quantity = item.quantity,
                description = item.name,
                unitPrice = item.price
                
            }).ToList()
        };

        var payloadWrapper = new
        {
            itemsJson = JsonSerializer.Serialize(payload)
        };

        var jsonPayload = JsonSerializer.Serialize(payloadWrapper);
            
        Console.WriteLine($"Sending JSON payload: {jsonPayload}");

        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var url = "http://localhost:7048/BC170/ODataV4/SalesOrderReceiver_CreateSalesOrder?company=CRONUS%20UK%20Ltd.";
        var response = await _httpClient.PostAsync(url, content);

        var responseText = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed calling API: {response.StatusCode} - {responseText}");
            throw new HttpRequestException($"Dynamics API call failed: {response.StatusCode} - {responseText}");
        }

        Console.WriteLine($"Dynamics API responded: {response.StatusCode} - {responseText}");

        // Optionally send email (same as before)
        var address = $"{dto.billing.address_1}, {dto.billing.city}, {dto.billing.postcode}, {dto.billing.country}";
        
        string customerEmail = dto.billing.email;
        var itemStringBuilder = new StringBuilder();
        foreach (var item in dto.line_items)
        {
            itemStringBuilder.AppendLine($@"
            <tr>
                <td>{item.name}</td>
                <td>{item.quantity}</td>
                <td>{item.total} KR</td>
            </tr>");
        }

        string itemListBilling = itemStringBuilder.ToString();
        string subtotalText = dto.total.ToString();
        Console.WriteLine("klar til at sende mail");
        await _email.SendOrderConfirmationEmail(
            customerEmail,
            customerFullName,
            itemListBilling,
            subtotalText,
            address
        );
    }


    public async Task CreateCustomer(CreateCustomerDto dto)
    {
        Console.WriteLine("Entered CreateCustomer");
        if (dto == null) throw new ArgumentException("Invalid order data.");
        var Id = dto.id;
        var Email = dto.email;
        var FirstName = dto.first_name;
        var LastName = dto.last_name;
        var customerFullName = $"{FirstName} {LastName}";


        var payload = new
        {
            customerNo = Id,
            customerName = customerFullName,
            email = Email,

        };
        var payloadWrapper = new
        {
            itemsJson = JsonSerializer.Serialize(payload)
        };
        var jsonPayload = JsonSerializer.Serialize(payloadWrapper);
        
        Console.WriteLine($"Sending JSON payload: {jsonPayload}");

        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        var url = "http://localhost:7048/BC170/ODataV4/SalesOrderReceiver_CreateCustomer?company=CRONUS%20UK%20Ltd.";
        
        var response = await _httpClient.PostAsync(url, content);

        var responseText = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed calling API: {response.StatusCode} - {responseText}");
            throw new HttpRequestException($"Dynamics API call failed: {response.StatusCode} - {responseText}");
        }

        Console.WriteLine($"Dynamics API responded: {response.StatusCode} - {responseText}");
        _email.SendCustomerCreationEmail(Email, customerFullName);


    }
}