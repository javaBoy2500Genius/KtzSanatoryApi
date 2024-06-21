using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KTZSanatoriumServiceApi.Models
{
    public enum SuperAdminFieldSort
    {
        UnSelect,
        Branch,
        ComeInDate,
        DReport,
        IIN,
        Plot,
        Name,
        TypeSocHelp,
        FullName,
    }

    public enum UserRole
    {
        User,
        Admin,
        SuperAdmin
    }

    public enum TaxType
    {
        Percent,
        Sum
    }
    public enum ApplicationStatus
    {
        Consideration,
        Acept,
        Decline,

    }

    public enum SocialTypeHelp
    {
        Sanatory
    }

    public enum TripStatus
    {
        Waiting,
        Caneled,
        Approved

    }
}
