using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBackupStoredProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create BackupLog table
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BackupLog')
                BEGIN
                    CREATE TABLE [dbo].[BackupLog] (
                        [Id] INT IDENTITY(1,1) PRIMARY KEY,
                        [BackupType] NVARCHAR(10) NOT NULL,
                        [BackupFileName] NVARCHAR(255) NOT NULL,
                        [BackupPath] NVARCHAR(1000) NOT NULL,
                        [BackupSize] BIGINT NULL,
                        [Success] BIT NOT NULL,
                        [ErrorMessage] NVARCHAR(4000) NULL,
                        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
                        [CreatedBy] NVARCHAR(256) NULL
                    );
                END
            ");

            // Create sp_BackupDatabase stored procedure
            migrationBuilder.Sql(@"
                CREATE PROCEDURE [dbo].[sp_BackupDatabase]
                    @BackupType NVARCHAR(10) = 'FULL',
                    @BackupPath NVARCHAR(500) = NULL,
                    @BackupFileName NVARCHAR(255) OUTPUT,
                    @ErrorMessage NVARCHAR(4000) OUTPUT,
                    @Success BIT OUTPUT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @DatabaseName NVARCHAR(128);
                    DECLARE @DefaultBackupPath NVARCHAR(500);
                    DECLARE @FullBackupPath NVARCHAR(1000);
                    DECLARE @BackupCommand NVARCHAR(MAX);
                    DECLARE @Timestamp NVARCHAR(20);

                    SET @Success = 0;
                    SET @ErrorMessage = NULL;

                    BEGIN TRY
                        SET @DatabaseName = DB_NAME();
                        SET @Timestamp = CONVERT(NVARCHAR(20), GETDATE(), 112) + '_' +
                                        REPLACE(CONVERT(NVARCHAR(20), GETDATE(), 108), ':', '');

                        IF @BackupType = 'FULL'
                            SET @BackupFileName = @DatabaseName + '_Backup_' + @Timestamp + '.bak';
                        ELSE IF @BackupType = 'DIFF'
                            SET @BackupFileName = @DatabaseName + '_Diff_' + @Timestamp + '.bak';
                        ELSE IF @BackupType = 'LOG'
                            SET @BackupFileName = @DatabaseName + '_Log_' + @Timestamp + '.trn';
                        ELSE
                        BEGIN
                            SET @ErrorMessage = 'Invalid backup type. Use FULL, DIFF, or LOG.';
                            RETURN;
                        END

                        IF @BackupPath IS NULL
                        BEGIN
                            EXEC master.dbo.xp_instance_regread
                                N'HKEY_LOCAL_MACHINE',
                                N'Software\Microsoft\MSSQLServer\MSSQLServer',
                                N'BackupDirectory',
                                @DefaultBackupPath OUTPUT;

                            IF @DefaultBackupPath IS NULL
                                SET @DefaultBackupPath = 'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\Backup';

                            SET @BackupPath = @DefaultBackupPath;
                        END

                        IF RIGHT(@BackupPath, 1) != '\'
                            SET @BackupPath = @BackupPath + '\';

                        SET @FullBackupPath = @BackupPath + @BackupFileName;

                        IF @BackupType = 'FULL'
                        BEGIN
                            SET @BackupCommand =
                                'BACKUP DATABASE [' + @DatabaseName + '] ' +
                                'TO DISK = N''' + @FullBackupPath + ''' ' +
                                'WITH FORMAT, INIT, ' +
                                'NAME = N''' + @DatabaseName + '-Full Database Backup'', ' +
                                'COMPRESSION, STATS = 10, CHECKSUM';
                        END
                        ELSE IF @BackupType = 'DIFF'
                        BEGIN
                            SET @BackupCommand =
                                'BACKUP DATABASE [' + @DatabaseName + '] ' +
                                'TO DISK = N''' + @FullBackupPath + ''' ' +
                                'WITH DIFFERENTIAL, FORMAT, INIT, ' +
                                'NAME = N''' + @DatabaseName + '-Differential Database Backup'', ' +
                                'COMPRESSION, STATS = 10, CHECKSUM';
                        END
                        ELSE IF @BackupType = 'LOG'
                        BEGIN
                            SET @BackupCommand =
                                'BACKUP LOG [' + @DatabaseName + '] ' +
                                'TO DISK = N''' + @FullBackupPath + ''' ' +
                                'WITH FORMAT, INIT, ' +
                                'NAME = N''' + @DatabaseName + '-Transaction Log Backup'', ' +
                                'COMPRESSION, STATS = 10, CHECKSUM';
                        END

                        EXEC sp_executesql @BackupCommand;

                        SET @Success = 1;
                        SET @ErrorMessage = 'Backup completed successfully: ' + @FullBackupPath;

                    END TRY
                    BEGIN CATCH
                        SET @Success = 0;
                        SET @ErrorMessage =
                            'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR(10)) + ' | ' +
                            'Error Message: ' + ERROR_MESSAGE() + ' | ' +
                            'Error Line: ' + CAST(ERROR_LINE() AS NVARCHAR(10));
                    END CATCH
                END
            ");

            // Create sp_LogBackup stored procedure
            migrationBuilder.Sql(@"
                CREATE PROCEDURE [dbo].[sp_LogBackup]
                    @BackupType NVARCHAR(10),
                    @BackupFileName NVARCHAR(255),
                    @BackupPath NVARCHAR(1000),
                    @BackupSize BIGINT = NULL,
                    @Success BIT,
                    @ErrorMessage NVARCHAR(4000) = NULL,
                    @CreatedBy NVARCHAR(256) = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;

                    INSERT INTO [dbo].[BackupLog]
                        (BackupType, BackupFileName, BackupPath, BackupSize, Success, ErrorMessage, CreatedBy)
                    VALUES
                        (@BackupType, @BackupFileName, @BackupPath, @BackupSize, @Success, @ErrorMessage, @CreatedBy);
                END
            ");

            // Create sp_GetBackupHistory stored procedure
            migrationBuilder.Sql(@"
                CREATE PROCEDURE [dbo].[sp_GetBackupHistory]
                    @Top INT = 50
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT TOP (@Top)
                        Id,
                        BackupType,
                        BackupFileName,
                        BackupPath,
                        BackupSize,
                        Success,
                        ErrorMessage,
                        CreatedAt,
                        CreatedBy
                    FROM [dbo].[BackupLog]
                    ORDER BY CreatedAt DESC;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[sp_GetBackupHistory]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[sp_LogBackup]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[sp_BackupDatabase]");
            migrationBuilder.Sql("DROP TABLE IF EXISTS [dbo].[BackupLog]");
        }
    }
}
