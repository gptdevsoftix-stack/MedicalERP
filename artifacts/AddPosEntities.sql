IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] uniqueidentifier NOT NULL,
        [CompanyId] uniqueidentifier NULL,
        [Description] nvarchar(250) NULL,
        [IsSystemRole] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] uniqueidentifier NOT NULL,
        [CompanyId] uniqueidentifier NULL,
        [FirstName] nvarchar(80) NOT NULL,
        [LastName] nvarchar(80) NOT NULL,
        [IsPlatformAdmin] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [LastLoginAt] datetime2 NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE TABLE [Companies] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Code] nvarchar(40) NOT NULL,
        [LegalName] nvarchar(200) NULL,
        [LogoUrl] nvarchar(max) NULL,
        [Email] nvarchar(256) NULL,
        [Phone] nvarchar(40) NULL,
        [Address] nvarchar(max) NULL,
        [City] nvarchar(max) NULL,
        [State] nvarchar(max) NULL,
        [Country] nvarchar(80) NOT NULL,
        [TaxNumber] nvarchar(max) NULL,
        [CurrencyCode] nvarchar(3) NOT NULL,
        [TimeZone] nvarchar(100) NOT NULL,
        [SubscriptionStatus] int NOT NULL,
        [SubscriptionStartsAt] datetime2 NULL,
        [SubscriptionEndsAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Companies] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [TokenHash] nvarchar(128) NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [CreatedByIp] nvarchar(64) NULL,
        [RevokedAt] datetime2 NULL,
        [RevokedByIp] nvarchar(64) NULL,
        [ReplacedByTokenHash] nvarchar(max) NULL,
        [ReasonRevoked] nvarchar(250) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] uniqueidentifier NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE TABLE [Stores] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Code] nvarchar(40) NOT NULL,
        [Email] nvarchar(256) NULL,
        [Phone] nvarchar(max) NULL,
        [Address] nvarchar(max) NULL,
        [City] nvarchar(max) NULL,
        [State] nvarchar(max) NULL,
        [Country] nvarchar(max) NOT NULL,
        [TaxNumber] nvarchar(max) NULL,
        [CurrencyCode] nvarchar(3) NOT NULL,
        [TimeZone] nvarchar(100) NOT NULL,
        [IsHeadOffice] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Stores] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Stores_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE TABLE [UserCompanyAccesses] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [IsDefaultCompany] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UserCompanyAccesses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserCompanyAccesses_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE TABLE [UserStoreAccesses] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        [IsDefaultStore] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UserStoreAccesses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserStoreAccesses_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserStoreAccesses_Stores_StoreId] FOREIGN KEY ([StoreId]) REFERENCES [Stores] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE TABLE [Warehouses] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Code] nvarchar(40) NOT NULL,
        [WarehouseType] int NOT NULL,
        [Address] nvarchar(max) NULL,
        [IsDefault] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Warehouses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Warehouses_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Warehouses_Stores_StoreId] FOREIGN KEY ([StoreId]) REFERENCES [Stores] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AspNetRoles_CompanyId_NormalizedName] ON [AspNetRoles] ([CompanyId], [NormalizedName]) WHERE [CompanyId] IS NOT NULL AND [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_CompanyId] ON [AspNetUsers] ([CompanyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Companies_Code] ON [Companies] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RefreshTokens_TokenHash] ON [RefreshTokens] ([TokenHash]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Stores_CompanyId_Code] ON [Stores] ([CompanyId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE INDEX [IX_UserCompanyAccesses_CompanyId] ON [UserCompanyAccesses] ([CompanyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UserCompanyAccesses_UserId_CompanyId] ON [UserCompanyAccesses] ([UserId], [CompanyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE INDEX [IX_UserStoreAccesses_CompanyId] ON [UserStoreAccesses] ([CompanyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE INDEX [IX_UserStoreAccesses_StoreId] ON [UserStoreAccesses] ([StoreId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UserStoreAccesses_UserId_StoreId] ON [UserStoreAccesses] ([UserId], [StoreId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Warehouses_CompanyId_StoreId_Code] ON [Warehouses] ([CompanyId], [StoreId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    CREATE INDEX [IX_Warehouses_StoreId] ON [Warehouses] ([StoreId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731145715_InitialModule1'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260731145715_InitialModule1', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] nvarchar(max) NOT NULL,
        [Action] nvarchar(max) NOT NULL,
        [EntityName] nvarchar(max) NOT NULL,
        [EntityId] nvarchar(max) NULL,
        [OldValuesJson] nvarchar(max) NULL,
        [NewValuesJson] nvarchar(max) NULL,
        [IpAddress] nvarchar(max) NULL,
        [OccurredAt] datetimeoffset NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [Categories] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Code] nvarchar(max) NOT NULL,
        [ParentCategoryId] uniqueidentifier NULL,
        [DisplayOrder] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Categories_Categories_ParentCategoryId] FOREIGN KEY ([ParentCategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [CustomerLedgerEntries] (
        [Id] uniqueidentifier NOT NULL,
        [CustomerId] uniqueidentifier NOT NULL,
        [EntryType] int NOT NULL,
        [ReferenceId] uniqueidentifier NULL,
        [ReferenceNumber] nvarchar(max) NULL,
        [EntryDate] datetimeoffset NOT NULL,
        [Debit] decimal(18,2) NOT NULL,
        [Credit] decimal(18,2) NOT NULL,
        [BalanceAfter] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CustomerLedgerEntries] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [Customers] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Code] nvarchar(max) NOT NULL,
        [Phone] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [Address] nvarchar(max) NULL,
        [TaxNumber] nvarchar(max) NULL,
        [CreditLimit] decimal(18,2) NOT NULL,
        [CreditDays] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [Doctors] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [RegistrationNumber] nvarchar(max) NULL,
        [Specialty] nvarchar(max) NULL,
        [Phone] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Doctors] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [DosageForms] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Code] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_DosageForms] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [ExpenseCategories] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Code] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ExpenseCategories] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [Expenses] (
        [Id] uniqueidentifier NOT NULL,
        [ExpenseCategoryId] uniqueidentifier NOT NULL,
        [RegisterSessionId] uniqueidentifier NULL,
        [PaymentMethodId] uniqueidentifier NOT NULL,
        [ExpenseNumber] nvarchar(max) NOT NULL,
        [ExpenseDate] datetimeoffset NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [AttachmentUrl] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Expenses] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [GenericMedicines] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_GenericMedicines] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [Manufacturers] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Code] nvarchar(max) NOT NULL,
        [LicenseNumber] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Manufacturers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [NumberSequences] (
        [Id] uniqueidentifier NOT NULL,
        [DocumentType] int NOT NULL,
        [Prefix] nvarchar(max) NOT NULL,
        [NextNumber] bigint NOT NULL,
        [Padding] int NOT NULL,
        [ResetYear] int NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_NumberSequences] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [Patients] (
        [Id] uniqueidentifier NOT NULL,
        [CustomerId] uniqueidentifier NULL,
        [Name] nvarchar(max) NOT NULL,
        [DateOfBirth] date NULL,
        [Gender] nvarchar(max) NULL,
        [Allergies] nvarchar(max) NULL,
        [MedicalNotes] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Patients] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [PaymentMethods] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Code] nvarchar(max) NOT NULL,
        [MethodType] int NOT NULL,
        [RequiresReference] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_PaymentMethods] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [Prescriptions] (
        [Id] uniqueidentifier NOT NULL,
        [PatientId] uniqueidentifier NOT NULL,
        [DoctorId] uniqueidentifier NOT NULL,
        [PrescriptionNumber] nvarchar(max) NOT NULL,
        [PrescriptionDate] date NOT NULL,
        [ValidUntil] date NULL,
        [Status] int NOT NULL,
        [AttachmentUrl] nvarchar(max) NULL,
        [VerifiedByUserId] nvarchar(max) NULL,
        [VerifiedAt] datetimeoffset NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Prescriptions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [ProductBrands] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Code] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ProductBrands] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [ProductPriceHistories] (
        [Id] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [OldPrice] decimal(18,2) NOT NULL,
        [NewPrice] decimal(18,2) NOT NULL,
        [EffectiveAt] datetimeoffset NOT NULL,
        [Reason] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ProductPriceHistories] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [PurchaseInvoices] (
        [Id] uniqueidentifier NOT NULL,
        [SupplierId] uniqueidentifier NOT NULL,
        [PurchaseOrderId] uniqueidentifier NULL,
        [GoodsReceiptId] uniqueidentifier NULL,
        [InvoiceNumber] nvarchar(max) NOT NULL,
        [SupplierInvoiceNumber] nvarchar(max) NOT NULL,
        [InvoiceDate] datetimeoffset NOT NULL,
        [DueDate] datetimeoffset NULL,
        [Status] int NOT NULL,
        [Subtotal] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [GrandTotal] decimal(18,2) NOT NULL,
        [PaidAmount] decimal(18,2) NOT NULL,
        [DueAmount] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_PurchaseInvoices] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [PurchaseReturns] (
        [Id] uniqueidentifier NOT NULL,
        [SupplierId] uniqueidentifier NOT NULL,
        [WarehouseId] uniqueidentifier NOT NULL,
        [PurchaseInvoiceId] uniqueidentifier NULL,
        [ReturnNumber] nvarchar(max) NOT NULL,
        [ReturnDate] datetimeoffset NOT NULL,
        [Status] int NOT NULL,
        [Subtotal] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [GrandTotal] decimal(18,2) NOT NULL,
        [Reason] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_PurchaseReturns] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [ReasonCodes] (
        [Id] uniqueidentifier NOT NULL,
        [Code] nvarchar(max) NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [AppliesTo] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ReasonCodes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [Registers] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Code] nvarchar(max) NOT NULL,
        [WarehouseId] uniqueidentifier NOT NULL,
        [IsEnabled] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Registers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [SaleReturns] (
        [Id] uniqueidentifier NOT NULL,
        [SaleId] uniqueidentifier NOT NULL,
        [WarehouseId] uniqueidentifier NOT NULL,
        [ReturnNumber] nvarchar(max) NOT NULL,
        [ReturnDate] datetimeoffset NOT NULL,
        [Status] int NOT NULL,
        [Subtotal] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [RefundAmount] decimal(18,2) NOT NULL,
        [Reason] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SaleReturns] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [StockAdjustments] (
        [Id] uniqueidentifier NOT NULL,
        [WarehouseId] uniqueidentifier NOT NULL,
        [AdjustmentNumber] nvarchar(max) NOT NULL,
        [AdjustmentType] int NOT NULL,
        [AdjustmentDate] datetimeoffset NOT NULL,
        [ReasonCodeId] uniqueidentifier NOT NULL,
        [Notes] nvarchar(max) NULL,
        [IsPosted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StockAdjustments] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [StockCounts] (
        [Id] uniqueidentifier NOT NULL,
        [WarehouseId] uniqueidentifier NOT NULL,
        [CountNumber] nvarchar(max) NOT NULL,
        [CountDate] datetimeoffset NOT NULL,
        [Status] int NOT NULL,
        [Notes] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StockCounts] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [StockDisposals] (
        [Id] uniqueidentifier NOT NULL,
        [WarehouseId] uniqueidentifier NOT NULL,
        [DisposalNumber] nvarchar(max) NOT NULL,
        [DisposalDate] datetimeoffset NOT NULL,
        [ReasonCodeId] uniqueidentifier NOT NULL,
        [ApprovedByUserId] nvarchar(max) NULL,
        [Notes] nvarchar(max) NULL,
        [IsPosted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StockDisposals] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [StockTransactions] (
        [Id] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [WarehouseId] uniqueidentifier NOT NULL,
        [ProductBatchId] uniqueidentifier NULL,
        [TransactionType] int NOT NULL,
        [ReferenceType] int NOT NULL,
        [ReferenceId] uniqueidentifier NULL,
        [ReferenceNumber] nvarchar(max) NULL,
        [QuantityIn] decimal(18,2) NOT NULL,
        [QuantityOut] decimal(18,2) NOT NULL,
        [BalanceAfter] decimal(18,2) NOT NULL,
        [UnitCost] decimal(18,2) NOT NULL,
        [TransactionAt] datetimeoffset NOT NULL,
        [Notes] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StockTransactions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [StoreSettings] (
        [Id] uniqueidentifier NOT NULL,
        [Key] nvarchar(max) NOT NULL,
        [Value] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StoreSettings] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [Strengths] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Value] decimal(18,2) NULL,
        [MeasurementUnit] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Strengths] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [SupplierLedgerEntries] (
        [Id] uniqueidentifier NOT NULL,
        [SupplierId] uniqueidentifier NOT NULL,
        [EntryType] int NOT NULL,
        [ReferenceId] uniqueidentifier NULL,
        [ReferenceNumber] nvarchar(max) NULL,
        [EntryDate] datetimeoffset NOT NULL,
        [Debit] decimal(18,2) NOT NULL,
        [Credit] decimal(18,2) NOT NULL,
        [BalanceAfter] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SupplierLedgerEntries] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [Suppliers] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Code] nvarchar(max) NOT NULL,
        [ContactPerson] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [Phone] nvarchar(max) NULL,
        [Address] nvarchar(max) NULL,
        [TaxNumber] nvarchar(max) NULL,
        [CreditDays] int NOT NULL,
        [CreditLimit] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Suppliers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [TaxRates] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Rate] decimal(18,2) NOT NULL,
        [IsInclusive] bit NOT NULL,
        [EffectiveFrom] date NULL,
        [EffectiveTo] date NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TaxRates] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [Units] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Symbol] nvarchar(max) NOT NULL,
        [AllowsDecimal] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Units] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [SaleOrders] (
        [Id] uniqueidentifier NOT NULL,
        [CustomerId] uniqueidentifier NULL,
        [WarehouseId] uniqueidentifier NOT NULL,
        [OrderNumber] nvarchar(max) NOT NULL,
        [OrderDate] datetimeoffset NOT NULL,
        [RequiredDate] datetimeoffset NULL,
        [Status] int NOT NULL,
        [Subtotal] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [GrandTotal] decimal(18,2) NOT NULL,
        [Notes] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SaleOrders] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SaleOrders_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [Sales] (
        [Id] uniqueidentifier NOT NULL,
        [WarehouseId] uniqueidentifier NOT NULL,
        [RegisterSessionId] uniqueidentifier NOT NULL,
        [SaleOrderId] uniqueidentifier NULL,
        [CustomerId] uniqueidentifier NULL,
        [PrescriptionId] uniqueidentifier NULL,
        [InvoiceNumber] nvarchar(max) NOT NULL,
        [SaleDate] datetimeoffset NOT NULL,
        [Status] int NOT NULL,
        [PaymentStatus] int NOT NULL,
        [Subtotal] decimal(18,2) NOT NULL,
        [ItemDiscount] decimal(18,2) NOT NULL,
        [InvoiceDiscount] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [RoundOffAmount] decimal(18,2) NOT NULL,
        [GrandTotal] decimal(18,2) NOT NULL,
        [PaidAmount] decimal(18,2) NOT NULL,
        [ChangeAmount] decimal(18,2) NOT NULL,
        [DueAmount] decimal(18,2) NOT NULL,
        [Notes] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Sales] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Sales_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [PurchaseInvoiceItems] (
        [Id] uniqueidentifier NOT NULL,
        [PurchaseInvoiceId] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [ProductUnitId] uniqueidentifier NOT NULL,
        [ProductBatchId] uniqueidentifier NULL,
        [Quantity] decimal(18,2) NOT NULL,
        [UnitPrice] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [LineTotal] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_PurchaseInvoiceItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PurchaseInvoiceItems_PurchaseInvoices_PurchaseInvoiceId] FOREIGN KEY ([PurchaseInvoiceId]) REFERENCES [PurchaseInvoices] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [SupplierPayments] (
        [Id] uniqueidentifier NOT NULL,
        [SupplierId] uniqueidentifier NOT NULL,
        [PurchaseInvoiceId] uniqueidentifier NULL,
        [PaymentMethodId] uniqueidentifier NOT NULL,
        [PaymentNumber] nvarchar(max) NOT NULL,
        [PaymentDate] datetimeoffset NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [ReferenceNumber] nvarchar(max) NULL,
        [Notes] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SupplierPayments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SupplierPayments_PurchaseInvoices_PurchaseInvoiceId] FOREIGN KEY ([PurchaseInvoiceId]) REFERENCES [PurchaseInvoices] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [PurchaseReturnItems] (
        [Id] uniqueidentifier NOT NULL,
        [PurchaseReturnId] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [ProductBatchId] uniqueidentifier NOT NULL,
        [ProductUnitId] uniqueidentifier NOT NULL,
        [Quantity] decimal(18,2) NOT NULL,
        [BaseQuantity] decimal(18,2) NOT NULL,
        [UnitPrice] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [LineTotal] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_PurchaseReturnItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PurchaseReturnItems_PurchaseReturns_PurchaseReturnId] FOREIGN KEY ([PurchaseReturnId]) REFERENCES [PurchaseReturns] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [RegisterSessions] (
        [Id] uniqueidentifier NOT NULL,
        [RegisterId] uniqueidentifier NOT NULL,
        [CashierUserId] nvarchar(max) NOT NULL,
        [OpenedAt] datetimeoffset NOT NULL,
        [ClosedAt] datetimeoffset NULL,
        [OpeningCash] decimal(18,2) NOT NULL,
        [ExpectedCash] decimal(18,2) NOT NULL,
        [CountedCash] decimal(18,2) NOT NULL,
        [Difference] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [ClosingNotes] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_RegisterSessions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RegisterSessions_Registers_RegisterId] FOREIGN KEY ([RegisterId]) REFERENCES [Registers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [SaleReturnItems] (
        [Id] uniqueidentifier NOT NULL,
        [SaleReturnId] uniqueidentifier NOT NULL,
        [SaleItemId] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [ProductBatchId] uniqueidentifier NOT NULL,
        [Quantity] decimal(18,2) NOT NULL,
        [BaseQuantity] decimal(18,2) NOT NULL,
        [UnitPrice] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [LineTotal] decimal(18,2) NOT NULL,
        [ReturnToStock] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SaleReturnItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SaleReturnItems_SaleReturns_SaleReturnId] FOREIGN KEY ([SaleReturnId]) REFERENCES [SaleReturns] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [StockAdjustmentItems] (
        [Id] uniqueidentifier NOT NULL,
        [StockAdjustmentId] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [ProductBatchId] uniqueidentifier NULL,
        [Quantity] decimal(18,2) NOT NULL,
        [UnitCost] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StockAdjustmentItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StockAdjustmentItems_StockAdjustments_StockAdjustmentId] FOREIGN KEY ([StockAdjustmentId]) REFERENCES [StockAdjustments] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [StockCountItems] (
        [Id] uniqueidentifier NOT NULL,
        [StockCountId] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [ProductBatchId] uniqueidentifier NULL,
        [SystemQuantity] decimal(18,2) NOT NULL,
        [CountedQuantity] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StockCountItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StockCountItems_StockCounts_StockCountId] FOREIGN KEY ([StockCountId]) REFERENCES [StockCounts] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [StockDisposalItems] (
        [Id] uniqueidentifier NOT NULL,
        [StockDisposalId] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [ProductBatchId] uniqueidentifier NOT NULL,
        [Quantity] decimal(18,2) NOT NULL,
        [UnitCost] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StockDisposalItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StockDisposalItems_StockDisposals_StockDisposalId] FOREIGN KEY ([StockDisposalId]) REFERENCES [StockDisposals] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [PurchaseOrders] (
        [Id] uniqueidentifier NOT NULL,
        [SupplierId] uniqueidentifier NOT NULL,
        [WarehouseId] uniqueidentifier NOT NULL,
        [OrderNumber] nvarchar(max) NOT NULL,
        [OrderDate] datetimeoffset NOT NULL,
        [ExpectedDeliveryDate] datetimeoffset NULL,
        [Status] int NOT NULL,
        [Subtotal] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [OtherCharges] decimal(18,2) NOT NULL,
        [GrandTotal] decimal(18,2) NOT NULL,
        [Notes] nvarchar(max) NULL,
        [ApprovedByUserId] nvarchar(max) NULL,
        [ApprovedAt] datetimeoffset NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_PurchaseOrders] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PurchaseOrders_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [SupplierStores] (
        [Id] uniqueidentifier NOT NULL,
        [SupplierId] uniqueidentifier NOT NULL,
        [IsPreferred] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SupplierStores] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SupplierStores_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [Products] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Code] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        [CategoryId] uniqueidentifier NOT NULL,
        [ProductBrandId] uniqueidentifier NULL,
        [ManufacturerId] uniqueidentifier NULL,
        [GenericMedicineId] uniqueidentifier NULL,
        [DosageFormId] uniqueidentifier NULL,
        [StrengthId] uniqueidentifier NULL,
        [BaseUnitId] uniqueidentifier NOT NULL,
        [ProductType] int NOT NULL,
        [IsMedicine] bit NOT NULL,
        [RequiresPrescription] bit NOT NULL,
        [IsControlledDrug] bit NOT NULL,
        [TrackBatch] bit NOT NULL,
        [TrackExpiry] bit NOT NULL,
        [AllowDiscount] bit NOT NULL,
        [AllowNegativeStock] bit NOT NULL,
        [RegulatoryNumber] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Products_DosageForms_DosageFormId] FOREIGN KEY ([DosageFormId]) REFERENCES [DosageForms] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Products_GenericMedicines_GenericMedicineId] FOREIGN KEY ([GenericMedicineId]) REFERENCES [GenericMedicines] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Products_Manufacturers_ManufacturerId] FOREIGN KEY ([ManufacturerId]) REFERENCES [Manufacturers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Products_ProductBrands_ProductBrandId] FOREIGN KEY ([ProductBrandId]) REFERENCES [ProductBrands] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Products_Strengths_StrengthId] FOREIGN KEY ([StrengthId]) REFERENCES [Strengths] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Products_Units_BaseUnitId] FOREIGN KEY ([BaseUnitId]) REFERENCES [Units] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [SaleOrderItems] (
        [Id] uniqueidentifier NOT NULL,
        [SaleOrderId] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [ProductUnitId] uniqueidentifier NOT NULL,
        [OrderedQuantity] decimal(18,2) NOT NULL,
        [FulfilledQuantity] decimal(18,2) NOT NULL,
        [ConversionFactor] decimal(18,2) NOT NULL,
        [UnitPrice] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [LineTotal] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SaleOrderItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SaleOrderItems_SaleOrders_SaleOrderId] FOREIGN KEY ([SaleOrderId]) REFERENCES [SaleOrders] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [SalePayments] (
        [Id] uniqueidentifier NOT NULL,
        [SaleId] uniqueidentifier NOT NULL,
        [PaymentMethodId] uniqueidentifier NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [PaidAt] datetimeoffset NOT NULL,
        [ReferenceNumber] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SalePayments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SalePayments_Sales_SaleId] FOREIGN KEY ([SaleId]) REFERENCES [Sales] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [CashMovements] (
        [Id] uniqueidentifier NOT NULL,
        [RegisterSessionId] uniqueidentifier NOT NULL,
        [MovementType] int NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [MovementAt] datetimeoffset NOT NULL,
        [Reason] nvarchar(max) NOT NULL,
        [ReferenceNumber] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CashMovements] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CashMovements_RegisterSessions_RegisterSessionId] FOREIGN KEY ([RegisterSessionId]) REFERENCES [RegisterSessions] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [GoodsReceipts] (
        [Id] uniqueidentifier NOT NULL,
        [SupplierId] uniqueidentifier NOT NULL,
        [WarehouseId] uniqueidentifier NOT NULL,
        [PurchaseOrderId] uniqueidentifier NULL,
        [ReceiptNumber] nvarchar(max) NOT NULL,
        [SupplierDeliveryNote] nvarchar(max) NULL,
        [ReceiptDate] datetimeoffset NOT NULL,
        [IsPosted] bit NOT NULL,
        [Notes] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_GoodsReceipts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GoodsReceipts_PurchaseOrders_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrders] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [PrescriptionItems] (
        [Id] uniqueidentifier NOT NULL,
        [PrescriptionId] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [PrescribedQuantity] decimal(18,2) NOT NULL,
        [DispensedQuantity] decimal(18,2) NOT NULL,
        [DosageInstructions] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_PrescriptionItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PrescriptionItems_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PrescriptionItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [ProductBatches] (
        [Id] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [WarehouseId] uniqueidentifier NOT NULL,
        [BatchNumber] nvarchar(max) NOT NULL,
        [ManufacturingDate] date NULL,
        [ExpiryDate] date NULL,
        [PurchasePrice] decimal(18,2) NOT NULL,
        [CostPrice] decimal(18,2) NOT NULL,
        [SalePrice] decimal(18,2) NOT NULL,
        [MaximumRetailPrice] decimal(18,2) NULL,
        [ReceivedAt] datetimeoffset NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ProductBatches] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProductBatches_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ProductBatches_Warehouses_WarehouseId] FOREIGN KEY ([WarehouseId]) REFERENCES [Warehouses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [ProductUnits] (
        [Id] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [UnitId] uniqueidentifier NOT NULL,
        [ConversionFactor] decimal(18,2) NOT NULL,
        [IsBaseUnit] bit NOT NULL,
        [IsPurchaseUnit] bit NOT NULL,
        [IsSaleUnit] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ProductUnits] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProductUnits_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ProductUnits_Units_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [Units] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [PurchaseOrderItems] (
        [Id] uniqueidentifier NOT NULL,
        [PurchaseOrderId] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [ProductUnitId] uniqueidentifier NOT NULL,
        [OrderedQuantity] decimal(18,2) NOT NULL,
        [ReceivedQuantity] decimal(18,2) NOT NULL,
        [FreeQuantity] decimal(18,2) NOT NULL,
        [ConversionFactor] decimal(18,2) NOT NULL,
        [UnitPrice] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [LineTotal] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_PurchaseOrderItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PurchaseOrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrders] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [SaleItems] (
        [Id] uniqueidentifier NOT NULL,
        [SaleId] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [ProductUnitId] uniqueidentifier NOT NULL,
        [PrescriptionItemId] uniqueidentifier NULL,
        [Quantity] decimal(18,2) NOT NULL,
        [BaseQuantity] decimal(18,2) NOT NULL,
        [ConversionFactor] decimal(18,2) NOT NULL,
        [UnitPrice] decimal(18,2) NOT NULL,
        [CostPrice] decimal(18,2) NOT NULL,
        [GrossAmount] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [NetAmount] decimal(18,2) NOT NULL,
        [ReturnedQuantity] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SaleItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SaleItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SaleItems_Sales_SaleId] FOREIGN KEY ([SaleId]) REFERENCES [Sales] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [StoreProducts] (
        [Id] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [SalePrice] decimal(18,2) NOT NULL,
        [WholesalePrice] decimal(18,2) NULL,
        [MinimumSalePrice] decimal(18,2) NULL,
        [ReorderLevel] decimal(18,2) NOT NULL,
        [ReorderQuantity] decimal(18,2) NOT NULL,
        [IsAvailableForSale] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_StoreProducts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StoreProducts_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StoreProducts_Stores_StoreId] FOREIGN KEY ([StoreId]) REFERENCES [Stores] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [GoodsReceiptItems] (
        [Id] uniqueidentifier NOT NULL,
        [GoodsReceiptId] uniqueidentifier NOT NULL,
        [PurchaseOrderItemId] uniqueidentifier NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [ProductUnitId] uniqueidentifier NOT NULL,
        [ProductBatchId] uniqueidentifier NOT NULL,
        [ReceivedQuantity] decimal(18,2) NOT NULL,
        [FreeQuantity] decimal(18,2) NOT NULL,
        [BaseQuantity] decimal(18,2) NOT NULL,
        [UnitCost] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_GoodsReceiptItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GoodsReceiptItems_GoodsReceipts_GoodsReceiptId] FOREIGN KEY ([GoodsReceiptId]) REFERENCES [GoodsReceipts] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_GoodsReceiptItems_ProductBatches_ProductBatchId] FOREIGN KEY ([ProductBatchId]) REFERENCES [ProductBatches] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [InventoryStocks] (
        [Id] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [WarehouseId] uniqueidentifier NOT NULL,
        [ProductBatchId] uniqueidentifier NULL,
        [QuantityOnHand] decimal(18,2) NOT NULL,
        [ReservedQuantity] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_InventoryStocks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InventoryStocks_ProductBatches_ProductBatchId] FOREIGN KEY ([ProductBatchId]) REFERENCES [ProductBatches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_InventoryStocks_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_InventoryStocks_Warehouses_WarehouseId] FOREIGN KEY ([WarehouseId]) REFERENCES [Warehouses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [ProductBarcodes] (
        [Id] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [ProductUnitId] uniqueidentifier NULL,
        [Barcode] nvarchar(max) NOT NULL,
        [IsPrimary] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ProductBarcodes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProductBarcodes_ProductUnits_ProductUnitId] FOREIGN KEY ([ProductUnitId]) REFERENCES [ProductUnits] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ProductBarcodes_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE TABLE [SaleItemBatches] (
        [Id] uniqueidentifier NOT NULL,
        [SaleItemId] uniqueidentifier NOT NULL,
        [ProductBatchId] uniqueidentifier NOT NULL,
        [BaseQuantity] decimal(18,2) NOT NULL,
        [UnitCost] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] varbinary(max) NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [StoreId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SaleItemBatches] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SaleItemBatches_ProductBatches_ProductBatchId] FOREIGN KEY ([ProductBatchId]) REFERENCES [ProductBatches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SaleItemBatches_SaleItems_SaleItemId] FOREIGN KEY ([SaleItemId]) REFERENCES [SaleItems] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_CashMovements_RegisterSessionId] ON [CashMovements] ([RegisterSessionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_Categories_ParentCategoryId] ON [Categories] ([ParentCategoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_GoodsReceiptItems_GoodsReceiptId] ON [GoodsReceiptItems] ([GoodsReceiptId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_GoodsReceiptItems_ProductBatchId] ON [GoodsReceiptItems] ([ProductBatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_GoodsReceipts_PurchaseOrderId] ON [GoodsReceipts] ([PurchaseOrderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_InventoryStocks_ProductBatchId] ON [InventoryStocks] ([ProductBatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_InventoryStocks_ProductId] ON [InventoryStocks] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_InventoryStocks_WarehouseId] ON [InventoryStocks] ([WarehouseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_PrescriptionItems_PrescriptionId] ON [PrescriptionItems] ([PrescriptionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_PrescriptionItems_ProductId] ON [PrescriptionItems] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_ProductBarcodes_ProductId] ON [ProductBarcodes] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_ProductBarcodes_ProductUnitId] ON [ProductBarcodes] ([ProductUnitId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_ProductBatches_ProductId] ON [ProductBatches] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_ProductBatches_WarehouseId] ON [ProductBatches] ([WarehouseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_Products_BaseUnitId] ON [Products] ([BaseUnitId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_Products_CategoryId] ON [Products] ([CategoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_Products_DosageFormId] ON [Products] ([DosageFormId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_Products_GenericMedicineId] ON [Products] ([GenericMedicineId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_Products_ManufacturerId] ON [Products] ([ManufacturerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_Products_ProductBrandId] ON [Products] ([ProductBrandId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_Products_StrengthId] ON [Products] ([StrengthId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_ProductUnits_ProductId] ON [ProductUnits] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_ProductUnits_UnitId] ON [ProductUnits] ([UnitId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_PurchaseInvoiceItems_PurchaseInvoiceId] ON [PurchaseInvoiceItems] ([PurchaseInvoiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_PurchaseOrderItems_ProductId] ON [PurchaseOrderItems] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_PurchaseOrderItems_PurchaseOrderId] ON [PurchaseOrderItems] ([PurchaseOrderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_PurchaseOrders_SupplierId] ON [PurchaseOrders] ([SupplierId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_PurchaseReturnItems_PurchaseReturnId] ON [PurchaseReturnItems] ([PurchaseReturnId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_RegisterSessions_RegisterId] ON [RegisterSessions] ([RegisterId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_SaleItemBatches_ProductBatchId] ON [SaleItemBatches] ([ProductBatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_SaleItemBatches_SaleItemId] ON [SaleItemBatches] ([SaleItemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_SaleItems_ProductId] ON [SaleItems] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_SaleItems_SaleId] ON [SaleItems] ([SaleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_SaleOrderItems_SaleOrderId] ON [SaleOrderItems] ([SaleOrderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_SaleOrders_CustomerId] ON [SaleOrders] ([CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_SalePayments_SaleId] ON [SalePayments] ([SaleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_SaleReturnItems_SaleReturnId] ON [SaleReturnItems] ([SaleReturnId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_Sales_CustomerId] ON [Sales] ([CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_StockAdjustmentItems_StockAdjustmentId] ON [StockAdjustmentItems] ([StockAdjustmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_StockCountItems_StockCountId] ON [StockCountItems] ([StockCountId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_StockDisposalItems_StockDisposalId] ON [StockDisposalItems] ([StockDisposalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_StoreProducts_ProductId] ON [StoreProducts] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_StoreProducts_StoreId] ON [StoreProducts] ([StoreId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_SupplierPayments_PurchaseInvoiceId] ON [SupplierPayments] ([PurchaseInvoiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    CREATE INDEX [IX_SupplierStores_SupplierId] ON [SupplierStores] ([SupplierId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803153948_AddPosEntities'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260803153948_AddPosEntities', N'10.0.10');
END;

COMMIT;
GO

