using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using HomeLib.Models;

namespace HomeLib.Data
{
    public class BookRepository : IBookRepository
    {
        private readonly string _connectionString;

        public BookRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            var books = new List<Book>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("usp_GetAllBooks", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            books.Add(MapToBook(reader));
                        }
                    }
                }
            }

            return books;
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("usp_GetBookById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@BookID", id);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapToBook(reader);
                        }
                    }
                }
            }

            return null;
        }

        public async Task<int> CreateBookAsync(Book book)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("usp_CreateBook", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // required
                    command.Parameters.AddWithValue("@Title", book.Title);
                    command.Parameters.AddWithValue("@Author", book.Author);
                    command.Parameters.AddWithValue("@PublishYear", book.PublishYear);
                    command.Parameters.AddWithValue("@PageCount", book.PageCount);

                    // not required
                    command.Parameters.AddWithValue("@Genre", 
                        string.IsNullOrEmpty(book.Genre) ? (object)DBNull.Value : book.Genre);
                    command.Parameters.AddWithValue("@Publisher", 
                        string.IsNullOrEmpty(book.Publisher) ? (object)DBNull.Value : book.Publisher);
                    command.Parameters.AddWithValue("@Language", 
                        string.IsNullOrEmpty(book.Language) ? (object)DBNull.Value : book.Language);
                    command.Parameters.AddWithValue("@TableOfContents", 
                        string.IsNullOrEmpty(book.TableOfContents) ? (object)DBNull.Value : book.TableOfContents);

                    var outputParam = new SqlParameter("@NewBookID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    return Convert.ToInt32(outputParam.Value);
                }
            }
        }

        public async Task<bool> UpdateBookAsync(Book book)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("usp_UpdateBook", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@BookID", book.BookID);
                    command.Parameters.AddWithValue("@Title", book.Title);
                    command.Parameters.AddWithValue("@Author", book.Author);
                    command.Parameters.AddWithValue("@Genre", 
                        string.IsNullOrEmpty(book.Genre) ? (object)DBNull.Value : book.Genre);
                    command.Parameters.AddWithValue("@PublishYear", book.PublishYear);
                    command.Parameters.AddWithValue("@Publisher", 
                        string.IsNullOrEmpty(book.Publisher) ? (object)DBNull.Value : book.Publisher);
                    command.Parameters.AddWithValue("@PageCount", book.PageCount);
                    command.Parameters.AddWithValue("@Language", 
                        string.IsNullOrEmpty(book.Language) ? (object)DBNull.Value : book.Language);
                    command.Parameters.AddWithValue("@TableOfContents", 
                        string.IsNullOrEmpty(book.TableOfContents) ? (object)DBNull.Value : book.TableOfContents);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("usp_DeleteBook", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@BookID", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
        }

        private Book MapToBook(SqlDataReader reader)
        {
            return new Book
            {
                BookID = GetInt32Safe(reader, "BookID"),
                Title = GetStringSafe(reader, "Title"),
                Author = GetStringSafe(reader, "Author"),
                Genre = GetStringNullable(reader, "Genre"),
                PublishYear = GetInt32Safe(reader, "PublishYear"),
                Publisher = GetStringNullable(reader, "Publisher"),
                PageCount = GetInt32Safe(reader, "PageCount"),
                Language = GetStringNullable(reader, "Language"),
                DateAdded = GetDateTimeSafe(reader, "DateAdded"),
                TableOfContents = GetStringNullable(reader, "TableOfContents")
            };
        }

        private string GetStringSafe(SqlDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetStringNullable(SqlDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
            }
            catch
            {
                return null;
            }
        }

        private int GetInt32Safe(SqlDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);
            }
            catch
            {
                return 0;
            }
        }

        private DateTime GetDateTimeSafe(SqlDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? DateTime.MinValue : reader.GetDateTime(ordinal);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }
}