using System;
using System.Collections.Generic;

namespace ApiTemplate.Models;

public partial class TblOrganisation
{
    public long OrganisationId { get; set; }

    public string OrganisationRefNo { get; set; } = null!;

    public string OrganisationTitle { get; set; } = null!;

    public DateTime CreationDate { get; set; }
}
