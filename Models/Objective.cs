namespace Models;

public class Objective
{
    public int Objective_id { get; set; }
    // Esto es (FK) referenciando a la tabla ACCOUNTS
    public int Account_id { get; set; }
    public string Name { get; set; }
    public double Target_amount { get; set; }
    public double Current_save { get; set; } = 0;
    public DateTime Deadline { get; set; }
    public string Objective_picture_url { get; set; }


    public Objective()
    {

    }

    public Objective(int account_id, string name, double target_amount, double current_save, DateTime deadline, string objective_picture_url)
    {
        Account_id = account_id;
        Name = name;
        Target_amount = target_amount;
        Current_save = current_save;
        Deadline = deadline;
        Objective_picture_url = objective_picture_url;

    }

}