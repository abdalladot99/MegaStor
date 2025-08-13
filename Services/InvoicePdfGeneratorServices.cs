using MegaStor.ViewModel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;

public class InvoicePdfGeneratorServices
{
	public static byte[] Generate(InvoicePdfModel model)
	{
		return Document.Create(container =>
		{
			container.Page(page =>
			{
				page.Margin(40);
				page.Size(PageSizes.A4);
				page.DefaultTextStyle(x => x.FontSize(12));

				// ===== HEADER =====
				page.Header().Column(header =>
				{
					header.Item().Text("MegaStor")
						.FontSize(26).Bold().FontColor(Colors.Blue.Medium);

					header.Item().Text("Order Invoice")
						.FontSize(16).SemiBold();

					header.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
				});

				// ===== CONTENT =====
				page.Content().Column(col =>
				{
					col.Spacing(15);

					// Customer & Order Info
					col.Item().Row(row =>
					{
						// Customer Info
						row.RelativeItem().Background(Colors.Grey.Lighten4).Padding(10).Column(left =>
						{
							left.Spacing(2);
							left.Item().Text("Billed To:").Bold();
							left.Item().Text(model.CustomerName);
							left.Item().Text(model.Address);
							left.Item().Text(model.Email);
							left.Item().Text(model.PhoneNumber);
						});

						// Order Info
						row.RelativeItem().Background(Colors.Grey.Lighten4).Padding(10).Column(right =>
						{
							right.Spacing(2);
							right.Item().Text("Order Date:").Bold();
							right.Item().Text(model.OrderDate);
							right.Item().Text("Order Status:").Bold();
							right.Item().Text(model.OrderStatus);
							right.Item().Text("Shipping Status:").Bold();
							right.Item().Text(model.ShippingStatus);
						});
					});

					// ===== ORDER TABLE =====
					col.Item().Table(table =>
					{
						table.ColumnsDefinition(columns =>
						{
							columns.RelativeColumn(4); // Item
							columns.RelativeColumn(2); // Price
							columns.RelativeColumn(2); // Qty
							columns.RelativeColumn(2); // Total
						});

						// Table Header
						table.Header(header =>
						{
							header.Cell().Element(HeaderCellStyle).Text("Item");
							header.Cell().Element(HeaderCellStyle).Text("Price");
							header.Cell().Element(HeaderCellStyle).Text("Quantity");
							header.Cell().Element(HeaderCellStyle).AlignRight().Text("Total");

							static IContainer HeaderCellStyle(IContainer container)
							{
								return container
									.Background(Colors.Blue.Medium)
									.Padding(5)
									.DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White));
							}
						});

						// Table Rows with Zebra Striping
						int index = 0;
						foreach (var item in model.Items)
						{
							var bgColor = (index % 2 == 0) ? Colors.White : Colors.Grey.Lighten4;

							table.Cell().Element(c => DataCellStyle(c, bgColor)).Text(item.ItemName);
							table.Cell().Element(c => DataCellStyle(c, bgColor)).Text($"${item.UnitPrice:F2}");
							table.Cell().Element(c => DataCellStyle(c, bgColor)).Text(item.Quantity.ToString());
							table.Cell().Element(c => DataCellStyle(c, bgColor)).AlignRight().Text($"${item.TotalPrice:F2}");

							index++;
						}

						static IContainer DataCellStyle(IContainer container, string bgColor)
						{
							return container.Background(bgColor).Padding(5);
						}
					});

					// ===== TOTALS =====
					col.Item().AlignRight().Background(Colors.Grey.Lighten4).Padding(10).Column(totals =>
					{
						totals.Spacing(3);
						totals.Item().Text($"Sub Total: ${model.TotalAmount:F2}").SemiBold();
						totals.Item().Text("Shipping: $0.00");
						totals.Item().Text("Tax: $0.00");
						totals.Item().Text("Discount: $0.00");
						totals.Item().Text($"Total: ${model.TotalAmount:F2}")
							.FontSize(14).Bold().FontColor(Colors.Green.Darken2);
					});
				});

				// ===== FOOTER =====
				page.Footer().Column(footer =>
				{
					footer.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
					footer.Item().AlignCenter().Text($"Generated on {DateTime.Now:dd MMM yyyy HH:mm}");
				});
			});
		}).GeneratePdf();
	}
}
