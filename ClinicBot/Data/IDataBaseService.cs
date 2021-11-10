using ClinicBot.Common.Models.MedicalAppointment;
using ClinicBot.Common.Models.Qualification;
using ClinicBot.Common.Models.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicBot.Data
{
    public interface IDataBaseService
    {
        DbSet<UserModel> User { get; set; }

        DbSet<QualificationModel> Qualification { get; set; }

        DbSet<MedicalAppointmentModel> MedicalAppointment { get; set; }

        Task<bool> SaveAsync();
    }
}
