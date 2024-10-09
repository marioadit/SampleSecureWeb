using System;
using SampleSecureWeb.Models;

namespace SampleSecureWeb.Data;

public interface IUser
{
    User Login(User user);
    User Registration(User user);
}
