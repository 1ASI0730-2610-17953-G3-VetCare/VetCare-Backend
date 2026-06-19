using VetCare.scheduling.domain.model.aggregates;
using VetCare.scheduling.domain.model.valueobjects;
using VetCare.scheduling.domain.repositories;

namespace VetCare.scheduling.application;

public class AppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;

    public AppointmentService(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Appointment> CreateAppointmentAsync(int patientId, int clientId, DateTime date, TimeSpan startTime, TimeSpan endTime, string type)
    {
        // Add overlap validation logic here if needed (e.g., query repo for existing appointments on the same date/time)
        var timeSlot = new TimeSlot(startTime, endTime);
        var appointment = new Appointment(patientId, clientId, date, timeSlot, type);
        
        await _appointmentRepository.AddAsync(appointment);
        return appointment;
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
    {
        return await _appointmentRepository.GetAllAsync();
    }
    
    public async Task ConfirmAppointmentAsync(int id)
    {
        var appointment = await _appointmentRepository.FindByIdAsync(id);
        if (appointment == null) throw new ArgumentException("Appointment not found");
        
        appointment.Confirm();
        await _appointmentRepository.UpdateAsync(appointment);
    }
}
