namespace Project2IdentityEmail.Dtos
{
    public class DashboardDto
    {
        // KPI
        public int TotalUsers { get; set; }
        public int TotalMessages { get; set; }     // Inbox total gibi kullanıyoruz
        public int InboxCount { get; set; }
        public int SendboxCount { get; set; }
        public string? SenderImageUrl { get; set; }
        public string MessageDetail { get; set; }
        public int UnreadMessages { get; set; }    // IsStatus == false
        public int ReadMessages { get; set; }      // IsStatus == true
        public int TotalCategories { get; set; }

        // Profil
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public string About { get; set; }

        // Listeler
        public List<LastMessageDto> LastMessages { get; set; } = new();
        public List<CategoryStatDto> CategoryStats { get; set; } = new();
    }

    public class LastMessageDto
    {
        public int MessageId { get; set; }
        public string SenderEmail { get; set; }
        public string ReceiverEmail { get; set; }
        public string Subject { get; set; }
        public DateTime SendDate { get; set; }
        public bool IsStatus { get; set; }
        public string MessageDetail { get; set; } // true=okundu
        public string? SenderImageUrl { get; set; }
    }

    public class CategoryStatDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int Count { get; set; }
    }
}
