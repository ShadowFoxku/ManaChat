namespace ManaChat.API.Models
{
    public class MessageResponse(string message)
    {
        public static MessageResponse Standard(string message)
        {
            return new MessageResponse(message);
        }

        public string Message = message;
    }
}
