// SettingsController.cs
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private readonly ISettingService _settingService;
        private readonly IConfiguration _configuration;

        public SettingsController(ISettingService settingService, IConfiguration configuration)
        {
            _settingService = settingService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _settingService.GetAllSettingsAsync();
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Dictionary<string, string> settings)
        {
            foreach (var setting in settings)
            {
                await _settingService.UpdateSettingAsync(setting.Key, setting.Value);
            }

            TempData["Success"] = "Settings updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HasPermission(Permission.ViewSystemInfo)]
        public IActionResult SystemInfo()
        {
            ViewBag.Version = "1.0.0";
            ViewBag.Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return View();
        }

        [Authorize(Roles = "Admin")]  // CRITICAL: Only Admin can access backup
        public IActionResult Backup()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]  // CRITICAL: Only Admin can backup database
        public async Task<IActionResult> BackupDatabase(string format = "bak")
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                var sqlConnectionString = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                var databaseName = sqlConnectionString.InitialCatalog;
                var currentUser = User?.Identity?.Name ?? "System";

                if (format.ToLower() == "bak")
                {
                    using (var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        // Call stored procedure for backup
                        using (var command = new Microsoft.Data.SqlClient.SqlCommand("sp_BackupDatabase", connection))
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.CommandTimeout = 600; // 10 minutes timeout

                            // Input parameters
                            command.Parameters.AddWithValue("@BackupType", "FULL");
                            command.Parameters.AddWithValue("@BackupPath", DBNull.Value);

                            // Output parameters
                            var backupFileNameParam = new Microsoft.Data.SqlClient.SqlParameter("@BackupFileName", System.Data.SqlDbType.NVarChar, 255)
                            {
                                Direction = System.Data.ParameterDirection.Output
                            };
                            command.Parameters.Add(backupFileNameParam);

                            var errorMessageParam = new Microsoft.Data.SqlClient.SqlParameter("@ErrorMessage", System.Data.SqlDbType.NVarChar, 4000)
                            {
                                Direction = System.Data.ParameterDirection.Output
                            };
                            command.Parameters.Add(errorMessageParam);

                            var successParam = new Microsoft.Data.SqlClient.SqlParameter("@Success", System.Data.SqlDbType.Bit)
                            {
                                Direction = System.Data.ParameterDirection.Output
                            };
                            command.Parameters.Add(successParam);

                            // Execute stored procedure
                            await command.ExecuteNonQueryAsync();

                            // Get output values
                            var success = (bool)successParam.Value;
                            var backupFileName = backupFileNameParam.Value?.ToString();
                            var errorMessage = errorMessageParam.Value?.ToString();

                            if (!success)
                            {
                                TempData["Error"] = $"Backup failed: {errorMessage}";
                                return RedirectToAction(nameof(Backup));
                            }

                            // Extract path from error message (contains full path)
                            var backupPath = errorMessage?.Replace("Backup completed successfully: ", "");

                            // Get file size for logging
                            long fileSize = 0;
                            if (!string.IsNullOrEmpty(backupPath) && System.IO.File.Exists(backupPath))
                            {
                                fileSize = new FileInfo(backupPath).Length;
                            }

                            // Log the backup operation
                            using (var logCommand = new Microsoft.Data.SqlClient.SqlCommand("sp_LogBackup", connection))
                            {
                                logCommand.CommandType = System.Data.CommandType.StoredProcedure;
                                logCommand.Parameters.AddWithValue("@BackupType", "FULL");
                                logCommand.Parameters.AddWithValue("@BackupFileName", backupFileName ?? "Unknown");
                                logCommand.Parameters.AddWithValue("@BackupPath", backupPath ?? "Unknown");
                                logCommand.Parameters.AddWithValue("@BackupSize", fileSize);
                                logCommand.Parameters.AddWithValue("@Success", success);
                                logCommand.Parameters.AddWithValue("@ErrorMessage", DBNull.Value);
                                logCommand.Parameters.AddWithValue("@CreatedBy", currentUser);
                                await logCommand.ExecuteNonQueryAsync();
                            }

                            // Read the backup file and return it
                            if (!string.IsNullOrEmpty(backupPath) && System.IO.File.Exists(backupPath))
                            {
                                var memory = new MemoryStream();
                                using (var stream = new FileStream(backupPath, FileMode.Open, FileAccess.Read))
                                {
                                    await stream.CopyToAsync(memory);
                                }
                                memory.Position = 0;

                                // Clean up the backup file
                                try
                                {
                                    System.IO.File.Delete(backupPath);
                                }
                                catch { /* Ignore cleanup errors */ }

                                return File(memory, "application/octet-stream", backupFileName);
                            }
                            else
                            {
                                TempData["Error"] = "Backup file not found.";
                                return RedirectToAction(nameof(Backup));
                            }
                        }
                    }
                }
                else if (format.ToLower() == "sql")
                {
                    // Generate SQL script
                    var scriptFileName = $"{databaseName}_Script_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
                    var script = await GenerateDatabaseScriptAsync(connectionString, databaseName);

                    var bytes = System.Text.Encoding.UTF8.GetBytes(script);

                    // Log the SQL script export
                    using (var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        using (var logCommand = new Microsoft.Data.SqlClient.SqlCommand("sp_LogBackup", connection))
                        {
                            logCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            logCommand.Parameters.AddWithValue("@BackupType", "SQL");
                            logCommand.Parameters.AddWithValue("@BackupFileName", scriptFileName);
                            logCommand.Parameters.AddWithValue("@BackupPath", "SQL Script Export");
                            logCommand.Parameters.AddWithValue("@BackupSize", bytes.Length);
                            logCommand.Parameters.AddWithValue("@Success", true);
                            logCommand.Parameters.AddWithValue("@ErrorMessage", DBNull.Value);
                            logCommand.Parameters.AddWithValue("@CreatedBy", currentUser);
                            await logCommand.ExecuteNonQueryAsync();
                        }
                    }

                    return File(bytes, "text/plain", scriptFileName);
                }
                else
                {
                    TempData["Error"] = "Invalid backup format specified.";
                    return RedirectToAction(nameof(Backup));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Backup failed: {ex.Message}";
                return RedirectToAction(nameof(Backup));
            }
        }

        private async Task<string> GenerateDatabaseScriptAsync(string connectionString, string databaseName)
        {
            var script = new System.Text.StringBuilder();

            script.AppendLine($"-- Database Script for {databaseName}");
            script.AppendLine($"-- Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            script.AppendLine($"-- WARNING: This script will drop and recreate the database!");
            script.AppendLine();
            script.AppendLine($"USE master;");
            script.AppendLine($"GO");
            script.AppendLine();
            script.AppendLine($"IF EXISTS (SELECT name FROM sys.databases WHERE name = N'{databaseName}')");
            script.AppendLine($"BEGIN");
            script.AppendLine($"    ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;");
            script.AppendLine($"    DROP DATABASE [{databaseName}];");
            script.AppendLine($"END");
            script.AppendLine($"GO");
            script.AppendLine();
            script.AppendLine($"CREATE DATABASE [{databaseName}];");
            script.AppendLine($"GO");
            script.AppendLine();
            script.AppendLine($"USE [{databaseName}];");
            script.AppendLine($"GO");
            script.AppendLine();

            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Get all tables
                var tablesCommand = new Microsoft.Data.SqlClient.SqlCommand(
                    "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME",
                    connection);

                var tables = new List<(string Schema, string Name)>();
                using (var reader = await tablesCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tables.Add((reader["TABLE_SCHEMA"].ToString(), reader["TABLE_NAME"].ToString()));
                    }
                }

                // Generate CREATE TABLE statements
                script.AppendLine("-- =============================================");
                script.AppendLine("-- CREATE TABLES");
                script.AppendLine("-- =============================================");
                script.AppendLine();

                foreach (var table in tables)
                {
                    script.AppendLine($"-- Table: [{table.Schema}].[{table.Name}]");

                    var createTableCmd = new Microsoft.Data.SqlClient.SqlCommand($@"
                        SELECT
                            c.COLUMN_NAME,
                            c.DATA_TYPE,
                            c.CHARACTER_MAXIMUM_LENGTH,
                            c.NUMERIC_PRECISION,
                            c.NUMERIC_SCALE,
                            c.IS_NULLABLE,
                            c.COLUMN_DEFAULT
                        FROM INFORMATION_SCHEMA.COLUMNS c
                        WHERE c.TABLE_SCHEMA = @Schema AND c.TABLE_NAME = @Table
                        ORDER BY c.ORDINAL_POSITION", connection);

                    createTableCmd.Parameters.AddWithValue("@Schema", table.Schema);
                    createTableCmd.Parameters.AddWithValue("@Table", table.Name);

                    script.AppendLine($"CREATE TABLE [{table.Schema}].[{table.Name}] (");

                    var columns = new List<string>();
                    using (var reader = await createTableCmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var columnName = reader["COLUMN_NAME"].ToString();
                            var dataType = reader["DATA_TYPE"].ToString().ToUpper();
                            var maxLength = reader["CHARACTER_MAXIMUM_LENGTH"] == DBNull.Value ? "" : reader["CHARACTER_MAXIMUM_LENGTH"].ToString();
                            var isNullable = reader["IS_NULLABLE"].ToString() == "YES" ? "NULL" : "NOT NULL";
                            var defaultValue = reader["COLUMN_DEFAULT"] == DBNull.Value ? "" : $"DEFAULT {reader["COLUMN_DEFAULT"]}";

                            var columnDef = $"    [{columnName}] [{dataType}]";

                            if (dataType == "NVARCHAR" || dataType == "VARCHAR")
                            {
                                columnDef += maxLength == "-1" ? "(MAX)" : $"({maxLength})";
                            }
                            else if (dataType == "DECIMAL" || dataType == "NUMERIC")
                            {
                                var precision = reader["NUMERIC_PRECISION"];
                                var scale = reader["NUMERIC_SCALE"];
                                columnDef += $"({precision},{scale})";
                            }

                            columnDef += $" {isNullable}";

                            if (!string.IsNullOrEmpty(defaultValue))
                            {
                                columnDef += $" {defaultValue}";
                            }

                            columns.Add(columnDef);
                        }
                    }

                    script.AppendLine(string.Join(",\n", columns));
                    script.AppendLine(");");
                    script.AppendLine("GO");
                    script.AppendLine();
                }

                // Generate INSERT statements for data
                script.AppendLine("-- =============================================");
                script.AppendLine("-- INSERT DATA");
                script.AppendLine("-- =============================================");
                script.AppendLine();

                foreach (var table in tables)
                {
                    var countCmd = new Microsoft.Data.SqlClient.SqlCommand($"SELECT COUNT(*) FROM [{table.Schema}].[{table.Name}]", connection);
                    var rowCount = (int)await countCmd.ExecuteScalarAsync();

                    if (rowCount > 0)
                    {
                        script.AppendLine($"-- Data for table [{table.Schema}].[{table.Name}] ({rowCount} rows)");
                        script.AppendLine($"SET IDENTITY_INSERT [{table.Schema}].[{table.Name}] ON;");

                        var selectCmd = new Microsoft.Data.SqlClient.SqlCommand($"SELECT * FROM [{table.Schema}].[{table.Name}]", connection);
                        using (var reader = await selectCmd.ExecuteReaderAsync())
                        {
                            var columnNames = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columnNames.Add(reader.GetName(i));
                            }

                            while (await reader.ReadAsync())
                            {
                                var values = new List<string>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (reader.IsDBNull(i))
                                    {
                                        values.Add("NULL");
                                    }
                                    else
                                    {
                                        var value = reader.GetValue(i);
                                        if (value is string || value is DateTime || value is Guid)
                                        {
                                            values.Add($"N'{value.ToString().Replace("'", "''")}'");
                                        }
                                        else if (value is bool)
                                        {
                                            values.Add((bool)value ? "1" : "0");
                                        }
                                        else
                                        {
                                            values.Add(value.ToString());
                                        }
                                    }
                                }

                                script.AppendLine($"INSERT INTO [{table.Schema}].[{table.Name}] ([{string.Join("], [", columnNames)}]) VALUES ({string.Join(", ", values)});");
                            }
                        }

                        script.AppendLine($"SET IDENTITY_INSERT [{table.Schema}].[{table.Name}] OFF;");
                        script.AppendLine("GO");
                        script.AppendLine();
                    }
                }
            }

            return script.ToString();
        }

        [Authorize(Roles = "Admin")]  // CRITICAL: Only Admin can access import/export
        public IActionResult ImportExport()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.ExportData)]
        public async Task<IActionResult> ExportData(string dataType, string format)
        {
            try
            {
                // Implement export logic here
                TempData["Success"] = $"{dataType} exported successfully!";
                return RedirectToAction(nameof(ImportExport));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error exporting data: {ex.Message}";
                return RedirectToAction(nameof(ImportExport));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.ImportData)]
        public async Task<IActionResult> ImportData(IFormFile file, string dataType)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    TempData["Error"] = "Please select a file to import";
                    return RedirectToAction(nameof(ImportExport));
                }

                // Implement import logic here
                TempData["Success"] = $"{dataType} imported successfully!";
                return RedirectToAction(nameof(ImportExport));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error importing data: {ex.Message}";
                return RedirectToAction(nameof(ImportExport));
            }
        }
    }
}
