using System;
using System.Collections.Generic;

namespace JWT.Models;

public partial class Userlogin
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }
}
