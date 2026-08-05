BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Units]') AND [c].[name] = N'Symbol');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Units] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Units] ALTER COLUMN [Symbol] nvarchar(20) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Units]') AND [c].[name] = N'RowVersion');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Units] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [Units] DROP COLUMN [RowVersion];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    ALTER TABLE [Units] ADD [RowVersion] rowversion NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Units]') AND [c].[name] = N'Name');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Units] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [Units] ALTER COLUMN [Name] nvarchar(150) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Strengths]') AND [c].[name] = N'Value');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Strengths] DROP CONSTRAINT ' + @var3 + ';');
    ALTER TABLE [Strengths] ALTER COLUMN [Value] decimal(18,4) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var4 nvarchar(max);
    SELECT @var4 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Strengths]') AND [c].[name] = N'RowVersion');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Strengths] DROP CONSTRAINT ' + @var4 + ';');
    ALTER TABLE [Strengths] DROP COLUMN [RowVersion];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    ALTER TABLE [Strengths] ADD [RowVersion] rowversion NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var5 nvarchar(max);
    SELECT @var5 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Strengths]') AND [c].[name] = N'Name');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Strengths] DROP CONSTRAINT ' + @var5 + ';');
    ALTER TABLE [Strengths] ALTER COLUMN [Name] nvarchar(150) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var6 nvarchar(max);
    SELECT @var6 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Strengths]') AND [c].[name] = N'MeasurementUnit');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Strengths] DROP CONSTRAINT ' + @var6 + ';');
    ALTER TABLE [Strengths] ALTER COLUMN [MeasurementUnit] nvarchar(40) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var7 nvarchar(max);
    SELECT @var7 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProductBrands]') AND [c].[name] = N'RowVersion');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [ProductBrands] DROP CONSTRAINT ' + @var7 + ';');
    ALTER TABLE [ProductBrands] DROP COLUMN [RowVersion];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    ALTER TABLE [ProductBrands] ADD [RowVersion] rowversion NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var8 nvarchar(max);
    SELECT @var8 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProductBrands]') AND [c].[name] = N'Name');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [ProductBrands] DROP CONSTRAINT ' + @var8 + ';');
    ALTER TABLE [ProductBrands] ALTER COLUMN [Name] nvarchar(150) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var9 nvarchar(max);
    SELECT @var9 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProductBrands]') AND [c].[name] = N'Code');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [ProductBrands] DROP CONSTRAINT ' + @var9 + ';');
    ALTER TABLE [ProductBrands] ALTER COLUMN [Code] nvarchar(50) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var10 nvarchar(max);
    SELECT @var10 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Manufacturers]') AND [c].[name] = N'RowVersion');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Manufacturers] DROP CONSTRAINT ' + @var10 + ';');
    ALTER TABLE [Manufacturers] DROP COLUMN [RowVersion];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    ALTER TABLE [Manufacturers] ADD [RowVersion] rowversion NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var11 nvarchar(max);
    SELECT @var11 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Manufacturers]') AND [c].[name] = N'Name');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Manufacturers] DROP CONSTRAINT ' + @var11 + ';');
    ALTER TABLE [Manufacturers] ALTER COLUMN [Name] nvarchar(150) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var12 nvarchar(max);
    SELECT @var12 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Manufacturers]') AND [c].[name] = N'LicenseNumber');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [Manufacturers] DROP CONSTRAINT ' + @var12 + ';');
    ALTER TABLE [Manufacturers] ALTER COLUMN [LicenseNumber] nvarchar(80) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var13 nvarchar(max);
    SELECT @var13 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Manufacturers]') AND [c].[name] = N'Code');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [Manufacturers] DROP CONSTRAINT ' + @var13 + ';');
    ALTER TABLE [Manufacturers] ALTER COLUMN [Code] nvarchar(50) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var14 nvarchar(max);
    SELECT @var14 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GenericMedicines]') AND [c].[name] = N'RowVersion');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [GenericMedicines] DROP CONSTRAINT ' + @var14 + ';');
    ALTER TABLE [GenericMedicines] DROP COLUMN [RowVersion];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    ALTER TABLE [GenericMedicines] ADD [RowVersion] rowversion NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var15 nvarchar(max);
    SELECT @var15 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GenericMedicines]') AND [c].[name] = N'Name');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [GenericMedicines] DROP CONSTRAINT ' + @var15 + ';');
    ALTER TABLE [GenericMedicines] ALTER COLUMN [Name] nvarchar(150) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var16 nvarchar(max);
    SELECT @var16 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GenericMedicines]') AND [c].[name] = N'Description');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [GenericMedicines] DROP CONSTRAINT ' + @var16 + ';');
    ALTER TABLE [GenericMedicines] ALTER COLUMN [Description] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var17 nvarchar(max);
    SELECT @var17 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DosageForms]') AND [c].[name] = N'RowVersion');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [DosageForms] DROP CONSTRAINT ' + @var17 + ';');
    ALTER TABLE [DosageForms] DROP COLUMN [RowVersion];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    ALTER TABLE [DosageForms] ADD [RowVersion] rowversion NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var18 nvarchar(max);
    SELECT @var18 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DosageForms]') AND [c].[name] = N'Name');
    IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [DosageForms] DROP CONSTRAINT ' + @var18 + ';');
    ALTER TABLE [DosageForms] ALTER COLUMN [Name] nvarchar(150) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    DECLARE @var19 nvarchar(max);
    SELECT @var19 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DosageForms]') AND [c].[name] = N'Code');
    IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [DosageForms] DROP CONSTRAINT ' + @var19 + ';');
    ALTER TABLE [DosageForms] ALTER COLUMN [Code] nvarchar(50) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    CREATE INDEX [IX_Units_CompanyId] ON [Units] ([CompanyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    CREATE INDEX [IX_Strengths_CompanyId] ON [Strengths] ([CompanyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ProductBrands_CompanyId_Code] ON [ProductBrands] ([CompanyId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Manufacturers_CompanyId_Code] ON [Manufacturers] ([CompanyId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    CREATE INDEX [IX_GenericMedicines_CompanyId] ON [GenericMedicines] ([CompanyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DosageForms_CompanyId_Code] ON [DosageForms] ([CompanyId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    ALTER TABLE [DosageForms] ADD CONSTRAINT [FK_DosageForms_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    ALTER TABLE [GenericMedicines] ADD CONSTRAINT [FK_GenericMedicines_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    ALTER TABLE [Manufacturers] ADD CONSTRAINT [FK_Manufacturers_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    ALTER TABLE [ProductBrands] ADD CONSTRAINT [FK_ProductBrands_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    ALTER TABLE [Strengths] ADD CONSTRAINT [FK_Strengths_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    ALTER TABLE [Units] ADD CONSTRAINT [FK_Units_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805151130_CatalogMasterCrudFix'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260805151130_CatalogMasterCrudFix', N'10.0.10');
END;

COMMIT;
GO

