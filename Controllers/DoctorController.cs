using dotnet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class DoctorController : ControllerBase
    {
        private readonly Context _db;

        public DoctorController(Context context)
        {
            _db = context;
        }

        [HttpGet("get")]
        public async Task<Response<List<Doctor>>> GetItems()
        {
            try
            {
                List<Doctor> doctorList = await _db.Doctors.Include(x => x.User).Include(x => x.User.Qualifications).ToListAsync();
                if (doctorList != null)
                {
                    if (doctorList.Count > 0)
                    {
                        return new Response<List<Doctor>>(true, "Success: Acquired data.", doctorList);
                    }
                }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Doctor>>(false, "Failure: Database is empty.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Doctor>>(false, $"Server Failure: Unable to get data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpGet("get/id/{id}")]
        public async Task<Response<Doctor>> GetItemById(int id)
        {
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Doctor doctor = await _db.Doctors.Include(x => x.User).Include(x => x.User.Qualifications).FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (doctor == null)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Doctor>(false, "Failure: Data doesn't exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                return new Response<Doctor>(true, "Success: Acquired data.", doctor);
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Doctor>(false, $"Server Failure: Unable to get data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpGet("search/{search}")]
        public async Task<Response<List<Doctor>>> SearchItems(String search)
        {
            try
            {
                if (String.IsNullOrEmpty(search))
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<List<Doctor>>(false, "Failure: Enter a valid search string.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                List<Doctor> doctorList = await _db.Doctors.Where(x => x.Id.ToString().Contains(search) || x.UserId.ToString().Contains(search) ||
                x.ConsultationFee.ToString().Contains(search) || x.EmergencyConsultationFee.ToString().Contains(search) || x.ShareInFee.ToString().Contains(search) ||
                x.SpecialityType.Contains(search) || x.User.FirstName.Contains(search) || x.User.LastName.Contains(search) || x.User.FatherHusbandName.Contains(search) ||
                x.User.Gender.Contains(search) || x.User.Cnic.Contains(search) || x.User.Contact.Contains(search) || x.User.EmergencyContact.Contains(search) ||
                x.User.Email.Contains(search) || x.User.Address.Contains(search) || x.User.Experience.Contains(search) ||
                x.User.FloorNo.ToString().Contains(search)).OrderBy(x => x.Id).Take(10).Include(x => x.User).ToListAsync();
                if (doctorList != null)
                {
                    if (doctorList.Count > 0)
                    {
                        return new Response<List<Doctor>>(true, "Success: Acquired data.", doctorList);
                    }
                }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Doctor>>(false, "Failure: Database is empty.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Doctor>>(false, $"Server Failure: Unable to get data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpPost("insert")]
        public async Task<Response<Doctor>> InsertItem(DoctorRequest doctorRequest)
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                User user = new User();
                user.UserType = doctorRequest.UserType;
                user.FirstName = doctorRequest.FirstName;
                user.LastName = doctorRequest.LastName;
                user.FatherHusbandName = doctorRequest.FatherHusbandName;
                user.Gender = doctorRequest.Gender;
                user.Cnic = doctorRequest.Cnic;
                user.Contact = doctorRequest.Contact;
                user.EmergencyContact = doctorRequest.EmergencyContact;
                user.Email = doctorRequest.Email;
                user.Address = doctorRequest.Address;
                user.JoiningDate = doctorRequest.JoiningDate;
                user.FloorNo = doctorRequest.FloorNo;
                user.Experience = doctorRequest.Experience;
                user.DateOfBirth = doctorRequest.DateOfBirth;
                user.MaritalStatus = doctorRequest.MaritalStatus;
                user.Religion = doctorRequest.Religion;
                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();

                if (doctorRequest.QualificationList != null)
                {
                    if (doctorRequest.QualificationList.Count > 0)
                    {
                        foreach (QualificationRequest drQualification in doctorRequest.QualificationList)
                        {
                            Qualification qualification = new Qualification();
                            qualification.UserId = user.Id;
                            qualification.Certificate = drQualification.Certificate;
                            qualification.Description = drQualification.Description;
                            qualification.QualificationType = drQualification.QualificationType;
                            await _db.Qualifications.AddAsync(qualification);
                            await _db.SaveChangesAsync();
                        }
                    }
                }

                Doctor doctor = new Doctor();
                doctor.UserId = user.Id;
                doctor.ConsultationFee = doctorRequest.ConsultationFee;
                doctor.EmergencyConsultationFee = doctorRequest.EmergencyConsultationFee;
                doctor.ShareInFee = doctorRequest.ShareInFee;
                doctor.SpecialityType = doctorRequest.SpecialityType;
                await _db.Doctors.AddAsync(doctor);
                await _db.SaveChangesAsync();

                transaction.Commit();
                return new Response<Doctor>(true, "Success: Inserted data.", doctor);
            }
            catch (Exception exception)
            {
                transaction.Rollback();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Doctor>(false, $"Server Failure: Unable to insert data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpPut("update/{id}")]
        public async Task<Response<Doctor>> UpdateItem(int id, DoctorRequest doctorRequest)
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                if (id != doctorRequest.Id)
                {
                    transaction.Rollback();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Doctor>(false, "Failure: Id sent in body does not match object Id", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Doctor doctor = await _db.Doctors.Include(x => x.User.Qualifications).FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (doctor == null)
                {
                    transaction.Rollback();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Doctor>(false, $"Failure: Unable to update doctor {doctorRequest.FirstName}. Because Id is invalid. ", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                doctor.ConsultationFee = doctorRequest.ConsultationFee;
                doctor.EmergencyConsultationFee = doctorRequest.EmergencyConsultationFee;
                doctor.ShareInFee = doctorRequest.ShareInFee;
                doctor.SpecialityType = doctorRequest.SpecialityType;
                await _db.SaveChangesAsync();

                if (doctorRequest.QualificationList != null)
                {
                    if (doctorRequest.QualificationList.Count > 0)
                    {
                        foreach (QualificationRequest drQualification in doctorRequest.QualificationList)
                        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                            Qualification qualification = await _db.Qualifications.FirstOrDefaultAsync(x => x.Id == drQualification.Id && x.UserId == doctor.UserId);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                            if (qualification == null)
                            {
                                transaction.Rollback();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                                return new Response<Doctor>(false, $"Failure: Unable to update qualification {drQualification.Certificate}. Because Id is invalid. ", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                            }
                            qualification.Certificate = drQualification.Certificate;
                            qualification.Description = drQualification.Description;
                            qualification.QualificationType = drQualification.QualificationType;
                            await _db.SaveChangesAsync();
                        }
                    }
                }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                User user = await _db.Users.FirstOrDefaultAsync(x => x.Id == doctor.UserId);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (user == null)
                {
                    transaction.Rollback();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Doctor>(false, "Failure: Data doesn't exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                user.UserType = doctorRequest.UserType;
                user.FirstName = doctorRequest.FirstName;
                user.LastName = doctorRequest.LastName;
                user.FatherHusbandName = doctorRequest.FatherHusbandName;
                user.Gender = doctorRequest.Gender;
                user.Cnic = doctorRequest.Cnic;
                user.Contact = doctorRequest.Contact;
                user.EmergencyContact = doctorRequest.EmergencyContact;
                user.Email = doctorRequest.Email;
                user.Address = doctorRequest.Address;
                user.JoiningDate = doctorRequest.JoiningDate;
                user.FloorNo = doctorRequest.FloorNo;
                user.Experience = doctorRequest.Experience;
                user.DateOfBirth = doctorRequest.DateOfBirth;
                user.MaritalStatus = doctorRequest.MaritalStatus;
                user.Religion = doctorRequest.Religion;
                await _db.SaveChangesAsync();

                transaction.Commit();
                return new Response<Doctor>(true, "Success: Updated data.", doctor);
            }
            catch (Exception exception)
            {
                transaction.Rollback();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Doctor>(false, $"Server Failure: Unable to update data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<Response<Doctor>> DeleteItemById(int id)
        {
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Doctor doctor = await _db.Doctors.FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (doctor == null)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Doctor>(false, $"Failure: Object with id={id} does not exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                User user = await _db.Users.FindAsync(doctor.UserId);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (user == null)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Doctor>(false, $"Failure: Object with id={id} does not exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();

                return new Response<Doctor>(true, "Success: Deleted data.", doctor);
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Doctor>(false, $"Server Failure: Unable to delete data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }
    }
}
