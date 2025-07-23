using System;
using System.Collections.Generic;

namespace HikariDataAccess.Entities;

public partial class Question
{
    public int Id { get; set; }

    public string QuestionText { get; set; } = null!;

    public string? OptionA { get; set; }

    public string? OptionB { get; set; }

    public string? OptionC { get; set; }

    public string? OptionD { get; set; }

    public string? CorrectOption { get; set; }

    public decimal? Mark { get; set; }

    public string EntityType { get; set; } = null!;

    public int EntityId { get; set; }
}
