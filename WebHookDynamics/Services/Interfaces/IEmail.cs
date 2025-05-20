namespace WebHookDynamics.Services.Interfaces;

public interface IEmail
{
    Task SendOrderConfirmationEmail(string customerEmail, string customerFullName, string itemListBilling, string subtotalText, string address);
    Task SendCustomerCreationEmail(string customerEmail, string customerFullName);
}