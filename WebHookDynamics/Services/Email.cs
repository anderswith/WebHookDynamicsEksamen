using MailKit.Net.Smtp;
using MimeKit;
using WebHookDynamics.Services.Interfaces;

namespace WebHookDynamics.Services;

public class Email : IEmail
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPass;
    public Email(string smtpServer, int smtpPort, string smtpUser, string smtpPass)
    {
        _smtpServer = smtpServer;
        _smtpPort = smtpPort;
        _smtpUser = smtpUser;
        _smtpPass = smtpPass;
    }

    public async Task SendOrderConfirmationEmail(string customerEmail, string customerFullName, string itemListBilling, string subtotalText, string address)
    {
        Console.WriteLine("Entered SendOrderConfirmationEmail");
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Woocommerce e-shop", _smtpUser));
        email.To.Add(new MailboxAddress(customerFullName, customerEmail));
        email.Subject = "Your Order Confirmation";
    
        email.Body = new TextPart("html")
        {
            Text = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset=""UTF-8"">
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        color: #333;
                        padding: 20px;
                    }}
                    .container {{
                        background-color: #ffffff;
                        padding: 20px;
                        border-radius: 6px;
                        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                        max-width: 600px;
                        margin: 0 auto;
                    }}
                    h2 {{
                        color: #2c3e50;
                        border-bottom: 1px solid #ddd;
                        padding-bottom: 10px;
                    }}
                    table {{
                        width: 100%;
                        border-collapse: collapse;
                        margin-top: 20px;
                    }}
                    th, td {{
                        text-align: left;
                        padding: 8px;
                        border: 1px solid #ddd;
                    }}
                    th {{
                        background-color: #f0f0f0;
                    }}
                    .subtotal {{
                        font-weight: bold;
                        margin-top: 20px;
                    }}
                    .footer {{
                        margin-top: 30px;
                        font-size: 0.9em;
                        color: #777;
                        text-align: center;
                    }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <h2>Order Confirmation</h2>
                    <p>Hello {customerFullName},</p>
                    <p>Thank you for your order! Below are your billing details:</p>

                    <h3>Billing Address</h3>
                    <p>{address.Replace("\r\n", "<br>").Replace("\n", "<br>")}</p>

                    <h3>Items Ordered</h3>
                    <table>
                        <thead>
                            <tr>
                                <th>Item Description</th>
                                <th>Quantity</th>
                                <th>Price</th>
                            </tr>
                        </thead>
                        <tbody>
                            {itemListBilling}
                        </tbody>
                    </table>

                    <p class=""subtotal"">Subtotal: {subtotalText} KR</p>

                    <p>We appreciate your business and look forward to serving you again!</p>

                    <div class=""footer"">
                        <p>Best regards,<br>
                        Woocommerce Webshop<br>
                        <a href=""mailto:support@webshop.com"">support@webshop.com</a><br>
                        +45 1234 567890</p>
                    </div>
                </div>
            </body>
            </html>"
        };
        
        Console.WriteLine("forsøger at sende mail");
        using var smtp = new SmtpClient();
          
        try
        {
            await smtp.ConnectAsync(_smtpServer, _smtpPort, true); 
            await smtp.AuthenticateAsync(_smtpUser, _smtpPass);
            Console.WriteLine("sender mail");
            await smtp.SendAsync(email);
            Console.WriteLine("mail sendt");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email send failed: {ex.Message}");
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
    public async Task SendCustomerCreationEmail(string customerEmail, string customerFullName)
    {
        Console.WriteLine("Entered SendCustomerCreationConfirmationEmail");
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Woocommerce e-shop", _smtpUser));
        email.To.Add(new MailboxAddress(customerFullName, customerEmail));
        email.Subject = "Your Account has been created";
    
        email.Body = new TextPart("html")
        {
            Text = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset=""UTF-8"">
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        color: #333;
                        padding: 20px;
                    }}
                    .container {{
                        background-color: #ffffff;
                        padding: 30px 40px;
                        border-radius: 8px;
                        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                        max-width: 600px;
                        margin: 0 auto;
                    }}
                    h2 {{
                        color: #2c3e50;
                        border-bottom: 2px solid #3498db;
                        padding-bottom: 10px;
                        margin-bottom: 20px;
                        font-weight: 700;
                    }}
                    p {{
                        font-size: 16px;
                        line-height: 1.5;
                    }}
                    .footer {{
                        margin-top: 30px;
                        font-size: 0.9em;
                        color: #777;
                        text-align: center;
                        border-top: 1px solid #ddd;
                        padding-top: 15px;
                    }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <h2>Welcome, {customerFullName}!</h2>
                    <p>Thank you for creating an account with us. We're excited to have you as a customer.</p>
                    <p>If you have any questions or need assistance, please don't hesitate to reach out.</p>
                    <div class=""footer"">
                        <p>Best regards,<br>
                        Woocommerce Webshop Team<br>
                        <a href=""mailto:support@webshop.com"">support@webshop.com</a><br>
                        +45 1234 567890</p>
                    </div>
                </div>
            </body>
            </html>"
        };
        
        Console.WriteLine("forsøger at sende mail");
        using var smtp = new SmtpClient();
          
        try
        {
            await smtp.ConnectAsync(_smtpServer, _smtpPort, true); 
            await smtp.AuthenticateAsync(_smtpUser, _smtpPass);
            Console.WriteLine("sender mail");
            await smtp.SendAsync(email);
            Console.WriteLine("mail sendt");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email send failed: {ex.Message}");
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}