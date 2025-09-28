using System;
using System.Collections.Generic;

namespace FitnessAPI.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<WeightLog> WeightLogs { get; set; } = new List<WeightLog>();
}
