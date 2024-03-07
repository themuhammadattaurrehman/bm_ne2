using dotnet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly Context _db;

        public InvoiceController(Context context)
        {
            _db = context;
        }

        [HttpGet("get")]
        public async Task<Response<List<Invoice>>> GetItems()
        {
            try
            {
                List<Invoice> invoiceList = await _db.Invoices.ToListAsync();
                if (invoiceList != null)
                {
                    if (invoiceList.Count > 0)
                    {
                        return new Response<List<Invoice>>(true, "Success: Acquired data.", invoiceList);
                    }
                }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Invoice>>(false, "Failure: Data does not exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Invoice>>(false, $"Server Failure: Unable to get data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpGet("get/id/{id}")]
        public async Task<Response<Invoice>> GetItemById(int id)
        {
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Invoice invoice = await _db.Invoices.FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (invoice != null)
                {
                    return new Response<Invoice>(true, "Success: Acquired data.", invoice);
                }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Invoice>(false, "Failure: Data doesnot exist.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Invoice>(false, $"Server Failure: Unable to get data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpGet("search/{search}")]
        public async Task<Response<List<Invoice>>> SearchItems(String search)
        {
            try
            {
                if (String.IsNullOrEmpty(search))
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<List<Invoice>>(false, "Failure: Enter a valid search string.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                List<Invoice> invoiceList = await _db.Invoices.Where(x => x.Id.ToString().Contains(search) ||
                x.AppointmentId.ToString().Contains(search) || x.DoctorId.ToString().Contains(search) ||
                x.PatientId.ToString().Contains(search) || x.Date.ToString().Contains(search) ||
                x.CheckupType.Contains(search) || x.CheckupFee.ToString().Contains(search) ||
                x.PaymentType.Contains(search) || x.Disposibles.ToString().Contains(search) ||
                x.GrossAmount.ToString().Contains(search)).OrderBy(x => x.Id).Take(10).ToListAsync();
                if (invoiceList != null)
                {
                    if (invoiceList.Count > 0)
                    {
                        return new Response<List<Invoice>>(true, "Success: Acquired data.", invoiceList);
                    }
                }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Invoice>>(false, "Failure: Database is empty.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Invoice>>(false, $"Server Failure: Unable to get data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpPost("post/search")]
        public async Task<Response<List<Invoice>>> SearchItemsByPost(InvoiceSearchRequest request)
        {
            try
            {
#pragma warning disable CS8073 // The result of the expression is always the same since a value of this type is never equal to 'null'
#pragma warning disable CS8073 // The result of the expression is always the same since a value of this type is never equal to 'null'
                if (request.Search.Length < 1)
                {
                    List<Invoice> invoiceList = await _db.Invoices.Where(x => (x.Date >= request.FromDate.Date && x.Date < request.ToDate.Date.AddDays(1))).
                    Include(x => x.Doctor).Include(x => x.Doctor.User).Include(x => x.Patient).Include(x => x.Patient.User).Include(x => x.Receipt).ToListAsync();
                    if (invoiceList != null)
                    {
                        if (invoiceList.Count > 0)
                        {
                            return new Response<List<Invoice>>(true, "Success: Acquired data.", invoiceList);
                        }
                    }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<List<Invoice>>(false, "Failure: Database is empty.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                else if (request.FromDate != null && request.ToDate != null && request.Search != null)
                {
                    if (request.Search.Length > 0)
                    {
                        List<Invoice> invoiceList = await _db.Invoices.Where(x => (x.Id.ToString() == request.Search) && (x.Date >= request.FromDate.Date &&
                        x.Date < request.ToDate.Date.AddDays(1))).Include(x => x.Doctor).Include(x => x.Doctor.User).
                        Include(x => x.Patient).Include(x => x.Patient.User).Include(x => x.Receipt).ToListAsync();
                        if (invoiceList != null)
                        {
                            if (invoiceList.Count > 0)
                            {
                                return new Response<List<Invoice>>(true, "Success: Acquired data.", invoiceList);
                            }
                        }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                        return new Response<List<Invoice>>(false, "Failure: Database is empty.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    }
                }
#pragma warning restore CS8073 // The result of the expression is always the same since a value of this type is never equal to 'null'
#pragma warning restore CS8073 // The result of the expression is always the same since a value of this type is never equal to 'null'
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Invoice>>(false, "Failure: Any of the following is missing. 'Search' 'FromDate' 'ToDate'", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Invoice>>(false, $"Server Failure: Unable to get data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpPost("insert")]
        public async Task<Response<Invoice>> InsertItem(InvoiceRequest invoiceRequest)
        {
            using var transaction = _db.Database.BeginTransaction();
            int appointmentId = invoiceRequest.AppointmentId;
            DateTime appointmentDate = invoiceRequest.AppointmentDate;
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Patient patient = await _db.Patients.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == invoiceRequest.PatientId);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (patient == null)
                {
                    transaction.Rollback();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Invoice>(false, $"Failure: Unable to create invoice. Because patient belonging to id = {invoiceRequest.PatientId} was not found.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Doctor doctor = await _db.Doctors.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == invoiceRequest.DoctorId);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (doctor == null)
                {
                    transaction.Rollback();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new Response<Invoice>(false, $"Failure: Unable to create invoice. Because doctor belonging to id = {invoiceRequest.DoctorId} was not found.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Appointment appointment = await _db.Appointments.FirstOrDefaultAsync(x => x.Id == appointmentId);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (appointment == null)
                {
                    if (invoiceRequest.AppointmentPatientCategory.ToLower() == "online" || invoiceRequest.AppointmentPatientCategory.ToLower() == "admitted")
                    {
                        transaction.Rollback();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                        return new Response<Invoice>(false, $"Failure: Unable to create invoice. Because appointment associated to id = {invoiceRequest.AppointmentId} not found.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    }
                    Appointment newAppointment = new Appointment();
                    newAppointment.PatientId = invoiceRequest.PatientId;
                    newAppointment.DoctorId = invoiceRequest.DoctorId;
                    newAppointment.ReceptionistId = invoiceRequest.ReceptionistId;
                    newAppointment.Code = invoiceRequest.AppointmentCode;
                    newAppointment.Date = DateTime.UtcNow.Date;
                    newAppointment.ConsultationDate = DateTime.UtcNow.Date;
                    newAppointment.Type = invoiceRequest.AppointmentType;
                    newAppointment.PatientCategory = invoiceRequest.AppointmentPatientCategory;
                    await _db.Appointments.AddAsync(newAppointment);
                    await _db.SaveChangesAsync();

                    appointmentId = newAppointment.Id;
                    appointmentDate = newAppointment.Date;
                }
                else if (appointment.Date.Date != DateTime.UtcNow.Date)
                {
                    Appointment newAppointment = new Appointment();
                    newAppointment.PatientId = invoiceRequest.PatientId;
                    newAppointment.DoctorId = invoiceRequest.DoctorId;
                    newAppointment.ReceptionistId = invoiceRequest.ReceptionistId;
                    newAppointment.Code = invoiceRequest.AppointmentCode;
                    newAppointment.Date = DateTime.UtcNow.Date;
                    newAppointment.ConsultationDate = DateTime.UtcNow.Date;
                    newAppointment.Type = invoiceRequest.AppointmentType;
                    newAppointment.PatientCategory = invoiceRequest.AppointmentPatientCategory;
                    await _db.Appointments.AddAsync(newAppointment);
                    await _db.SaveChangesAsync();

                    appointmentId = newAppointment.Id;
                    appointmentDate = newAppointment.Date;
                }

                AppointmentDetail appointmentDetail = new AppointmentDetail();
                appointmentDetail.AppointmentId = appointmentId;
                appointmentDetail.HasDischarged = invoiceRequest.AppointmentDetailsHasDischarged;
                appointmentDetail.WalkinType = invoiceRequest.AppointmentDetailsWalkinType;
                await _db.AppointmentDetails.AddAsync(appointmentDetail);
                await _db.SaveChangesAsync();

                Invoice invoice = new Invoice();
                invoice.AppointmentId = appointmentId;
                invoice.DoctorId = invoiceRequest.DoctorId;
                invoice.PatientId = invoiceRequest.PatientId;
                invoice.ReceptionistId = invoiceRequest.ReceptionistId;
                invoice.Date = appointmentDate;
                invoice.CheckupType = invoiceRequest.InvoiceCheckupType;
                invoice.CheckupFee = invoiceRequest.InvoiceCheckupFee;
                invoice.PaymentType = invoiceRequest.InvoicePaymentType;
                invoice.Disposibles = invoiceRequest.InvoiceDisposibles;
                invoice.GrossAmount = invoiceRequest.InvoiceGrossAmount;
                await _db.Invoices.AddAsync(invoice);
                await _db.SaveChangesAsync();

                if (invoiceRequest.ProcedureList != null)
                {
                    if (invoiceRequest.ProcedureList.Count > 0)
                    {
                        foreach (InvoiceProcedureRequest irProcedure in invoiceRequest.ProcedureList)
                        {
                            InvoiceProcedures invoiceProcedures = new InvoiceProcedures();
                            invoiceProcedures.ProcedureId = irProcedure.ProcedureId;
                            invoiceProcedures.InvoiceId = invoice.Id;
                            await _db.InvoiceProcedures.AddAsync(invoiceProcedures);
                            await _db.SaveChangesAsync();
                        }
                    }
                }

                Receipt receipt = new Receipt();
                receipt.PatientId = invoiceRequest.PatientId;
                receipt.ReceiptionistId = invoiceRequest.ReceptionistId;
                receipt.DoctorId = invoiceRequest.DoctorId;
                receipt.InvoiceId = invoice.Id;
                receipt.Pmid = invoiceRequest.ReceiptPmid;
                receipt.Discount = invoiceRequest.ReceiptDiscount;
                receipt.TotalAmount = invoiceRequest.ReceiptTotalAmount;
                receipt.PendingAmount = invoiceRequest.ReceiptPendingAmount;
                receipt.PaidAmount = invoiceRequest.ReceiptPaidAmount;
                receipt.DoctorFee = invoiceRequest.DoctorConsultationFee;
                await _db.Receipts.AddAsync(receipt);
                await _db.SaveChangesAsync();

                invoice.Doctor = doctor;
                invoice.Patient = patient;

                transaction.Commit();
                return new Response<Invoice>(true, "Success: Created object.", invoice);
            }
            catch (Exception exception)
            {
                transaction.Rollback();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<Invoice>(false, $"Server Failure: Unable to insert object. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpPost("post/doctor/share")]
        public async Task<Response<List<Invoice>>> CalculateDoctorShare(DoctorShareRequest request)
        {
            try
            {
                int doctorShare = 0;
                List<Invoice> invoiceList;
                if (request != null)
                {
                    if (request.CheckupType.Equals("Procedure"))
                    {
                        invoiceList = await _db.Invoices.Where(x => (x.CheckupType == request.CheckupType) &&
                        (x.Date >= request.FromDate.Date && x.Date < request.ToDate.Date.AddDays(1))).
                        Include(x => x.Doctor).Include(x => x.Doctor.User).Include(x => x.Patient).
                        Include(x => x.Patient.User).Include(x => x.Receipt).Include(x => x.InvoiceProcedures).
                        Include(x => x.InvoiceProcedures.Procedures).ToListAsync();

                        if (invoiceList != null)
                        {
                            if (invoiceList.Count > 0)
                            {
                                foreach (Invoice invoice in invoiceList)
                                {
                                    if (invoice.InvoiceProcedures.Procedures.Executant.Equals("Doctor"))
                                    {
                                        int doctorShareFromProcedure = (invoice.InvoiceProcedures.Procedures.Charges / 100) * invoice.InvoiceProcedures.Procedures.ExecutantShare;
                                        doctorShare += doctorShareFromProcedure;
                                    }
                                    else
                                    {
                                        invoiceList.Remove(invoice);
                                    }
                                }
                                return new Response<List<Invoice>>(true, doctorShare.ToString(), invoiceList);
                            }
                        }
                    }
                    else
                    {
                        invoiceList = await _db.Invoices.Where(x => (x.CheckupType == request.CheckupType) && (x.Date >= request.FromDate.Date &&
                        x.Date < request.ToDate.Date.AddDays(1))).Include(x => x.Doctor).Include(x => x.Doctor.User).
                        Include(x => x.Patient).Include(x => x.Patient.User).Include(x => x.Receipt).ToListAsync();

                        if (invoiceList != null)
                        {
                            if (invoiceList.Count > 0)
                            {
                                foreach (Invoice invoice in invoiceList)
                                {
                                    doctorShare += invoice.Receipt.DoctorFee;
                                }
                                return new Response<List<Invoice>>(true, doctorShare.ToString(), invoiceList);
                            }
                        }
                    }
                }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Invoice>>(false, "Failure: Database is empty.", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new Response<List<Invoice>>(false, $"Server Failure: Unable to get data. Because {exception.Message}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }



    }
}
