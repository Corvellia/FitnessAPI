using System;
using System.Collections.Generic;

namespace FitnessAPI.Models;

public partial class WeightLog
{
    public int LogId { get; set; }

    public double Weight { get; set; }

    public string LogDate { get; set; } = null!;

    public string? Units { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
