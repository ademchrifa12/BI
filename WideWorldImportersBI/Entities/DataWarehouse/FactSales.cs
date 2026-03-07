using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WideWorldImportersBI.Entities.DataWarehouse;

[Table("Fact_Ventes")]
public class FactVentes
{
    [Key]
    [Column("Vente_SK")]
    public int VenteSK { get; set; }

    [Column("Date_SK")]
    public int? DateSK { get; set; }

    [Column("Client_SK")]
    public int? ClientSK { get; set; }

    [Column("Produit_SK")]
    public int? ProduitSK { get; set; }

    [Column("Employe_SK")]
    public int? EmployeSK { get; set; }

    [Column("InvoiceLineID")]
    public int? InvoiceLineId { get; set; }

    [Column("Quantite")]
    public int? Quantite { get; set; }

    [Column("PrixUnitaire")]
    public decimal? PrixUnitaire { get; set; }

    [Column("MontantHT")]
    public decimal? MontantHT { get; set; }

    [Column("Taxe")]
    public decimal? Taxe { get; set; }

    [Column("Profit")]
    public decimal? Profit { get; set; }

    [ForeignKey("ClientSK")]
    public virtual DimClient? Client { get; set; }

    [ForeignKey("ProduitSK")]
    public virtual DimProduit? Produit { get; set; }

    [ForeignKey("EmployeSK")]
    public virtual DimEmploye? Employe { get; set; }
}
