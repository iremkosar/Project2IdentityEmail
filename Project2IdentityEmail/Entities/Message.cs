namespace Project2IdentityEmail.Entities
{
    public class Message
    {
        public int MessageId { get; set; }
        public string ReceiverEmail { get; set; }
        public string SenderEmail {  get; set; }
        public string Subject { get; set; }
        public string MessageDetail { get; set; }
        public DateTime SendDate { get; set; }
        public bool IsStatus { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public bool IsStarred { get; set; }      // yıldızlandı mı?
        public bool IsDraft { get; set; }        // taslak mı?
        public bool IsDeleted { get; set; }      // çöpte mi? (soft delete)
     
    }
}
