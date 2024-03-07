using dotnet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class ServiceController : ControllerBase
    {
        private readonly Context _db;

        public ServiceController(Context context)
        {
            _db = context;
        }

        [HttpGet("get")]
        public async Task<Response<List<Service>>> GetItems()
        {
            try
            {
                List<Service> serviceList = await _db.Services.ToListAsync();
                if (serviceList != null)
                {
                    if (serviceList.Count > 0)
                    {
                        return new Response<List<Service>>(true, "Success: Acquired data.", serviceList);
                    }
                }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Service>>(false, "Failure: Database is empty.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Service>>(false, $"Server Failure: Unable to get data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpGet("get/id/{id}")]
        public async Task<Response<Service>> GetItemById(int id)
        {
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Service service = await _db.Services.FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (service == null)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Service>(false, "Failure: Data doesn't exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                return new Response<Service>(true, "Success: Acquired data.", service);
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Service>(false, $"Server Failure: Unable to get data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpGet("search/{search}")]
        public async Task<Response<List<Service>>> SearchItems(String search)
        {
            try
            {
                if (String.IsNullOrEmpty(search))
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<List<Service>>(false, "Failure: Enter a valid search string.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                List<Service> serviceList = await _db.Services.Where(x => x.Id.ToString().Contains(search) || x.Name.Contains(search) || x.Description.Contains(search)).OrderBy(x => x.Id).Take(10).ToListAsync();
                if (serviceList != null)
                {
                    if (serviceList.Count > 0)
                    {
                        return new Response<List<Service>>(true, "Success: Acquired data.", serviceList);
                    }
                }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Service>>(false, "Failure: Database is empty.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Service>>(false, $"Server Failure: Unable to get data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpPost("insert")]
        public async Task<Response<Service>> InsertItem(ServiceRequest serviceRequest)
        {
            try
            {
                Service service = new Service();
                service.Name = serviceRequest.Name;
                service.Description = serviceRequest.Description;
                await _db.Services.AddAsync(service);
                await _db.SaveChangesAsync();

                return new Response<Service>(true, "Success: Inserted data.", service);
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Service>(false, $"Server Failure: Unable to insert data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpPut("update/{id}")]
        public async Task<Response<Service>> UpdateItem(int id, ServiceRequest serviceRequest)
        {
            try
            {
                if (id != serviceRequest.Id)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Service>(false, "Failure: Id sent in body does not match object Id", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Service service = await _db.Services.FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (service == null)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Service>(false, "Failure: Data doesn't exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                service.Name = serviceRequest.Name;
                service.Description = serviceRequest.Description;
                await _db.SaveChangesAsync();

                return new Response<Service>(true, "Success: Updated data.", service);
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Service>(false, $"Server Failure: Unable to update data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<Response<Service>> DeleteItemById(int id)
        {
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Service service = await _db.Services.FindAsync(id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (service == null)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Service>(false, "Failure: Object doesn't exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                _db.Services.Remove(service);
                await _db.SaveChangesAsync();

                return new Response<Service>(true, "Success: Deleted data.", service);
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Service>(false, $"Server Failure: Unable to delete data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }
    }
}
