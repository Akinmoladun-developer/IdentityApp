using System.ComponentModel.DataAnnotations;
using System;

namespace Api.DTOs.Account
{
        public class UserDto
        {
            // The DTO Class will be passed to the User or the Client
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string JWT { get; set; }
        }
    

}
