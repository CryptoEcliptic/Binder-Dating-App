using System;

namespace BinderApp.API.DTOs
{
    public class MessageForCreationDto
    {
        public MessageForCreationDto()
        {
            MessageSent = DateTime.Now;
        }

        public int SenderId { get; set; }

        public int RecipientId { get; set; }

        public string Content { get; set; }

        public DateTime MessageSent { get; set; }
    }
}