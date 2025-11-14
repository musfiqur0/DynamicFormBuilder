using DynamicFormBuilder.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DynamicFormBuilder.Data
{
    public class FormRepository
    {
        private readonly string _connectionString;

        public FormRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentException("DefaultConnection not found in configuration.");
        }
        public async Task<(List<Form> Forms, int TotalCount)> GetFormsPagedAsync(int pageNumber, int pageSize)
        {
            var forms = new List<Form>();
            int totalCount = 0;

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var cmd = new SqlCommand("sp_GetFormsPaged", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            await using var reader = await cmd.ExecuteReaderAsync();

            // First result: total count
            if (await reader.ReadAsync())
            {
                totalCount = reader.GetInt32(reader.GetOrdinal("TotalCount"));
            }

            // Move to second result set: page rows
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    var form = new Form
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("FormId")),
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                    };
                    forms.Add(form);
                }
            }

            return (forms, totalCount);
        }

        public async Task<Form?> GetFormByIdAsync(int id)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand("sp_GetFormById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@FormId", id);
            await using var reader = await cmd.ExecuteReaderAsync();
            Form? form = null;
            if (await reader.ReadAsync())
            {
                form = new Form
                {
                    Id = reader.GetInt32(reader.GetOrdinal("FormId")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                };
            }
            if (form != null && await reader.NextResultAsync())
            {
                form.Fields = new List<FormField>();
                while (await reader.ReadAsync())
                {
                    var field = new FormField
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("FieldId")),
                        FormId = reader.GetInt32(reader.GetOrdinal("FormId")),
                        Label = reader.GetString(reader.GetOrdinal("Label")),
                        IsRequired = reader.GetBoolean(reader.GetOrdinal("IsRequired")),
                        SelectedOption = reader.GetString(reader.GetOrdinal("SelectedOption"))
                    };
                    form.Fields.Add(field);
                }
            }
            return form;
        }
        public async Task<int> SaveFormAsync(Form form)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand("sp_SaveForm", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Title", form.Title);
            var fieldsTable = new DataTable();
            fieldsTable.Columns.Add("Label", typeof(string));
            fieldsTable.Columns.Add("IsRequired", typeof(bool));
            fieldsTable.Columns.Add("SelectedOption", typeof(string));
            foreach (var field in form.Fields)
            {
                fieldsTable.Rows.Add(field.Label, field.IsRequired, field.SelectedOption);
            }
            var tvpParam = cmd.Parameters.AddWithValue("@Fields", fieldsTable);
            tvpParam.SqlDbType = SqlDbType.Structured;
            tvpParam.TypeName = "dbo.FormFieldTableType";
            var outputId = new SqlParameter("@NewFormId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputId);

            await cmd.ExecuteNonQueryAsync();
            return (int)(outputId.Value ?? 0);
        }
    }
}