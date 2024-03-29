USE [test_bigfile]
GO
/****** Object:  UserDefinedTableType [dbo].[ProductsVariantsTable]    Script Date: 7/29/2023 9:50:17 AM ******/
CREATE TYPE [dbo].[ProductsVariantsTable] AS TABLE(
	[id] [bigint] NULL,
	[product_id] [bigint] NULL,
	[variant_name] [varchar](1000) NULL,
	[reference_id] [nvarchar](100) NULL,
	[variant_reference_id] [nvarchar](100) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[ProductTable]    Script Date: 7/29/2023 9:50:18 AM ******/
CREATE TYPE [dbo].[ProductTable] AS TABLE(
	[product_name] [varchar](100) NULL,
	[product_code] [varchar](100) NULL,
	[base_price] [numeric](18, 0) NULL,
	[reference_id] [nvarchar](100) NULL
)
GO
/****** Object:  Table [dbo].[donation]    Script Date: 7/29/2023 9:50:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[donation](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[parent_id] [int] NULL,
	[elec_code] [nvarchar](50) NULL,
	[senator_id] [nvarchar](50) NULL,
	[amount] [int] NULL,
	[representative_name] [nvarchar](250) NULL,
	[postal_code] [nchar](10) NULL,
	[address] [nvarchar](max) NULL,
	[occupation_id] [int] NULL,
	[receipt_no] [nvarchar](max) NULL,
 CONSTRAINT [PK_donation] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[donation_1]    Script Date: 7/29/2023 9:50:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[donation_1](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[parent_id] [int] NULL,
	[elec_code] [nvarchar](50) NULL,
	[senator_id] [nvarchar](50) NULL,
	[amount] [int] NULL,
	[representative_name] [nvarchar](250) NULL,
	[postal_code] [nchar](10) NULL,
	[address] [nvarchar](max) NULL,
	[occupation_id] [int] NULL,
	[receipt_no] [nvarchar](max) NULL,
 CONSTRAINT [PK_donation_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[products]    Script Date: 7/29/2023 9:50:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[products](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[product_name] [varchar](1000) NULL,
	[product_code] [varchar](100) NULL,
	[base_price] [numeric](18, 0) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[products_variants]    Script Date: 7/29/2023 9:50:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[products_variants](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[product_id] [bigint] NULL,
	[variant_name] [varchar](1000) NULL,
 CONSTRAINT [PK_products_variants] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[products_variants]  WITH CHECK ADD  CONSTRAINT [FK_products_variants_products_variants] FOREIGN KEY([id])
REFERENCES [dbo].[products_variants] ([id])
GO
ALTER TABLE [dbo].[products_variants] CHECK CONSTRAINT [FK_products_variants_products_variants]
GO
/****** Object:  StoredProcedure [dbo].[MergeTableDonation]    Script Date: 7/29/2023 9:50:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[MergeTableDonation]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET IDENTITY_INSERT donation ON

merge donation as Target
using donation_1 as Source
on (Target.id = Source.id)

when Matched  then
Update SET TARGET.parent_id = Source.parent_id,
		   Target.elec_code = Source.elec_code,
		   Target.senator_id = Source.senator_id,
		   Target.amount = Source.amount,
		   Target.representative_name = Source.representative_name,
		   Target.postal_code = Source.postal_code,
		   Target.address = Source.address,
		   Target.occupation_id = Source.occupation_id,
		   Target.receipt_no = Source.receipt_no
when not matched by target then
insert (id, parent_id, elec_code, senator_id, amount, representative_name, postal_code, address, occupation_id, receipt_no  )
values (Source.id, Source.parent_id, Source.elec_code, Source.senator_id, Source.amount, Source.representative_name, Source.postal_code, Source.address, Source.occupation_id, Source.receipt_no);

END
GO
/****** Object:  StoredProcedure [dbo].[sp_demo_relational_Data]    Script Date: 7/29/2023 9:50:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>


-- =============================================
CREATE PROCEDURE [dbo].[sp_demo_relational_Data] 
	-- Add the parameters for the stored procedure here
	 @ProductTable ProductTable READONLY  ,
     @ProductVariantTable [dbo].[ProductsVariantsTable] READONLY
AS
BEGIN

 BEGIN TRY  
 BEGIN TRANSACTION  
  
    DECLARE @InsertedValuesProduct TABLE(ID BIGINT NOT NULL, [UID] NVARCHAR(100) NULL ) 

    DECLARE @InsertedValuesProductVariant TABLE(ID BIGINT NOT NULL, [UID] NVARCHAR(100) NULL ) 

	MERGE INTO [dbo].[products] USING @ProductTable AS temp ON 1 = 0
	WHEN NOT MATCHED THEN
    INSERT (product_name, product_code,[base_price])
    VALUES (temp.product_name, temp.product_code, temp.base_price)
    OUTPUT Inserted.id, temp.reference_id into @InsertedValuesProduct ;

	    -- --prod variants  
    INSERT INTO [dbo].[products_variants]([product_id], [variant_name])   
    SELECT IVP.ID,[variant_name] 
	from @ProductVariantTable PVT left join @InsertedValuesProduct IVP ON PVT.reference_id = IVP.[UID]  
    
   
  commit TRANSACTION   
 END TRY  
 BEGIN CATCH 
 ROLLBACK TRAN
 throw  
 END CATCH  
END
GO
