using dotnet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class LoginController : ControllerBase
    {
        private readonly Context _db;

        public LoginController(Context context)
        {
            _db = context;
        }

        [HttpGet("get")]
        public async Task<Response<List<Login>>> GetItems()
        {
            try
            {
                List<Login> loginList = await _db.Login.ToListAsync();
                if (loginList != null)
                {
                    if (loginList.Count > 0)
                    {
                        return new Response<List<Login>>(true, "Success: Acquired data.", loginList);
                    }
                }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Login>>(false, "Failure: Data does not exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Login>>(false, "Server Failure: Unable to get data. Because " + exception.Message, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpGet("get/id/{id}")]
        public async Task<Response<Login>> GetItemById(int id)
        {
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Login loginObject = await _db.Login.FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (loginObject == null)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Login>(false, "Failure: Data doesnot exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                return new Response<Login>(true, "Success: Acquired data.", loginObject);
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Login>(false, "Server Failure: Unable to get data. Because " + exception.Message, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpGet("search/{search}")]
        public async Task<Response<List<Login>>> SearchItems(String search)
        {
            try
            {
                if (String.IsNullOrEmpty(search))
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<List<Login>>(false, "Failure: Enter a valid search string.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                List<Login> loginList = await _db.Login.Where(x => x.Id.ToString().Contains(search) || x.UserId.ToString().Contains(search) || x.UserName.Contains(search) || x.Password.Contains(search)).OrderBy(x => x.Id).Take(10).ToListAsync();
                if (loginList != null)
                {
                    if (loginList.Count > 0)
                    {
                        return new Response<List<Login>>(true, "Success: Acquired data.", loginList);
                    }
                }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Login>>(false, "Failure: Database is empty.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Login>>(false, $"Server Failure: Unable to get data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpPost("insert")]
        public async Task<Response<Login>> InsertItem(Login loginObject)
        {
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                User userObject = await _db.Users.FirstOrDefaultAsync(x => x.Id == loginObject.UserId);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (userObject == null)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Login>(false, "Failure: Invalid user id or user does not exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                await _db.Login.AddAsync(loginObject);
                await _db.SaveChangesAsync();

                return new Response<Login>(true, "Success: Created object.", loginObject);
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Login>(false, "Server Failure: Unable to create object. Because " + exception.Message, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpPut("update/{id}")]
        public async Task<Response<Login>> UpdateItem(int id, Login loginObject)
        {
            if (id != loginObject.Id)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Login>(false, "Failure: Id sent in body does not match object Id", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Login existingItem = await _db.Login.FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (existingItem == null)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Login>(false, "Failure: Object doesnot exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                existingItem = loginObject;
                await _db.SaveChangesAsync();

                return new Response<Login>(true, "Success: Updated object.", loginObject);
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Login>(false, "API Failure: Unable to update object. Because " + exception.Message, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<Response<Login>> DeleteItemById(int id)
        {
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Login loginObject = await _db.Login.FindAsync(id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (loginObject == null)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Login>(false, "Failure: Object doesnot exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                _db.Login.Remove(loginObject);
                await _db.SaveChangesAsync();

                return new Response<Login>(true, "Success: Object deleted.", loginObject);
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Login>(false, "API Failure: Unable to delete object. Because " + exception.Message, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }
    }
}
