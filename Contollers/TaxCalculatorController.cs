using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

[ApiController]
[Route("api/[controller]")]
public class TaxCalculatorController : ControllerBase
{
    [HttpPost]
    [Route("calculate")]
    public IActionResult Calculate([FromBody] InputModel input)
    {
        if (string.IsNullOrWhiteSpace(input.Text))
        {
            return BadRequest("Input text cannot be empty.");
        }

        try
        {
            // Extract <total>
            var totalMatch = Regex.Match(input.Text, @"<total>(.*?)</total>");
            if (!totalMatch.Success)
            {
                return BadRequest("Missing <total> tag.");
            }
            if (!decimal.TryParse(totalMatch.Groups[1].Value, out var total))
            {
                return BadRequest("Invalid <total> value.");
            }

            // Extract <cost_centre>
            var costCentreMatch = Regex.Match(input.Text, @"<cost_centre>(.*?)</cost_centre>");
            var costCentre = costCentreMatch.Success ? costCentreMatch.Groups[1].Value : "UNKNOWN";

            // Calculate sales tax and total excluding tax
            const decimal taxRate = 0.10m; // Example: 10% tax rate
            var totalExcludingTax = total / (1 + taxRate);
            var salesTax = total - totalExcludingTax;

            // Check for unmatched tags
            if (HasUnmatchedTags(input.Text))
            {
                return BadRequest("Invalid XML: Unmatched opening or closing tags.");
            }

            // Prepare response
            var output = new OutputModel
            {
                CostCentre = costCentre,
                Total = total,
                TotalExcludingTax = totalExcludingTax,
                SalesTax = salesTax
            };

            return Ok(output);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    private bool HasUnmatchedTags(string text)
    {
        var openingTags = Regex.Matches(text, @"<(\w+)>").Select(m => m.Groups[1].Value).ToList();
        var closingTags = Regex.Matches(text, @"</(\w+)>").Select(m => m.Groups[1].Value).ToList();

        foreach (var tag in openingTags)
        {
            if (!closingTags.Contains(tag))
            {
                return true;
            }
            closingTags.Remove(tag);
        }

        return closingTags.Any();
    }
}