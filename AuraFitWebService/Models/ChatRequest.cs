namespace AuraFitWebService.Models
{
    public class ChatRequest
    {
        public List<Message> Messages { get; set; }
        public string Model { get; set; } = "gpt-4o-mini";
    }
}
