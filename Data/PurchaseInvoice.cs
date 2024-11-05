using System;
using System.Collections.Generic;

namespace ItalianCharmBracelet.Data;

public partial class PurchaseInvoice
{
    public string Id { get; set; } = null!;

    public DateOnly? Date { get; set; }

    public double? TotalPayment { get; set; }

    public string? Note { get; set; }

    public string? State { get; set; }

    public virtual ICollection<PurchaseInvoiceDetail> PurchaseInvoiceDetails { get; set; } = new List<PurchaseInvoiceDetail>();
}
