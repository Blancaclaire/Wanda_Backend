namespace Models
{
    public class TransactionSplit
    {
        public int Split_id { get; set; }
        public int User_id { get; set; }
        public int Transaction_id { get; set; }
        public double Amount_assigned { get; set; }
        public string Status { get; set; } 
        
        public DateTime? Paid_at { get; set; } 

        public TransactionSplit() { }

        public TransactionSplit(int user_id, int transaction_id, double amount_assigned, string status, DateTime? paid_at = null)
        {
            User_id = user_id;
            Transaction_id = transaction_id;
            Amount_assigned = amount_assigned;
            Status = status;
            Paid_at = paid_at;
        }
    }
}