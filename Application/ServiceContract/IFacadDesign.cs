using Infrastructure.DataBaseRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ServiceContract
{
    public interface IFacadDesign
    {
        public IGenericRepository repository { get; set; }
        Task SaveChangeAsync();
    }

}
