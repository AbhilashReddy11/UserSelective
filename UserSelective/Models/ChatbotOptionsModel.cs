using System.ComponentModel.DataAnnotations;

namespace UserSelective.Models
{
    public class ChatbotOptionsModel
    {
        [Required]
        public string SelectedOption { get; set; }
        [Required]
        public string InputPrompt { get; set; }
    }
}
