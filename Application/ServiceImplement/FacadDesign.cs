using Application.ServiceContract;
using Infrastructure.DataBaseRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ServiceImplement
{
    internal class FacadDesign : IFacadDesign
    {
        
        private IGenericRepository _repository;
        public FacadDesign(IGenericRepository repository)
        {
            _repository = repository;
        }
        public IGenericRepository repository { get  { return _repository; } set  { _repository = value; } }

        public async Task SaveChangeAsync()
        {
            await _repository.SaveChangeAsync();
        }
    }
}
