using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReddit.FileService.Contracts
{
    public interface ITokenService
    {
        void SaveTokenAsync(string token);

        string GetTokenAsync();
    }
}
