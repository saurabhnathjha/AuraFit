namespace AuraFitWebService.Models
{
    public class Choice
    {
        public int Index { get; set; }
        public ChatbotResponseMessage Message { get; set; }
        public string FinishReason { get; set; }
    }
}
