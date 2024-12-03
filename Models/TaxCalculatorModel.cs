public class InputModel
{
    public string? Text { get; set; }
}

public class OutputModel
{
    public string? CostCentre { get; set; }
    public decimal Total { get; set; }
    public decimal TotalExcludingTax { get; set; }
    public decimal SalesTax { get; set; }
}