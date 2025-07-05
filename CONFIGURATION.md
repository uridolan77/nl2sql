# NL2SQL Configuration Guide

## ⚠️ Security Notice

**IMPORTANT**: Never commit sensitive configuration files to version control. This repository is configured to ignore `appsettings.json` and other sensitive files.

## Configuration Setup

### 1. Create Local Configuration

Copy the template file and update with your actual values:

```bash
cp appsettings.Development.json.template appsettings.json
```

### 2. Update Connection Strings

Edit `appsettings.json` and replace the placeholder values:

#### DefaultConnection (Business Metadata Database)
```json
"DefaultConnection": "Server=YOUR_SERVER;Database=BIReportingCopilot_Dev;User ID=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=true"
```

#### BIDatabase (Data Source Database)
```json
"BIDatabase": "Server=YOUR_BI_SERVER;Database=YOUR_BI_DATABASE;User ID=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=true"
```

### 3. Configure OpenAI API

Replace the OpenAI API key with your actual key:

```json
"OpenAI": {
  "ApiKey": "sk-proj-YOUR_ACTUAL_OPENAI_API_KEY_HERE"
}
```

### 4. Update JWT Secret

Generate a secure JWT secret (minimum 32 characters):

```json
"JwtSettings": {
  "Secret": "YOUR_SECURE_JWT_SECRET_KEY_HERE_MINIMUM_32_CHARACTERS"
}
```

## Database Requirements

### Business Metadata Tables

The system requires these tables in your `DefaultConnection` database:

1. **BusinessTableInfo** - Contains metadata about database tables
2. **BusinessColumnInfo** - Contains metadata about database columns

### Sample Schema

```sql
-- BusinessTableInfo table structure
CREATE TABLE [dbo].[BusinessTableInfo] (
    [Id] int IDENTITY(1,1) PRIMARY KEY,
    [TableName] nvarchar(255) NOT NULL,
    [BusinessPurpose] nvarchar(1000),
    [DomainClassification] nvarchar(255),
    [ImportanceScore] decimal(5,2) DEFAULT 1.0,
    [IsActive] bit DEFAULT 1,
    -- ... additional metadata fields
);

-- BusinessColumnInfo table structure  
CREATE TABLE [dbo].[BusinessColumnInfo] (
    [Id] int IDENTITY(1,1) PRIMARY KEY,
    [TableName] nvarchar(255) NOT NULL,
    [ColumnName] nvarchar(255) NOT NULL,
    [BusinessMeaning] nvarchar(1000),
    [DataType] nvarchar(255),
    [ImportanceScore] decimal(5,2) DEFAULT 1.0,
    [IsActive] bit DEFAULT 1,
    -- ... additional metadata fields
);
```

## Environment-Specific Configuration

### Development
- Use `appsettings.Development.json` for local development
- Enable detailed logging and debugging features

### Production
- Use `appsettings.Production.json` for production deployment
- Enable security features and audit logging
- Use Azure Key Vault for sensitive configuration

## Security Best Practices

1. **Never commit sensitive data** to version control
2. **Use environment variables** for production secrets
3. **Enable HTTPS** in production environments
4. **Rotate API keys** regularly
5. **Use Azure Key Vault** for production secrets
6. **Enable audit logging** in production

## Testing Configuration

For testing the Enhanced NL2SQL system:

1. Ensure your database contains sample data in BusinessTableInfo and BusinessColumnInfo
2. Verify database connectivity before running tests
3. The system will fail gracefully if database is not accessible (no mock data fallback)

## Troubleshooting

### Database Connection Issues
- Verify connection strings are correct
- Check firewall settings
- Ensure database user has proper permissions

### OpenAI API Issues
- Verify API key is valid and has sufficient credits
- Check rate limiting settings
- Monitor API usage and costs

### Entity Framework Issues
- Run database migrations if needed
- Verify table schemas match entity models
- Check Entity Framework logging for detailed errors
