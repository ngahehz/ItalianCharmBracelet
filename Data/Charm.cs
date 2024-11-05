using System;
using System.Collections.Generic;

namespace ItalianCharmBracelet.Data;

public partial class Charm
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? CateId { get; set; }

    public int? Quantity { get; set; }

    public double Price { get; set; }

    public string? Img { get; set; }

    public string Unit { get; set; } = null!;

    public string? Tag { get; set; }

    public double? Discount { get; set; }

    public string? Description { get; set; }

    public string Link { get; set; } = null!;

    public virtual Category? Cate { get; set; }

    public virtual ICollection<PurchaseInvoiceDetail> PurchaseInvoiceDetails { get; set; } = new List<PurchaseInvoiceDetail>();

    public virtual ICollection<SalesInvoiceDetail> SalesInvoiceDetails { get; set; } = new List<SalesInvoiceDetail>();
}
