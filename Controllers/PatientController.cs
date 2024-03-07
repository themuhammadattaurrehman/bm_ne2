using dotnet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly Context _db;

        public PatientController(Context context)
        {
            _db = context;
        }

        [HttpGet("get")]
        public async Task<Response<List<Patient>>> GetItems()
        {
            try
            {
                List<Patient> patientList = await _db.Patients.Include(x => x.User).ToListAsync();
                if (patientList != null)
                {
                    if (patientList.Count > 0)
                    {
                        return new Response<List<Patient>>(true, "Success: Acquired data.", patientList);
                    }
                }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Patient>>(false, "Failure: Data does not exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Patient>>(false, $"Server Failure: Unable to get data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpGet("get/id/{id}")]
        public async Task<Response<Patient>> GetItemById(int id)
        {
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Patient patient = await _db.Patients.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (patient != null)
                {
                    return new Response<Patient>(true, "Success: Acquired data.", patient);
                }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Patient>(false, "Failure: Data doesnot exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Patient>(false, $"Server Failure: Unable to get object. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpGet("search/{search}")]
        public async Task<Response<List<Patient>>> SearchItems(String search)
        {
            try
            {
                if (String.IsNullOrEmpty(search))
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<List<Patient>>(false, "Failure: Enter a valid search string.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                List<Patient> patientList = await _db.Patients.Where(x => x.Id.ToString().Contains(search) ||
                x.UserId.ToString().Contains(search) || x.BirthPlace.Contains(search) || x.Type.Contains(search) ||
                x.ExternalId.Contains(search) || x.BloodGroup.Contains(search) || x.ClinicSite.Contains(search) ||
                x.ReferredBy.Contains(search) || x.Guardian.Contains(search) || x.PaymentProfile.Contains(search) ||
                x.Description.Contains(search) || x.User.FirstName.Contains(search) || x.User.LastName.Contains(search) ||
                x.User.FatherHusbandName.Contains(search) || x.User.Gender.Contains(search) || x.User.Cnic.Contains(search) ||
                x.User.Contact.Contains(search) || x.User.EmergencyContact.Contains(search) || x.User.Email.Contains(search) ||
                x.User.Address.Contains(search) || x.User.Experience.Contains(search) || x.User.FloorNo.ToString().Contains(search)).
                OrderBy(x => x.Id).Take(10).Include(x => x.User).ToListAsync();
                if (patientList != null)
                {
                    if (patientList.Count > 0)
                    {
                        return new Response<List<Patient>>(true, "Success: Acquired data.", patientList);
                    }
                }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Patient>>(false, "Failure: Database is empty.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Patient>>(false, $"Server Failure: Unable to get data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpPost("insert")]
        public async Task<Response<Patient>> InsertItem(PatientRequest patientRequest)
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                User user = new User();
                user.UserType = patientRequest.UserType;
                user.FirstName = patientRequest.FirstName;
                user.LastName = patientRequest.LastName;
                user.FatherHusbandName = patientRequest.FatherHusbandName;
                user.Gender = patientRequest.Gender;
                user.Cnic = patientRequest.Cnic;
                user.Contact = patientRequest.Contact;
                user.EmergencyContact = patientRequest.EmergencyContact;
                user.Email = patientRequest.Email;
                user.Address = patientRequest.Address;
                user.JoiningDate = patientRequest.JoiningDate;
                user.FloorNo = patientRequest.FloorNo;
                user.Experience = patientRequest.Experience;
                user.DateOfBirth = patientRequest.DateOfBirth;
                user.MaritalStatus = patientRequest.MaritalStatus;
                user.Religion = patientRequest.Religion;
                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();

                Patient patient = new Patient();
                patient.UserId = user.Id;
                patient.BirthPlace = patientRequest.BirthPlace;
                patient.Type = patientRequest.Type;
                patient.ExternalId = patientRequest.ExternalId;
                patient.BloodGroup = patientRequest.BloodGroup;
                patient.ClinicSite = patientRequest.ClinicSite;
                patient.ReferredBy = patientRequest.ReferredBy;
                patient.ReferredDate = patientRequest.ReferredDate;
                patient.Guardian = patientRequest.Guardian;
                patient.PaymentProfile = patientRequest.PaymentProfile;
                patient.Description = patientRequest.Description;
                await _db.Patients.AddAsync(patient);
                await _db.SaveChangesAsync();

                Appointment appointment = new Appointment();
                appointment.PatientId = patient.Id;
                appointment.DoctorId = patientRequest.DoctorId;
                appointment.ReceptionistId = patientRequest.ReceptionistId;
                appointment.Code = patientRequest.AppointmentCode;
                appointment.Date = DateTime.UtcNow;
                appointment.ConsultationDate = patientRequest.ConsultationDate;
                appointment.Type = patientRequest.AppointmentType;
                appointment.PatientCategory = patientRequest.Category;
                await _db.Appointments.AddAsync(appointment);
                await _db.SaveChangesAsync();

                transaction.Commit();
                return new Response<Patient>(true, "Success: Created object.", patient);
            }
            catch (Exception exception)
            {
                transaction.Rollback();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Patient>(false, $"Server Failure: Unable to insert object. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpPut("update/{id}")]
        public async Task<Response<Patient>> UpdateItem(int id, PatientRequest patientRequest)
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                if (id != patientRequest.Id)
                {
                    transaction.Rollback();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Patient>(false, "Failure: Id sent in body does not match object Id", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Patient patient = await _db.Patients.FirstOrDefaultAsync(x => x.Id == id); ;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (patient == null)
                {
                    transaction.Rollback();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Patient>(false, $"Failure: Unable to update patient {patientRequest.FirstName}. Because Id is invalid. ", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                patient.BirthPlace = patientRequest.BirthPlace;
                patient.Type = patientRequest.Type;
                patient.ExternalId = patientRequest.ExternalId;
                patient.BloodGroup = patientRequest.BloodGroup;
                patient.ClinicSite = patientRequest.ClinicSite;
                patient.ReferredBy = patientRequest.ReferredBy;
                patient.ReferredDate = patientRequest.ReferredDate;
                patient.Guardian = patientRequest.Guardian;
                patient.PaymentProfile = patientRequest.PaymentProfile;
                patient.Description = patientRequest.Description;
                await _db.SaveChangesAsync();

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                User user = await _db.Users.FirstOrDefaultAsync(x => x.Id == patient.UserId);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (user == null)
                {
                    transaction.Rollback();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Patient>(false, "Failure: Data does not exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                user.UserType = patientRequest.UserType;
                user.FirstName = patientRequest.FirstName;
                user.LastName = patientRequest.LastName;
                user.FatherHusbandName = patientRequest.FatherHusbandName;
                user.Gender = patientRequest.Gender;
                user.Cnic = patientRequest.Cnic;
                user.Contact = patientRequest.Contact;
                user.EmergencyContact = patientRequest.EmergencyContact;
                user.Email = patientRequest.Email;
                user.Address = patientRequest.Address;
                user.JoiningDate = patientRequest.JoiningDate;
                user.FloorNo = patientRequest.FloorNo;
                user.Experience = patientRequest.Experience;
                user.DateOfBirth = patientRequest.DateOfBirth;
                user.MaritalStatus = patientRequest.MaritalStatus;
                user.Religion = patientRequest.Religion;
                await _db.SaveChangesAsync();

                transaction.Commit();
                return new Response<Patient>(true, "Success: Updated object.", patient);
            }
            catch (Exception exception)
            {
                transaction.Rollback();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Patient>(false, $"Server Failure: Unable to update object. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<Response<Patient>> DeleteItemById(int id)
        {
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Patient patient = await _db.Patients.FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (patient == null)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Patient>(false, $"Failure: Object with id={id} does not exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                User user = await _db.Users.FindAsync(patient.UserId);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (user == null)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Patient>(false, $"Failure: Object with id={id} does not exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();

                return new Response<Patient>(true, "Success: Deleted data.", patient);
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Patient>(false, $"Server Failure: Unable to delete object. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }
    }
}
