using System.Text.Json.Serialization;

namespace WebHookDynamics.DTO
{
    public class DynamicsOrderDto
    {
        public int id { get; set; }                   
        public string status { get; set; }
        public string total { get; set; }
        public string currency { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_modified { get; set; }
        public Billing billing { get; set; }
        public List<LineItem> line_items { get; set; }
        public string payment_method { get; set; }
        public string payment_method_title { get; set; }
        public int customer_id { get; set; }
        public string customer_note { get; set; }
    }

    public class Billing
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string address_1 { get; set; }
        public string city { get; set; }
        public string postcode { get; set; }
        public string country { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }

    public class LineItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public string total { get; set; }
        public string sku { get; set; }

        public decimal price { get; set; }  
    }
}