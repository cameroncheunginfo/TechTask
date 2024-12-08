using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data.Common;

namespace DataAccess;

public class OrderDbContext : DbContext
{
    private readonly DbConnection _connection;

    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
        _connection = RelationalOptionsExtension.Extract(options).Connection!;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
                                     {
                                         entity.ToTable("Addresses");

                                         entity.HasKey(e => e.AddressId);

                                         entity.Property(e => e.AddressId)
                                               .ValueGeneratedOnAdd()
                                               .UseIdentityColumn(1);

                                         entity.Property(e => e.Created)
                                               .HasColumnType("datetime")
                                               .IsRequired();

                                         entity.Property(e => e.Hash)
                                               .IsRequired()
                                               .HasMaxLength(50)
                                               .IsUnicode(false);
                                         entity.Property(e => e.LineOne)
                                               .IsRequired()
                                               .HasMaxLength(255)
                                               .IsUnicode(false);
                                         entity.Property(e => e.LineTwo)
                                               .IsRequired()
                                               .HasMaxLength(255)
                                               .IsUnicode(false);
                                         entity.Property(e => e.LineThree)
                                               .IsRequired()
                                               .HasMaxLength(255)
                                               .IsUnicode(false);
                                         entity.Property(e => e.PostCode)
                                               .IsRequired()
                                               .HasMaxLength(10)
                                               .IsUnicode(false);
                                     });

        modelBuilder.Entity<Customer>(entity =>
                                      {
                                          entity.ToTable("Customers");

                                          entity.HasKey(e => e.CustomerId);

                                          entity.Property(e => e.CustomerId)
                                                .ValueGeneratedOnAdd()
                                                .UseIdentityColumn(1);

                                          entity.Property(e => e.Created).HasColumnType("datetime");
                                          entity.Property(e => e.Email)
                                                .IsRequired()
                                                .HasMaxLength(255)
                                                .IsUnicode(false);
                                          entity.Property(e => e.ExternalId)
                                                .IsRequired()
                                                .HasMaxLength(50)
                                                .IsUnicode(false);
                                          entity.Property(e => e.Name)
                                                .IsRequired()
                                                .HasMaxLength(255)
                                                .IsUnicode(false);
                                          entity.Property(e => e.PhoneNumber)
                                                .IsRequired()
                                                .HasMaxLength(12)
                                                .IsUnicode(false);
                                      });

        modelBuilder.Entity<Order>(entity =>
                                   {
                                       entity.ToTable("Orders");

                                       entity.HasKey(e => e.OrderId);

                                       entity.Property(e => e.OrderId)
                                             .ValueGeneratedOnAdd()
                                             .UseIdentityColumn(1);

                                       entity.Property(e => e.Created)
                                             .IsRequired()
                                             .HasColumnType("datetime");
                                       entity.Property(e => e.LastModified)
                                             .IsRequired()
                                             .HasColumnType("datetime");

                                       entity.Property(e => e.OrderNumber)
                                             .IsRequired()
                                             .HasMaxLength(50)
                                             .IsUnicode(false);

                                       entity.Property(e => e.TotalPrice)
                                             .IsRequired()
                                             .HasColumnType("money");

                                       entity.HasOne(d => d.BillingAddress).WithMany(p => p.BillingOrders)
                                             .HasForeignKey(d => d.BillingAddressId)
                                             .OnDelete(DeleteBehavior.ClientSetNull)
                                             .HasConstraintName("FK_Orders_Addresses_Billing");

                                       entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                                             .HasForeignKey(d => d.CustomerId)
                                             .OnDelete(DeleteBehavior.ClientSetNull)
                                             .HasConstraintName("FK_Orders_Customers");

                                       entity.HasOne(d => d.ShippingAddress).WithMany(p => p.ShippingOrders)
                                             .HasForeignKey(d => d.ShippingAddressId)
                                             .OnDelete(DeleteBehavior.ClientSetNull)
                                             .HasConstraintName("FK_Orders_Addresses_Shipping");
                                   });

        modelBuilder.Entity<OrderItem>(entity =>
                                       {
                                           entity.ToTable("OrderItems");

                                           entity.HasKey(e => e.OrderItemId);

                                           entity.Property(e => e.OrderItemId)
                                                 .ValueGeneratedOnAdd()
                                                 .UseIdentityColumn(1);

                                           entity.Property(e => e.Quantity)
                                                 .IsRequired();

                                           entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                                                 .HasForeignKey(d => d.OrderId)
                                                 .OnDelete(DeleteBehavior.ClientSetNull)
                                                 .HasConstraintName("FK_OrderItems_Orders");

                                           entity.HasOne(d => d.Variant).WithMany(p => p.OrderItems)
                                                 .HasForeignKey(d => d.VariantId)
                                                 .OnDelete(DeleteBehavior.ClientSetNull)
                                                 .HasConstraintName("FK_OrderItems_Variants");
                                       });

        modelBuilder.Entity<OutboxMessage>(entity =>
                                           {
                                               entity.ToTable("Outbox");

                                               entity.HasKey(e => e.OutboxMessageId);

                                               entity.Property(e => e.OutboxMessageId)
                                                     .ValueGeneratedOnAdd()
                                                     .UseIdentityColumn(1);

                                               entity.Property(e => e.Created)
                                                     .IsRequired()
                                                     .HasColumnType("datetime");

                                               entity.Property(e => e.MessageId)
                                                     .IsRequired()
                                                     .HasMaxLength(50)
                                                     .IsUnicode(false);

                                               entity.Property(e => e.Payload)
                                                     .IsRequired()
                                                     .IsUnicode(false);
                                           });

        modelBuilder.Entity<Product>(entity =>
                                     {
                                         entity.ToTable("Products");

                                         entity.HasKey(e => e.ProductId);

                                         entity.Property(e => e.ProductId)
                                               .ValueGeneratedOnAdd()
                                               .UseIdentityColumn(1);

                                         entity.Property(e => e.ExternalId)
                                               .IsRequired()
                                               .HasMaxLength(50)
                                               .IsUnicode(false);

                                         entity.Property(e => e.ImageUrl)
                                               .IsRequired()
                                               .HasMaxLength(1000)
                                               .IsUnicode(false);

                                         entity.Property(e => e.Name)
                                               .IsRequired()
                                               .HasMaxLength(255)
                                               .IsUnicode(false);
                                     });

        modelBuilder.Entity<Variant>(entity =>
                                     {
                                         entity.ToTable("Variants");

                                         entity.HasKey(e => e.VariantId);

                                         entity.Property(e => e.VariantId)
                                               .ValueGeneratedOnAdd()
                                               .UseIdentityColumn(1);

                                         entity.Property(e => e.ExternalId)
                                               .IsRequired()
                                               .HasMaxLength(50)
                                               .IsUnicode(false);

                                         entity.Property(e => e.Name)
                                               .IsRequired()
                                               .HasMaxLength(50)
                                               .IsUnicode(false);

                                         entity.Property(e => e.Price)
                                               .IsRequired()
                                               .HasColumnType("money");

                                         entity.Property(e => e.Sku)
                                               .IsRequired()
                                               .HasMaxLength(50)
                                               .IsUnicode(false);

                                         entity.HasOne(d => d.Product).WithMany(p => p.Variants)
                                               .HasForeignKey(d => d.ProductId)
                                               .OnDelete(DeleteBehavior.ClientSetNull)
                                               .HasConstraintName("FK_Variants_Products");
                                     });

        var veronaBath = new Product("Verona Freestanding Modern Bath",
                                     "https://images.victorianplumbing.co.uk/products/1650-x-750-luxury-modern-double-ended-curved-freestanding-bath/mainimages/bfre011_l2.png");
        veronaBath.ProductId = 1000;
        var kentBath = new Product("Kent Single Ended Bath",
                                     "https://images.victorianplumbing.co.uk/products/kent-single-ended-bath/mainimages/kentsingleendedbathl.jpg");
        kentBath.ProductId = 1100;
        var milanBath = new Product("Milan Shower Bath - 1700mm L Shaped with Screen + Panel",
                                     "https://images.victorianplumbing.co.uk/products/l-shaped-shower-bath-1700mm-inc-fixed-screen-acrylic-panel/mainimages/l1700fsn-l2.jpg");
        milanBath.ProductId = 1200;

        var metroToilet = new Product("Metro Rimless Close Coupled Modern Toilet + Soft Close Seat",
                                      "https://images.victorianplumbing.co.uk/products/metro-modern-close-coupled-toilet-with-soft-close-seat/mainimages/metcc_l2.png");
        metroToilet.ProductId = 2000;
        var arezzoToilet = new Product("Arezzo BTW Close Coupled Toilet + Soft Close Seat",
                                       "https://images.victorianplumbing.co.uk/products/arezzo-btw-close-coupled-toilet-with-soft-close-seat/mainimages/arzbtwcc_lrg3.png");
        arezzoToilet.ProductId = 2100;
        var novaToilet = new Product("Nova Rimless Round Back To Wall Pan with Soft Close Seat",
                                     "https://images.victorianplumbing.co.uk/products/nova-rimless-back-to-wall-pan-with-soft-close-seat/mainimages/nvbtw01_l3.png");
        novaToilet.ProductId = 2200;

        var arezzoBasin = new Product("Arezzo 400 x 220mm Curved Wall Hung 1TH Cloakroom Basin",
                                      "https://images.victorianplumbing.co.uk/products/arezzo-400-x-215mm-curved-wall-hung-1th-cloakroom-basin/variants/az78204l/mainimages/az78204lnew.jpg");
        arezzoBasin.ProductId = 3000;
        var cuboBasin = new Product("Cubo Basin + Full Pedestal (520mm Wide - 1 Tap Hole)",
                                       "https://images.victorianplumbing.co.uk/products/cubo-basin-full-pedestal-520mm-wide-1-tap-hole/mainimages/cbfp_n_l.jpg");
        cuboBasin.ProductId = 3100;
        var cascaBasin = new Product("Casca Oval Counter Top Basin 0TH - 410 x 330mm",
                                     "https://images.victorianplumbing.co.uk/products/casca-oval-counter-top-basin-0th-410-x-340mm/mainimages/cascaovalcountertopbasinl.jpg");
        cascaBasin.ProductId = 3200;

        var milanTaps = new Product("Milan Modern Mono Basin Mixer and Bath Filler - Chrome",
                                    "https://images.victorianplumbing.co.uk/products/milan-modern-mono-basin-mixer-and-bath-filler-chrome/mainimages/milanmodernmonobasinmixerandbathfillerlarge.jpg");
        milanTaps.ProductId = 4000;
        var cruzeTaps = new Product("Cruze Modern Bathroom Tap Package (Bath + Basin Tap)",
                                    "https://images.victorianplumbing.co.uk/products/cruze-modern-tap-package-bath-basin-tap/mainimages/cruzemoderntappackagel.jpg");
        cruzeTaps.ProductId = 4100;
        var summitTaps = new Product("Summit Modern Tap Package (Bath + Basin Tap)",
                                    "https://images.victorianplumbing.co.uk/products/summit-modern-tap-package-bath-basin-tap/mainimages/summit_modern_tap_package_lrg.jpg");
        summitTaps.ProductId = 4200;

        modelBuilder.Entity<Product>()
                    .HasData(veronaBath, kentBath, milanBath,
                             metroToilet, arezzoToilet, novaToilet,
                             arezzoBasin, cuboBasin, cascaBasin,
                             milanTaps, cruzeTaps, summitTaps);

        var veronaBFRE011 = new Variant(veronaBath, "1400 x 750 x 570mm", "BFRE011", 599.95m);
        veronaBFRE011.VariantId = 100;
        var veronaBFRE012 = new Variant(veronaBath, "1550 x 750 x 570mm", "BFRE012", 619.95m);
        veronaBFRE012.VariantId = 200;
        var veronaBFRE013 = new Variant(veronaBath, "1650 x 750 x 570mm", "BFRE013", 629.95m);
        veronaBFRE013.VariantId = 300;
        var veronaBFRE013B = new Variant(veronaBath,"1650 x 850 x 570mm", "BFRE013B", 639.95m);
        veronaBFRE013B.VariantId = 400;
        var veronaBFRE014 = new Variant(veronaBath, "1800 x 750 x 570mm", "BFRE014", 649.95m);
        veronaBFRE014.VariantId = 500;

        var kentKB127 = new Variant(kentBath, "1200 x 700mm", "KB127", 159.95m);
        kentKB127.VariantId = 600;
        var kentKB137 = new Variant(kentBath, "1300 x 700mm", "KB137", 159.95m);
        kentKB137.VariantId = 700;
        var kentKB147 = new Variant(kentBath, "1400 x 700mm", "KB147", 159.95m);
        kentKB147.VariantId = 800;
        var kentKB157 = new Variant(kentBath, "1500 x 700mm", "KB157", 159.95m);
        kentKB157.VariantId = 900;

        var milanL1700FSLH = new Variant(milanBath, "Left Hand Option", "L1700FSLH", 319.95m);
        milanL1700FSLH.VariantId = 1000;
        var milanL1700FSRH = new Variant(milanBath, "Right Hand Option", "L1700FSRH", 319.95m);
        milanL1700FSRH.VariantId = 1100;

        var metroMETCC = new Variant(metroToilet, "METCC", "METCC", 189.95m);
        metroMETCC.VariantId = 1200;
        var arezzoARZBTWCC = new Variant(arezzoToilet, "ARZBTWCC", "ARZBTWCC", 199.95m);
        arezzoARZBTWCC.VariantId = 1300;
        var novaNVBTW01 = new Variant(novaToilet, "NVBTW01", "NVBTW01", 179.95m);
        novaNVBTW01.VariantId = 1400;

        var arezzoAZ78204L = new Variant(arezzoBasin, "Left Hand Tap Hole", "AZ78204L", 69.95m);
        arezzoAZ78204L.VariantId = 1500;
        var arezzoAZ78204R = new Variant(arezzoBasin, "Right Hand Tap Hole", "AZ78204R", 69.95m);
        arezzoAZ78204R.VariantId = 1600;
        var cuboCBFP = new Variant(cuboBasin, "CBFP", "CBFP", 79.95m);
        cuboCBFP.VariantId = 1700;
        var cascaVES978 = new Variant(cascaBasin, "VES978", "VES978", 89.95m);
        cascaVES978.VariantId = 1800;

        var milanMIL001MIL008 = new Variant(milanTaps, "MIL001-MIL008", "MIL001-MIL008", 139.95m);
        milanMIL001MIL008.VariantId = 1900;
        var cruzeCRZPK = new Variant(cruzeTaps, "CRZ-PK", "CRZ-PK", 124.95m);
        cruzeCRZPK.VariantId = 2000;
        var summitSUMPK = new Variant(summitTaps, "SUMPK", "SUMPK", 124.95m);
        summitSUMPK.VariantId = 2100;

        modelBuilder.Entity<Variant>()
                    .HasData(veronaBFRE011, veronaBFRE012, veronaBFRE013, veronaBFRE013B, veronaBFRE014,
                             kentKB127, kentKB137, kentKB147, kentKB157,
                             milanL1700FSLH, milanL1700FSRH,
                             metroMETCC, arezzoARZBTWCC, novaNVBTW01,
                             arezzoAZ78204L, arezzoAZ78204R, cuboCBFP, cascaVES978,
                             milanMIL001MIL008, cruzeCRZPK, summitSUMPK);

    }

    public override void Dispose()
    {
        _connection.Dispose();

        base.Dispose();
    }
}

