using System.Data;
using Microsoft.Data.SqlClient;
using ServiceBookingSystemAPI.Models;

namespace ServiceBookingSystem.Repository
{
    public class DBAccessLayer
    {
        private readonly IConfiguration _configuration;

        public DBAccessLayer(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        #region GetBookedOrdersAsync

        public async Task<List<Booking>> GetBookedOrdersAsync()
        {
            List<Booking> bookedOrders = new List<Booking>();

            // Get the connection string from configuration
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("GetBookedOrders", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var bookedOrder = new Booking
                                {
                                    BookingDate = reader["BookingDate"] != DBNull.Value ? Convert.ToDateTime(reader["BookingDate"]) : DateTime.MinValue,
                                    Status = reader["Status"]?.ToString() ?? "Pending",

                                    User = new User
                                    {
                                        Name = reader["UserName"]?.ToString() ?? "Unknown",
                                    },

                                    Service = new Service
                                    {
                                        Name = reader["ServiceName"]?.ToString() ?? "Unknown",
                                    }
                                };

                                bookedOrders.Add(bookedOrder);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }

            return bookedOrders;
        }

        #endregion
    }
}