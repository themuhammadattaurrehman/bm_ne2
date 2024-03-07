using dotnet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class ProcedureController : ControllerBase
    {
        private readonly Context _db;

        public ProcedureController(Context context)
        {
            _db = context;
        }

        [HttpGet("get")]
        public async Task<Response<List<Procedure>>> GetItems()
        {
            try
            {
                List<Procedure> procedureList = await _db.Procedures.ToListAsync();
                if (procedureList != null)
                {
                    if (procedureList.Count > 0)
                    {
                        return new Response<List<Procedure>>(true, "Success: Acquired data.", procedureList);
                    }
                }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Procedure>>(false, "Failure: Database is empty.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Procedure>>(false, $"Server Failure: Unable to get data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpGet("get/id/{id}")]
        public async Task<Response<Procedure>> GetItemById(int id)
        {
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Procedure procedure = await _db.Procedures.FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (procedure == null)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Procedure>(false, "Failure: Data doesn't exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

                }
                return new Response<Procedure>(true, "Success: Acquired data.", procedure);
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Procedure>(false, $"Server Failure: Unable to get data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpGet("search/{search}")]
        public async Task<Response<List<Procedure>>> SearchItems(String search)
        {
            try
            {
                if (String.IsNullOrEmpty(search))
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<List<Procedure>>(false, "Failure: Enter a valid search string.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                List<Procedure> procedureList = await _db.Procedures.Where(x => x.Id.ToString().Contains(search) || x.Name.Contains(search) || x.ExecutantShare.ToString().Contains(search) || x.Charges.ToString().Contains(search)).OrderBy(x => x.Id).Take(10).ToListAsync();
                if (procedureList != null)
                {
                    if (procedureList.Count > 0)
                    {
                        return new Response<List<Procedure>>(true, "Success: Acquired data.", procedureList);
                    }
                }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Procedure>>(false, "Failure: Database is empty.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Procedure>>(false, $"Server Failure: Unable to get data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpPost("insert")]
        public async Task<Response<Procedure>> InsertItem(ProcedureRequest procedureRequest)
        {
            try
            {
                Procedure procedure = new Procedure();
                procedure.Name = procedureRequest.Name;
                procedure.Executant = procedureRequest.Executant;
                procedure.ExecutantShare = procedureRequest.ExecutantShare;
                procedure.Charges = procedureRequest.Charges;
                procedure.Consent = procedureRequest.Consent;
                await _db.Procedures.AddAsync(procedure);
                await _db.SaveChangesAsync();

                return new Response<Procedure>(true, "Success: Inserted data.", procedure);
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Procedure>(false, $"Server Failure: Unable to insert data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpPut("update/{id}")]
        public async Task<Response<Procedure>> UpdateItem(int id, ProcedureRequest procedureRequest)
        {
            try
            {
                if (id != procedureRequest.Id)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Procedure>(false, "Failure: Id sent in body does not match object Id", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Procedure procedure = await _db.Procedures.FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (procedure == null)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Procedure>(false, "Failure: Data doesn't exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                procedure.Name = procedureRequest.Name;
                procedure.Executant = procedureRequest.Executant;
                procedure.ExecutantShare = procedureRequest.ExecutantShare;
                procedure.Charges = procedureRequest.Charges;
                procedure.Consent = procedureRequest.Consent;
                await _db.SaveChangesAsync();

                return new Response<Procedure>(true, "Success: Updated data.", procedure);
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Procedure>(false, $"Server Failure: Unable to update data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<Response<Procedure>> DeleteItemById(int id)
        {
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Procedure procedure = await _db.Procedures.FindAsync(id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (procedure == null)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Procedure>(false, "Failure: Object doesn't exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                _db.Procedures.Remove(procedure);
                await _db.SaveChangesAsync();

                return new Response<Procedure>(true, "Success: Deleted data.", procedure);
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Procedure>(false, $"Server Failure: Unable to delete data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }
    }
}
